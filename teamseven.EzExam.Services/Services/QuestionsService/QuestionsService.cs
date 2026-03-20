using AutoMapper;
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
        private readonly IMapper _mapper;

        public QuestionsService(IUnitOfWork unitOfWork, ILogger<QuestionsService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

            var question = _mapper.Map<Question>(questionDataRequest);
            question.QuestionType = string.IsNullOrWhiteSpace(questionDataRequest.QuestionType) ? "MULTIPLE_CHOICE" : questionDataRequest.QuestionType!;
            question.Options = questionDataRequest.Options != null ? JsonSerializer.Serialize(questionDataRequest.Options) : null;
            question.CreatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.QuestionRepository.CreateAsync(question);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                var response = _mapper.Map<QuestionDataResponse>(question);
                response.Options = question.Options != null
                    ? JsonSerializer.Deserialize<List<string>>(question.Options) ?? new List<string>()
                    : (questionDataRequest.Options ?? new List<string>());
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

            var answers = await _unitOfWork.AnswerRepository.GetByQuestionIdAsync(id);

            var response = _mapper.Map<QuestionDataResponse>(question);
            response.Options = question.Options != null
                ? JsonSerializer.Deserialize<List<string>>(question.Options) ?? new List<string>()
                : (answers?.Select(a => a.Content).ToList() ?? new List<string>());
            return response;
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

                var response = _mapper.Map<QuestionDataResponse>(existingQuestion);
                response.Options = existingQuestion.Options != null
                    ? JsonSerializer.Deserialize<List<string>>(existingQuestion.Options) ?? new List<string>()
                    : (questionDataRequest.Options ?? new List<string>());
                return response;
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
    int? textbookId = null)
        {
            try
            {
                List<Question> questions;
                int totalItems;

                if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
                {
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
                        textbookId);
                }
                else
                {
                    questions = await _unitOfWork.QuestionRepository.GetAllAsync() ?? new List<Question>();

                    if (createdByUserId.HasValue)
                    {
                        questions = questions.Where(q => q.CreatedByUserId == createdByUserId.Value).ToList();
                    }

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
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;
            var skip = (page - 1) * pageSize;

            try
            {
                var ctx = _unitOfWork.Context;

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

                        AnswerCount = ctx.Answers.Count(a => a.QuestionId == q.Id),
                        CommentCount = ctx.QuestionComments.Count(c => c.QuestionId == q.Id && (c.IsDeleted == false || c.IsDeleted == null)),

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
                    _logger.LogWarning("️ [QuestionsService] Repository returned NULL!");
                    return new List<QuestionSimpleResponse>();
                }

                

                var query = questions.AsQueryable();
                var initialCount = query.Count();

                if (searchRequest != null)
                {
                    

                    if (!string.IsNullOrEmpty(searchRequest.Content))
                    {
                        query = query.Where(q => q.Content.Contains(searchRequest.Content, StringComparison.OrdinalIgnoreCase));
                        
                    }

                    if (searchRequest.DifficultyLevelId.HasValue)
                    {
                        query = query.Where(q => q.DifficultyLevelId == searchRequest.DifficultyLevelId.Value);
                        
                    }

                    if (searchRequest.GradeIds != null && searchRequest.GradeIds.Any())
                    {
                        var beforeCount = query.Count();
                        query = query.Where(q => q.GradeId.HasValue && searchRequest.GradeIds.Contains(q.GradeId.Value));
                        
                    }

                    if (searchRequest.SubjectIds != null && searchRequest.SubjectIds.Any())
                    {
                        var beforeCount = query.Count();
                        var questionsWithLessonAndChapter = query.Count(q => q.Lesson != null && q.Lesson.Chapter != null);
                        
                        
                        query = query.Where(q => q.Lesson != null && 
                            q.Lesson.Chapter != null && 
                            searchRequest.SubjectIds.Contains(q.Lesson.Chapter.SubjectId));
                        
                    }

                    if (searchRequest.ChapterIds != null && searchRequest.ChapterIds.Any())
                    {
                        var beforeCount = query.Count();
                        query = query.Where(q => q.Lesson != null && 
                            searchRequest.ChapterIds.Contains(q.Lesson.ChapterId));
                        
                    }

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
                _logger.LogError(ex, " [QuestionsService] CRITICAL ERROR retrieving simple questions: {Message}", ex.Message);
                _logger.LogError(ex, " [QuestionsService] Exception Type: {ExceptionType}", ex.GetType().Name);
                _logger.LogError(ex, " [QuestionsService] Stack Trace: {StackTrace}", ex.StackTrace);
                
                return new List<QuestionSimpleResponse>();
            }
        }

    }
}
