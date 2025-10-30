using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
                SubjectId = questionDataRequest.SubjectId,
                ChapterId = questionDataRequest.ChapterId,
                LessonId = questionDataRequest.LessonId,
                TextbookId = questionDataRequest.TextbookId,
                QuestionType = string.IsNullOrWhiteSpace(questionDataRequest.QuestionType) ? "MULTIPLE_CHOICE" : questionDataRequest.QuestionType!,
                Image = questionDataRequest.Image,
                TemplateQuestionId = questionDataRequest.TemplateQuestionId,
                CreatedByUserId = questionDataRequest.CreatedByUserId,
                Formula = questionDataRequest.Formula,
                CorrectAnswer = questionDataRequest.CorrectAnswer,
                Explanation = questionDataRequest.Explanation,
                Options = questionDataRequest.Options != null ? JsonSerializer.Serialize(questionDataRequest.Options) : null,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _unitOfWork.QuestionRepository.CreateAsync(question);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                var response = new QuestionDataResponse
                {
                    Id = question.Id,
                    Content = question.Content,
                    QuestionSource = question.QuestionSource,
                    DifficultyLevelId = question.DifficultyLevelId,
                    LessonId = question.LessonId,
                    TextbookId = question.TextbookId,
                    ChapterId = question.ChapterId,
                    CreatedByUserId = question.CreatedByUserId,
                    CreatedAt = question.CreatedAt,
                    UpdatedAt = question.UpdatedAt,
                    Formula = question.Formula,
                    CorrectAnswer = question.CorrectAnswer,
                    Explanation = question.Explanation,
                    Type = question.QuestionType?.ToLower() ?? "multiple-choice",
                    Options = question.Options != null ? JsonSerializer.Deserialize<List<string>>(question.Options) ?? new List<string>() : (questionDataRequest.Options ?? new List<string>())
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
                DifficultyLevelId = question.DifficultyLevelId,
                Image = question.Image,
                LessonId = question.LessonId,
                ChapterId = question.ChapterId,
                CreatedByUserId = question.CreatedByUserId,
                CreatedAt = question.CreatedAt,
                UpdatedAt = question.UpdatedAt,
                LessonName = question.Lesson?.Name,
                ChapterName = question.Chapter?.Name,
                CreatedByUserName = question.CreatedByUser?.Email,
                // New fields from Question model
                Formula = question.Formula,
                CorrectAnswer = question.CorrectAnswer,
                Explanation = question.Explanation,
                Type = question.QuestionType?.ToLower() ?? "multiple-choice",
                Options = question.Options != null ? JsonSerializer.Deserialize<List<string>>(question.Options) ?? new List<string>() : (answers?.Select(a => a.Content).ToList() ?? new List<string>())
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
            existingQuestion.DifficultyLevelId = questionDataRequest.DifficultyLevelId;
            if (questionDataRequest.QuestionSource != null) existingQuestion.QuestionSource = questionDataRequest.QuestionSource;
            if (questionDataRequest.SubjectId.HasValue) existingQuestion.SubjectId = questionDataRequest.SubjectId.Value;
            if (questionDataRequest.ChapterId.HasValue) existingQuestion.ChapterId = questionDataRequest.ChapterId;
            if (questionDataRequest.LessonId.HasValue) existingQuestion.LessonId = questionDataRequest.LessonId;
            if (questionDataRequest.TextbookId.HasValue) existingQuestion.TextbookId = questionDataRequest.TextbookId;
            if (questionDataRequest.Image != null) existingQuestion.Image = questionDataRequest.Image;
            if (!string.IsNullOrWhiteSpace(questionDataRequest.QuestionType)) existingQuestion.QuestionType = questionDataRequest.QuestionType!;
            if (questionDataRequest.TemplateQuestionId.HasValue) existingQuestion.TemplateQuestionId = questionDataRequest.TemplateQuestionId;
            if (questionDataRequest.Formula != null) existingQuestion.Formula = questionDataRequest.Formula;
            if (questionDataRequest.CorrectAnswer != null) existingQuestion.CorrectAnswer = questionDataRequest.CorrectAnswer;
            if (questionDataRequest.Explanation != null) existingQuestion.Explanation = questionDataRequest.Explanation;
            if (questionDataRequest.Options != null) existingQuestion.Options = JsonSerializer.Serialize(questionDataRequest.Options);
            existingQuestion.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.QuestionRepository.UpdateAsync(existingQuestion);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                // Return updated data
                return new QuestionDataResponse
                {
                    Id = existingQuestion.Id,
                    Content = existingQuestion.Content,
                    QuestionSource = existingQuestion.QuestionSource,
                    DifficultyLevelId = existingQuestion.DifficultyLevelId,
                    Image = existingQuestion.Image,
                    LessonId = existingQuestion.LessonId,
                    ChapterId = existingQuestion.ChapterId,
                    CreatedAt = existingQuestion.CreatedAt,
                    UpdatedAt = existingQuestion.UpdatedAt,
                    Formula = existingQuestion.Formula,
                    CorrectAnswer = existingQuestion.CorrectAnswer,
                    Explanation = existingQuestion.Explanation,
                    Type = existingQuestion.QuestionType?.ToLower() ?? "multiple-choice",
                    Options = existingQuestion.Options != null ? JsonSerializer.Deserialize<List<string>>(existingQuestion.Options) ?? new List<string>() : (questionDataRequest.Options ?? new List<string>())
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
    int? createdByUserId = null,
    int? textbookId = null) // Th�m l?c theo user
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
                        createdByUserId ?? 0,
                        textbookId); // c?n th�m n?u repo c� x? l�
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
                    if (textbookId.HasValue)
                        questions = questions.Where(q => q.TextbookId == textbookId.Value).ToList();

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
                        DifficultyLevelId = q.DifficultyLevelId,
                        LessonId = q.LessonId,
                        ChapterId = q.ChapterId,
                        TextbookId = q.TextbookId,
                        CreatedByUserId = q.CreatedByUserId,
                        CreatedAt = q.CreatedAt,
                        UpdatedAt = q.UpdatedAt,
                        LessonName = allLessons.FirstOrDefault(l => l.Id == q.LessonId)?.Name ?? string.Empty,
                        ChapterName = allChapters.FirstOrDefault(c => c.Id == q.ChapterId)?.Name ?? string.Empty,
                        CreatedByUserName = allUsers.FirstOrDefault(u => u.Id == q.CreatedByUserId)?.Email ?? string.Empty,
                        Formula = q.Formula,
                        CorrectAnswer = q.CorrectAnswer,
                        Explanation = q.Explanation,
                        Type = q.QuestionType?.ToLower() ?? "multiple-choice",
                        Options = q.Options != null ? JsonSerializer.Deserialize<List<string>>(q.Options) ?? new List<string>() : (answers?.Select(a => a.Content).ToList() ?? new List<string>())
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

        /// <summary>
        /// Optimized feed for questions: single query projection that only retrieves metadata (counts) and shallow fields.
        /// Avoids N+1 by using subqueries in projection and DbContext for efficient SQL translation.
        /// </summary>
        public async Task<PagedResponse<QuestionFeedResponse>> GetOptimizedQuestionsFeedAsync(
            int currentUserId,
            int page = 1,
            int pageSize = 20,
            string? search = null,
            int? lessonId = null,
            int? difficultyLevelId = null)
        {
            // Basic validation / normalize paging
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;
            var skip = (page - 1) * pageSize;

            try
            {
                var ctx = _unitOfWork.Context;

                // Base query: only active questions
                var baseQuery = ctx.Questions.AsQueryable().Where(q => q.IsActive == true);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var lowered = search.Trim();
                    baseQuery = baseQuery.Where(q => q.Content.ToLower().Contains(lowered.ToLower()));
                }

                if (lessonId.HasValue)
                    baseQuery = baseQuery.Where(q => q.LessonId == lessonId.Value);

                if (difficultyLevelId.HasValue)
                    baseQuery = baseQuery.Where(q => q.DifficultyLevelId == difficultyLevelId.Value);

                var totalItems = await baseQuery.CountAsync();

                var projected = await baseQuery
                    .OrderByDescending(q => q.CreatedAt)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(q => new QuestionFeedResponse
                    {
                        Id = q.Id,
                        Content = q.Content,
                        CreatedAt = q.CreatedAt,
                        UpdatedAt = q.UpdatedAt,
                        LessonId = q.LessonId,
                        LessonName = q.Lesson != null ? q.Lesson.Name : null,
                        ChapterName = q.Chapter != null ? q.Chapter.Name : null,
                        CreatedByUserId = q.CreatedByUserId,
                        CreatedByUserName = q.CreatedByUser != null ? q.CreatedByUser.FullName : null,
                        DifficultyLevel = q.DifficultyLevel != null ? q.DifficultyLevel.Name : null,
                        Type = q.QuestionType != null ? q.QuestionType.ToLower() : "multiple-choice",

                        // metadata counts are translated to subqueries in SQL by EF Core
                        AnswerCount = ctx.Answers.Count(a => a.QuestionId == q.Id),
                        CommentCount = ctx.QuestionComments.Count(c => c.QuestionId == q.Id && (c.IsDeleted == false || c.IsDeleted == null)),

                        // whether the current user has any recorded attempt / answered this question
                        IsAnsweredByCurrentUser = ctx.UserQuestionAttempts.Any(uqa => uqa.QuestionId == q.Id && uqa.UserId == currentUserId)
                    })
                    .AsNoTracking()
                    .ToListAsync();

                return new PagedResponse<QuestionFeedResponse>(projected, page, pageSize, totalItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving optimized questions feed: {Message}", ex.Message);
                throw new ApplicationException("Error retrieving optimized questions feed.", ex);
            }
        }

        public async Task<List<QuestionDataResponse>> GetQuestionBySubjectIdAsync(int subjectId)
        {
            try
            {
                var questions = await _unitOfWork.QuestionRepository.GetBySubjectIdAsync(subjectId);
                if (questions == null || !questions.Any())
                {
                    _logger.LogWarning("No questions found for SubjectId {SubjectId}", subjectId);
                    return new List<QuestionDataResponse>();
                }

                var allLessons = await _unitOfWork.LessonRepository.GetAllAsync();
                var allUsers = await _unitOfWork.UserRepository.GetAllAsync();
                var allChapters = await _unitOfWork.ChapterRepository.GetAllAsync();

                var responses = new List<QuestionDataResponse>();

                foreach (var q in questions)
                {
                    var answers = await _unitOfWork.AnswerRepository.GetByQuestionIdAsync(q.Id);

                    responses.Add(new QuestionDataResponse
                    {
                        Id = q.Id,
                        Content = q.Content,
                        QuestionSource = q.QuestionSource,
                        DifficultyLevelId = q.DifficultyLevelId,
                        LessonId = q.LessonId,
                        ChapterId = q.ChapterId,
                        CreatedByUserId = q.CreatedByUserId,
                        CreatedAt = q.CreatedAt,
                        UpdatedAt = q.UpdatedAt,
                        LessonName = allLessons.FirstOrDefault(l => l.Id == q.LessonId)?.Name ?? string.Empty,
                        ChapterName = allChapters.FirstOrDefault(c => c.Id == q.ChapterId)?.Name ?? string.Empty,
                        CreatedByUserName = allUsers.FirstOrDefault(u => u.Id == q.CreatedByUserId)?.Email ?? string.Empty,
                        Formula = q.Formula,
                        CorrectAnswer = q.CorrectAnswer,
                        Explanation = q.Explanation,
                        Type = q.QuestionType?.ToLower() ?? "multiple-choice",
                        Options = q.Options != null
                            ? JsonSerializer.Deserialize<List<string>>(q.Options) ?? new List<string>()
                            : (answers?.Select(a => a.Content).ToList() ?? new List<string>())
                    });
                }

                return responses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions for SubjectId {SubjectId}: {Message}", subjectId, ex.Message);
                throw new ApplicationException($"Error retrieving questions for subject {subjectId}.", ex);
            }
        }

        public async Task<List<QuestionSimpleResponse>> GetAllQuestionsSimpleAsync(QuestionSearchRequest? searchRequest = null)
        {
            try
            {
                
                var questions = await _unitOfWork.QuestionRepository.GetAllAsync();
                
                if (questions == null)
                {
                    _logger.LogWarning("⚠️ [QuestionsService] Repository returned NULL!");
                    return new List<QuestionSimpleResponse>();
                }

                

                var query = questions.AsQueryable();
                var initialCount = query.Count();

                // Apply search filters if provided
                if (searchRequest != null)
                {
                    

                    if (!string.IsNullOrEmpty(searchRequest.Content))
                    {
                        query = query.Where(q => q.Content.Contains(searchRequest.Content, StringComparison.OrdinalIgnoreCase));
                        
                    }

                    // Lọc theo ID độ khó
                    if (searchRequest.DifficultyLevelId.HasValue)
                    {
                        query = query.Where(q => q.DifficultyLevelId == searchRequest.DifficultyLevelId.Value);
                        
                    }

                    // Lọc theo nhiều khối lớp
                    if (searchRequest.GradeIds != null && searchRequest.GradeIds.Any())
                    {
                        var beforeCount = query.Count();
                        query = query.Where(q => q.GradeId.HasValue && searchRequest.GradeIds.Contains(q.GradeId.Value));
                        
                    }

                    // Lọc theo nhiều môn học (thông qua Lesson → Chapter → Subject)
                    if (searchRequest.SubjectIds != null && searchRequest.SubjectIds.Any())
                    {
                        var beforeCount = query.Count();
                        // Đếm bao nhiêu questions có Lesson và Chapter
                        var questionsWithLessonAndChapter = query.Count(q => q.Lesson != null && q.Lesson.Chapter != null);
                        
                        
                        query = query.Where(q => q.Lesson != null && 
                            q.Lesson.Chapter != null && 
                            searchRequest.SubjectIds.Contains(q.Lesson.Chapter.SubjectId));
                        
                    }

                    // Lọc theo nhiều chương (thông qua Lesson → Chapter)
                    if (searchRequest.ChapterIds != null && searchRequest.ChapterIds.Any())
                    {
                        var beforeCount = query.Count();
                        query = query.Where(q => q.Lesson != null && 
                            searchRequest.ChapterIds.Contains(q.Lesson.ChapterId));
                        
                    }

                    // Lọc theo nhiều bài học
                    if (searchRequest.LessonIds != null && searchRequest.LessonIds.Any())
                    {
                        var beforeCount = query.Count();
                        query = query.Where(q => q.LessonId.HasValue && searchRequest.LessonIds.Contains(q.LessonId.Value));
                        
                    }
                }

                var result = query.Select(q => new QuestionSimpleResponse
                {
                    Id = q.Id,
                    Content = q.Content,
                    DifficultyLevel = q.DifficultyLevel != null ? q.DifficultyLevel.Name : null,
                    GradeId = q.GradeId,
                    GradeName = q.Grade != null ? q.Grade.Name : null,
                    LessonId = q.LessonId,
                    LessonName = q.Lesson != null ? q.Lesson.Name : null
                }).ToList();

                
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [QuestionsService] CRITICAL ERROR retrieving simple questions: {Message}", ex.Message);
                _logger.LogError(ex, "❌ [QuestionsService] Exception Type: {ExceptionType}", ex.GetType().Name);
                _logger.LogError(ex, "❌ [QuestionsService] Stack Trace: {StackTrace}", ex.StackTrace);
                
                // Return empty list instead of throwing to prevent cascading failures
                return new List<QuestionSimpleResponse>();
            }
        }

    }
}
