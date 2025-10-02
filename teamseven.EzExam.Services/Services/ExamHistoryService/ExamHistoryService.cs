using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Interfaces;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.ExamHistoryService
{
    public class ExamHistoryService : IExamHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ExamHistoryService> _logger;

        public ExamHistoryService(IUnitOfWork unitOfWork, ILogger<ExamHistoryService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> CreateExamHistoryAsync(CreateExamHistoryRequest request)
        {
            try
            {
                var examHistory = new ExamHistory
                {
                    ExamId = request.ExamId,
                    UserId = request.UserId,
                    Score = request.Score,
                    CorrectCount = request.CorrectCount,
                    IncorrectCount = request.IncorrectCount,
                    UnansweredCount = request.UnansweredCount,
                    TotalQuestions = request.TotalQuestions,
                    SubmittedAt = request.SubmittedAt,
                    TimeTaken = request.TimeTaken,
                    Answers = request.Answers != null ? JsonConvert.SerializeObject(request.Answers) : null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.ExamHistoryRepository.CreateAsync(examHistory);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation($"Created exam history {examHistory.Id} for user {request.UserId}");
                return examHistory.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating exam history for user {request.UserId}");
                throw;
            }
        }

        public async Task<ExamHistoryResponse?> GetExamHistoryByIdAsync(int id)
        {
            try
            {
                var examHistory = await _unitOfWork.ExamHistoryRepository.GetByIdAsync(id);
                if (examHistory == null) return null;

                return MapToResponse(examHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving exam history {id}");
                throw;
            }
        }

        public async Task<IEnumerable<ExamHistoryResponse>> GetExamHistoriesByUserIdAsync(string userId)
        {
            try
            {
                var examHistories = await _unitOfWork.ExamHistoryRepository.GetAllAsync();
                var userHistories = examHistories.Where(h => h.UserId == userId).OrderByDescending(h => h.SubmittedAt);
                
                return userHistories.Select(MapToResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving exam histories for user {userId}");
                throw;
            }
        }

        public async Task<IEnumerable<ExamHistoryResponse>> GetExamHistoriesByExamIdAsync(string examId)
        {
            try
            {
                var examHistories = await _unitOfWork.ExamHistoryRepository.GetAllAsync();
                var examSpecificHistories = examHistories.Where(h => h.ExamId == examId).OrderByDescending(h => h.SubmittedAt);
                
                return examSpecificHistories.Select(MapToResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving exam histories for exam {examId}");
                throw;
            }
        }

        public async Task<IEnumerable<ExamHistoryResponse>> GetAllExamHistoriesAsync()
        {
            try
            {
                var examHistories = await _unitOfWork.ExamHistoryRepository.GetAllAsync();
                return examHistories.OrderByDescending(h => h.SubmittedAt).Select(MapToResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all exam histories");
                throw;
            }
        }

        public async Task UpdateExamHistoryAsync(int id, CreateExamHistoryRequest request)
        {
            try
            {
                var examHistory = await _unitOfWork.ExamHistoryRepository.GetByIdAsync(id);
                if (examHistory == null)
                    throw new ArgumentException("Exam history not found");

                examHistory.Score = request.Score;
                examHistory.CorrectCount = request.CorrectCount;
                examHistory.IncorrectCount = request.IncorrectCount;
                examHistory.UnansweredCount = request.UnansweredCount;
                examHistory.TotalQuestions = request.TotalQuestions;
                examHistory.SubmittedAt = request.SubmittedAt;
                examHistory.TimeTaken = request.TimeTaken;
                examHistory.Answers = request.Answers != null ? JsonConvert.SerializeObject(request.Answers) : null;
                examHistory.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.ExamHistoryRepository.UpdateAsync(examHistory);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation($"Updated exam history {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating exam history {id}");
                throw;
            }
        }

        public async Task DeleteExamHistoryAsync(int id)
        {
            try
            {
                var examHistory = await _unitOfWork.ExamHistoryRepository.GetByIdAsync(id);
                if (examHistory == null)
                    throw new ArgumentException("Exam history not found");

                await _unitOfWork.ExamHistoryRepository.DeleteAsync(examHistory);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation($"Deleted exam history {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting exam history {id}");
                throw;
            }
        }

        private ExamHistoryResponse MapToResponse(ExamHistory examHistory)
        {
            List<AnswerDetailResponse>? answers = null;
            if (!string.IsNullOrEmpty(examHistory.Answers))
            {
                try
                {
                    var answerDetails = JsonConvert.DeserializeObject<List<AnswerDetail>>(examHistory.Answers);
                    if (answerDetails != null)
                    {
                        answers = answerDetails.Select(a => new AnswerDetailResponse
                        {
                            QuestionId = a.QuestionId,
                            SelectedAnswer = a.SelectedAnswer,
                            CorrectAnswer = a.CorrectAnswer,
                            IsCorrect = a.IsCorrect,
                            TimeSpent = a.TimeSpent
                        }).ToList();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Failed to deserialize answers for exam history {examHistory.Id}");
                }
            }

            return new ExamHistoryResponse
            {
                Id = examHistory.Id,
                ExamId = examHistory.ExamId,
                UserId = examHistory.UserId,
                Score = examHistory.Score,
                CorrectCount = examHistory.CorrectCount,
                IncorrectCount = examHistory.IncorrectCount,
                UnansweredCount = examHistory.UnansweredCount,
                TotalQuestions = examHistory.TotalQuestions,
                SubmittedAt = examHistory.SubmittedAt,
                TimeTaken = examHistory.TimeTaken,
                Answers = answers,
                CreatedAt = examHistory.CreatedAt,
                UpdatedAt = examHistory.UpdatedAt
            };
        }
    }
}
