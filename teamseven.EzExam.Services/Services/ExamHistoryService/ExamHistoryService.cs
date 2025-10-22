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

                return await MapToResponseAsync(examHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving exam history {id}");
                throw;
            }
        }

        public async Task<IEnumerable<ExamHistoryResponse>> GetExamHistoriesByUserIdAsync(int userId)
        {
            try
            {
                var examHistories = await _unitOfWork.ExamHistoryRepository.GetAllAsync();
                var userHistories = examHistories.Where(h => h.UserId == userId).OrderByDescending(h => h.SubmittedAt);
                
                var responses = new List<ExamHistoryResponse>();
                foreach (var history in userHistories)
                {
                    responses.Add(await MapToResponseAsync(history));
                }
                return responses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving exam histories for user {userId}");
                throw;
            }
        }

        public async Task<IEnumerable<ExamHistoryMinimalResponse>> GetExamHistoriesMinimalByUserIdAsync(int userId)
        {
            try
            {
                // Lấy exam histories với Exam information để có SubjectId, GradeId, LessonId
                var examHistories = await _unitOfWork.ExamHistoryRepository.GetHistoryByUserIdWithExamAsync(userId);
                
                var responses = new List<ExamHistoryMinimalResponse>();
                foreach (var history in examHistories)
                {
                    responses.Add(new ExamHistoryMinimalResponse
                    {
                        ExamId = history.ExamId,
                        UserId = history.UserId,
                        Score = history.Score,
                        CorrectCount = history.CorrectCount,
                        TotalQuestions = history.TotalQuestions,
                        SubmittedAt = history.SubmittedAt,
                        TimeTaken = history.TimeTaken,
                        SubjectId = history.Exam?.SubjectId ?? 0,
                        SubjectName = history.Exam?.Subject?.Name ?? "Unknown",
                        GradeId = history.Exam?.GradeId,
                        GradeName = history.Exam?.Grade?.Name ?? "Unknown",
                        ChapterId = history.Exam?.Lesson?.ChapterId,
                        ChapterName = history.Exam?.Lesson?.Chapter?.Name ?? "Unknown",
                        LessonId = history.Exam?.LessonId,
                        LessonName = history.Exam?.Lesson?.Name ?? "Unknown"
                    });
                }
                return responses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving minimal exam histories for user {userId}");
                throw;
            }
        }

        public async Task<IEnumerable<ExamHistoryResponse>> GetExamHistoriesByExamIdAsync(int examId)
        {
            try
            {
                var examHistories = await _unitOfWork.ExamHistoryRepository.GetAllAsync();
                var examSpecificHistories = examHistories.Where(h => h.ExamId == examId).OrderByDescending(h => h.SubmittedAt);
                
                var responses = new List<ExamHistoryResponse>();
                foreach (var history in examSpecificHistories)
                {
                    responses.Add(await MapToResponseAsync(history));
                }
                return responses;
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
                var responses = new List<ExamHistoryResponse>();
                foreach (var history in examHistories.OrderByDescending(h => h.SubmittedAt))
                {
                    responses.Add(await MapToResponseAsync(history));
                }
                return responses;
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

        public async Task<IEnumerable<ExamQuestionDetailResponse>> GetExamQuestionsDetailAsync(int examId)
        {
            try
            {
                var examQuestions = await _unitOfWork.ExamQuestionRepository.GetAllAsync();
                var examSpecificQuestions = examQuestions.Where(eq => eq.ExamId == examId).ToList();
                
                if (!examSpecificQuestions.Any())
                {
                    _logger.LogWarning($"No questions found for exam {examId}");
                    return new List<ExamQuestionDetailResponse>();
                }
              
                var questionIds = examSpecificQuestions.Select(eq => eq.QuestionId).ToList();                         
                var questions = await _unitOfWork.QuestionRepository.GetByIdsAsync(questionIds);
                var responses = new List<ExamQuestionDetailResponse>();
                
                foreach (var question in questions)
                {
                    var response = new ExamQuestionDetailResponse
                    {
                        Id = question.Id,
                        ContentQuestion = question.Content,
                        CorrectAnswer = question.CorrectAnswer,
                        Options = ParseOptions(question.Options),
                        Explanation = question.Explanation,
                        ImageUrl = question.Image,
                        Formula = question.Formula
                    };
                    
                    responses.Add(response);
                }

                _logger.LogInformation($"Retrieved {responses.Count} questions for exam {examId}");
                return responses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving exam questions for exam {examId}");
                throw;
            }
        }

        private async Task<ExamHistoryResponse> MapToResponseAsync(ExamHistory examHistory)
        {
            List<AnswerDetailResponse>? answers = null;
            if (!string.IsNullOrEmpty(examHistory.Answers))
            {
                try
                {
                    var answerDetails = JsonConvert.DeserializeObject<List<AnswerDetail>>(examHistory.Answers);
                    if (answerDetails != null && answerDetails.Any())
                    {
                        var questionIds = answerDetails.Select(a => int.Parse(a.QuestionId)).ToList();
                        
                        var questions = await _unitOfWork.QuestionRepository.GetByIdsAsync(questionIds);
                        var questionDict = questions.ToDictionary(q => q.Id, q => q);
                        
                        answers = new List<AnswerDetailResponse>();
                        
                        foreach (var answerDetail in answerDetails)
                        {
                            var questionId = int.Parse(answerDetail.QuestionId);
                            var question = questionDict.GetValueOrDefault(questionId);
                            
                            var answerResponse = new AnswerDetailResponse
                            {
                                QuestionId = answerDetail.QuestionId,
                                SelectedAnswer = answerDetail.SelectedAnswer,
                                CorrectAnswer = answerDetail.CorrectAnswer,
                                IsCorrect = answerDetail.IsCorrect,
                                TimeSpent = answerDetail.TimeSpent,
                            
                                ContentQuestion = question?.Content ?? string.Empty,
                                Options = ParseOptions(question?.Options),
                                Explanation = question?.Explanation,
                                ImageUrl = question?.Image,
                                Formula = question?.Formula
                            };
                            
                            answers.Add(answerResponse);
                        }
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

        private List<string> ParseOptions(string? optionsJson)
        {
            if (string.IsNullOrEmpty(optionsJson))
                return new List<string>();

            try
            {
                // Thử parse JSON array
                var jsonArray = JsonConvert.DeserializeObject<List<string>>(optionsJson);
                if (jsonArray != null)
                    return jsonArray;

                // Thử parse JSON object (key-value pairs)
                var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(optionsJson);
                if (jsonObject != null)
                    return jsonObject.Values.ToList();

                return new List<string>();
            }
            catch
            {
                // Nếu không parse được JSON, trả về empty list
                return new List<string>();
            }
        }
    }
}
