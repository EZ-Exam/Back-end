using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Dtos;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.QuestionsService
{
    public class QuestionsService : IQuestionsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QuestionsService> _logger;

        public QuestionsService(IUnitOfWork unitOfWork, ILogger<QuestionsService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<QuestionDataResponse> AddQuestionAsync(QuestionDataRequest questionDataRequest)
        {
            if (questionDataRequest == null)
            {
                _logger.LogWarning("QuestionDataRequest is null.");
                throw new ArgumentNullException(nameof(questionDataRequest), "Question data request cannot be null.");
            }

            if (await _unitOfWork.UserRepository.GetByIdAsync(questionDataRequest.CreatedByUserId) == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", questionDataRequest.CreatedByUserId);
                throw new NotFoundException($"User with ID {questionDataRequest.CreatedByUserId} not found.");
            }

            var question = new Question
            {
                Content = questionDataRequest.Content,
                QuestionSource = questionDataRequest.QuestionSource,
                DifficultyLevelId = questionDataRequest.DifficultyLevelId,
                Image = questionDataRequest.Image,
                LessonId = questionDataRequest.LessonId,
                CreatedByUserId = questionDataRequest.CreatedByUserId,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _unitOfWork.QuestionRepository.CreateAsync(question);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                _logger.LogInformation("Question with ID {QuestionId} created successfully.", question.Id);
                var response = new QuestionDataResponse
                {
                    Id = question.Id,
                    Content = question.Content,
                    QuestionSource = question.QuestionSource,
                    DifficultyLevel = question.DifficultyLevel?.Name ?? "Unknown",
                    LessonId = question.LessonId,
                    CreatedByUserId = question.CreatedByUserId,
                    CreatedAt = question.CreatedAt,
                    UpdatedAt = question.UpdatedAt
                };

                return response;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question: {Message}", ex.Message);
                throw new ApplicationException("An error occurred while creating the question.", ex);
            }
        }

        public async Task DeleteQuestionAsync(int id)
        {
            var target = await _unitOfWork.QuestionRepository.GetByIdAsync(id);
            if (target == null)
            {
                _logger.LogWarning("Question with ID {QuestionId} not found.", id);
                throw new NotFoundException($"Question with ID {id} not found.");
            }

            await _unitOfWork.QuestionRepository.RemoveAsync(target);
            await _unitOfWork.SaveChangesWithTransactionAsync();
            _logger.LogInformation("Question with ID {QuestionId} deleted successfully.", id);
        }

        public async Task<QuestionDataResponse> GetQuestionById(int id)
        {
            var question = await _unitOfWork.QuestionRepository.GetByIdAsync(id);
            if (question == null)
                throw new NotFoundException($"Question with ID {id} not found.");

            // Get answers for options
            var answers = await _unitOfWork.AnswerRepository.GetByQuestionIdAsync(id);

            return new QuestionDataResponse
            {
                Id = question.Id,
                Content = question.Content,
                QuestionSource = question.QuestionSource,
                DifficultyLevel = question.DifficultyLevel?.Name ?? "Unknown",
                Image = question.Image,
                LessonId = question.LessonId,
                ChapterId = question.ChapterId,
                CreatedByUserId = question.CreatedByUserId,
                CreatedAt = question.CreatedAt,
                UpdatedAt = question.UpdatedAt,
                LessonName = question.Lesson?.Name,
                ChapterName = question.Chapter?.Name,
                CreatedByUserName = question.CreatedByUser?.Email,
                // New fields from Question model (temporarily set to null until migration)
                Formula = null,
                CorrectAnswer = null,
                Explanation = null,
                Type = question.QuestionType?.ToLower() ?? "multiple-choice",
                Options = answers?.Select(a => a.Content).ToList() ?? new List<string>()
            };
        }

        public async Task<QuestionDataResponse> ModifyQuestionAsync(UpdateQuestionRequest questionDataRequest)
        {
            if (questionDataRequest == null)
            {
                _logger.LogWarning("QuestionDataRequest is null.");
                throw new ArgumentNullException(nameof(questionDataRequest));
            }

            var existingQuestion = await _unitOfWork.QuestionRepository.GetByIdAsync(questionDataRequest.Id);
            if (existingQuestion == null)
            {
                _logger.LogWarning("Question with ID {QuestionId} not found.", questionDataRequest.Id);
                throw new NotFoundException($"Question with ID {questionDataRequest.Id} not found.");
            }

            existingQuestion.Content = questionDataRequest.Content;
            //existingQuestion.QuestionSource = questionDataRequest.QuestionSource;
            existingQuestion.DifficultyLevelId = questionDataRequest.DifficultyLevelId;
            //existingQuestion.Image = questionDataRequest.Image;
            //existingQuestion.LessonId = questionDataRequest.LessonId;
            existingQuestion.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.QuestionRepository.UpdateAsync(existingQuestion);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("Question with ID {QuestionId} updated successfully.", existingQuestion.Id);

                // ? Tr? v? d? li?u sau khi c?p nh?t
                return new QuestionDataResponse
                {
                    Id = existingQuestion.Id,
                    Content = existingQuestion.Content,
                    QuestionSource = existingQuestion.QuestionSource,
                    DifficultyLevel = existingQuestion.DifficultyLevel?.Name ?? "Unknown",
                    Image = existingQuestion.Image,
                    LessonId = existingQuestion.LessonId,
                    CreatedAt = existingQuestion.CreatedAt,
                    UpdatedAt = existingQuestion.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question with ID {QuestionId}: {Message}", existingQuestion.Id, ex.Message);
                throw new ApplicationException($"Error updating question with ID {existingQuestion.Id}", ex);
            }
        }




        public async Task<PagedResponse<QuestionDataResponse>> GetQuestionsAsync(
    int? pageNumber = null,
    int? pageSize = null,
    string? search = null,
    string? sort = null,
    int? lessonId = null,
    string? difficultyLevel = null,
    int? chapterId = null,
    int isSort = 0,
    int? createdByUserId = null) // Th�m l?c theo user
        {
            try
            {
                List<Question> questions;
                int totalItems;

                if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
                {
                    // N?u b?n c� s?a GetPagedAsync d? truy?n th�m createdByUserId th� truy?n v�o d�y
                    (questions, totalItems) = await _unitOfWork.QuestionRepository.GetPagedAsync(
                        pageNumber.Value,
                        pageSize.Value,
                        search,
                        sort,
                        lessonId,
                        difficultyLevel,
                        chapterId,
                        isSort,
                        createdByUserId ?? 0); // c?n th�m n?u repo c� x? l�
                }
                else
                {
                    questions = await _unitOfWork.QuestionRepository.GetAllAsync() ?? new List<Question>();

                    // L?c theo user t?o
                    if (createdByUserId.HasValue)
                    {
                        questions = questions.Where(q => q.CreatedByUserId == createdByUserId.Value).ToList();
                    }

                    // L?c theo search
                    if (!string.IsNullOrEmpty(search))
                    {
                        var searchNormalized = search.RemoveDiacritics().ToLower();
                        questions = questions.Where(q =>
                            q.Content.RemoveDiacritics().ToLower().Contains(searchNormalized) ||
                            q.QuestionSource.RemoveDiacritics().ToLower().Contains(searchNormalized)).ToList();
                    }

                    if (lessonId.HasValue)
                    {
                        questions = questions.Where(q => q.LessonId == lessonId.Value).ToList();
                    }

                    if (!string.IsNullOrEmpty(difficultyLevel))
                    {
                        questions = questions.Where(q => q.DifficultyLevel.Name == difficultyLevel || q.DifficultyLevel.Code == difficultyLevel).ToList();
                    }

                    if (chapterId.HasValue)
                    {
                        var lessonIds = await _unitOfWork.LessonRepository.GetAllAsync();
                        lessonIds = lessonIds.Where(l => l.ChapterId == chapterId.Value).ToList();
                        questions = questions.Where(q => lessonIds.Any(l => l.Id == q.LessonId)).ToList();
                    }

                    if (isSort == 1)
                    {
                        questions = sort?.ToLower() switch
                        {
                            "content:asc" => questions.OrderBy(q => q.Content).ToList(),
                            "content:desc" => questions.OrderByDescending(q => q.Content).ToList(),
                            "difficultylevel:asc" => questions.OrderBy(q => q.DifficultyLevel.Name).ToList(),
                            "difficultylevel:desc" => questions.OrderByDescending(q => q.DifficultyLevel.Name).ToList(),
                            "createdat:asc" => questions.OrderBy(q => q.CreatedAt).ToList(),
                            "createdat:desc" => questions.OrderByDescending(q => q.CreatedAt).ToList(),
                            "updatedat:asc" => questions.OrderBy(q => q.UpdatedAt).ToList(),
                            "updatedat:desc" => questions.OrderByDescending(q => q.UpdatedAt).ToList(),
                            _ => questions.OrderByDescending(q => q.CreatedAt).ToList()
                        };
                    }
                    else
                    {
                        questions = questions.OrderBy(q => q.Id).ToList();
                    }

                    totalItems = questions.Count;
                }
                var allLessons = await _unitOfWork.LessonRepository.GetAllAsync();
                var allUsers = await _unitOfWork.UserRepository.GetAllAsync();
                var allChapters = await _unitOfWork.ChapterRepository.GetAllAsync();
                
                var questionResponses = new List<QuestionDataResponse>();
                foreach (var q in questions)
                {
                    var answers = await _unitOfWork.AnswerRepository.GetByQuestionIdAsync(q.Id);
                    questionResponses.Add(new QuestionDataResponse
                    {
                        Id = q.Id,
                        Content = q.Content,
                        QuestionSource = q.QuestionSource,
                        DifficultyLevel = q.DifficultyLevel?.Name ?? "Unknown",
                        LessonId = q.LessonId,
                        ChapterId = q.ChapterId,
                        CreatedByUserId = q.CreatedByUserId,
                        CreatedAt = q.CreatedAt,
                        UpdatedAt = q.UpdatedAt,
                        LessonName = allLessons.FirstOrDefault(l => l.Id == q.LessonId)?.Name ?? string.Empty,
                        ChapterName = allChapters.FirstOrDefault(c => c.Id == q.ChapterId)?.Name ?? string.Empty,
                        CreatedByUserName = allUsers.FirstOrDefault(u => u.Id == q.CreatedByUserId)?.Email ?? string.Empty,
                        Formula = null,
                        CorrectAnswer = null,
                        Explanation = null,
                        Type = q.QuestionType?.ToLower() ?? "multiple-choice",
                        Options = answers?.Select(a => a.Content).ToList() ?? new List<string>()
                    });
                }

                return new PagedResponse<QuestionDataResponse>(
                    questionResponses,
                    pageNumber ?? 1,
                    pageSize ?? totalItems,
                    totalItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions: {Message}", ex.Message);
                throw new ApplicationException("Error retrieving questions.", ex);
            }
        }


    }
}
