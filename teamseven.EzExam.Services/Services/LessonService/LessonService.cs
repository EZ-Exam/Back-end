using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.LessonService
{
    public class LessonService : ILessonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LessonService> _logger;

        public LessonService(IUnitOfWork unitOfWork, ILogger<LessonService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<LessonDataResponse>> GetAllLessonAsync()
        {
            var lessons = await _unitOfWork.LessonRepository.GetAllAsync() ?? new List<Lesson>();
            return lessons.Select(l => new LessonDataResponse
            {
                Id = l.Id,
                Name = l.Name,
                ChapterId = l.ChapterId,
                Document = l.Document,
                DocumentType = l.DocumentType,
                CreatedAt = l.CreatedAt,
                UpdatedAt = l.UpdatedAt
            });
        }

        public async Task<IEnumerable<LessonDataResponse>> GetLessonsByChapterIdAsync(int chapterId)
        {
            var lessons = await _unitOfWork.LessonRepository.GetByChapterIdAsync(chapterId) ?? new List<Lesson>();
            return lessons.Select(l => new LessonDataResponse
            {
                Id = l.Id,
                Name = l.Name,
                ChapterId = l.ChapterId,
                Document = l.Document,
                DocumentType = l.DocumentType,
                CreatedAt = l.CreatedAt,
                UpdatedAt = l.UpdatedAt
            });
        }

        public async Task<PagedResponse<LessonDataResponse>> GetLessonsAsync(
             int? pageNumber = null,
             int? pageSize = null,
             string? search = null,
             string? sort = null,
             int? chapterId = null,
             int isSort = 0) // Default isSort = 0 (No)
        {
            try
            {
                List<Lesson> lessons;
                int totalItems;

                if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
                {
                    (lessons, totalItems) = await _unitOfWork.LessonRepository.GetPagedAsync(
                        pageNumber.Value,
                        pageSize.Value,
                        search,
                        sort,
                        chapterId,
                        isSort);
                }
                else
                {
                    lessons = await _unitOfWork.LessonRepository.GetAllAsync() ?? new List<Lesson>();
                    if (!string.IsNullOrEmpty(search))
                    {
                        var searchNormalized = search.RemoveDiacritics().ToLower();
                        lessons = lessons.Where(l => l.Name.RemoveDiacritics().ToLower().Contains(searchNormalized)).ToList();
                    }
                    if (chapterId.HasValue)
                    {
                        lessons = lessons.Where(l => l.ChapterId == chapterId.Value).ToList();
                    }
                    if (isSort == 1)
                    {
                        lessons = sort?.ToLower() switch
                        {
                            "name:asc" => lessons.OrderBy(l => l.Name).ToList(),
                            "name:desc" => lessons.OrderByDescending(l => l.Name).ToList(),
                            "createdat:asc" => lessons.OrderBy(l => l.CreatedAt).ToList(),
                            "createdat:desc" => lessons.OrderByDescending(l => l.CreatedAt).ToList(),
                            "updatedat:asc" => lessons.OrderBy(l => l.UpdatedAt).ToList(),
                            "updatedat:desc" => lessons.OrderByDescending(l => l.UpdatedAt).ToList(),
                            _ => lessons.OrderByDescending(l => l.CreatedAt).ToList() // Default when isSort=1
                        };
                    }
                    else
                    {
                        lessons = lessons.OrderBy(l => l.Id).ToList(); // Default when isSort=0
                    }
                    totalItems = lessons.Count;
                }

                var lessonResponses = lessons.Select(l => new LessonDataResponse
                {
                    Id = l.Id,
                    Name = l.Name,
                    ChapterId = l.ChapterId,
                    Document = l.Document,
                    DocumentType = l.DocumentType,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt
                }).ToList();

                return new PagedResponse<LessonDataResponse>(
                    lessonResponses,
                    pageNumber ?? 1,
                    pageSize ?? totalItems,
                    totalItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lessons: {Message}", ex.Message);
                throw new ApplicationException("An error occurred while retrieving lessons.", ex);
            }
        }


        public async Task<LessonDataResponse> GetLessonByIdAsync(int id)
        {
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(id);
            if (lesson == null)
            {
                _logger.LogWarning("Lesson with ID {Id} not found.", id);
                throw new NotFoundException($"Lesson with ID {id} not found.");
            }

            return new LessonDataResponse
            {
                Id = lesson.Id,
                Name = lesson.Name,
                ChapterId = lesson.ChapterId,
                Document = lesson.Document,
                DocumentType = lesson.DocumentType,
                CreatedAt = lesson.CreatedAt,
                UpdatedAt = lesson.UpdatedAt
            };
        }

        public async Task CreateLessonAsync(CreateLessonRequest request)
        {
            var chapter = await _unitOfWork.ChapterRepository.GetByIdAsync(request.ChapterId);
            if (chapter == null)
            {
                _logger.LogWarning("Chapter with ID {Id} not found.", request.ChapterId);
                throw new NotFoundException($"Chapter with ID {request.ChapterId} not found.");
            }

            var lesson = new Lesson
            {
                Name = request.Name,
                ChapterId = request.ChapterId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.LessonRepository.CreateAsync(lesson);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task UpdateLessonAsync(LessonDataRequest request)
        {
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(request.Id);
            if (lesson == null)
                throw new NotFoundException("Lesson not found");

            lesson.Name = request.Name;
            lesson.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.LessonRepository.UpdateAsync(lesson);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task DeleteLessonAsync(int id)
        {
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(id);
            if (lesson == null)
            {
                _logger.LogWarning("Lesson with ID {Id} not found.", id);
                throw new NotFoundException($"Lesson with ID {id} not found.");
            }

            await _unitOfWork.LessonRepository.RemoveAsync(lesson);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        // =================== OPTIMIZED: Lesson feed ===================
        public async Task<PagedResponse<LessonFeedResponse>> GetOptimizedLessonsFeedAsync(
            int currentUserId,
            int page = 1,
            int pageSize = 20,
            string? search = null,
            int? chapterId = null,
            int isSort = 0)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;
            var skip = (page - 1) * pageSize;

            var ctx = _unitOfWork.Context;

            var baseQuery = ctx.Lessons.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                baseQuery = baseQuery.Where(l => l.Name.ToLower().Contains(s) || (l.Document != null && l.Document.ToLower().Contains(s)));
            }
            if (chapterId.HasValue) baseQuery = baseQuery.Where(l => l.ChapterId == chapterId.Value);

            var total = await baseQuery.CountAsync();

            var query = baseQuery.OrderByDescending(l => l.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .Select(l => new LessonFeedResponse
                {
                    Id = l.Id,
                    Name = l.Name,
                    ChapterId = l.ChapterId,
                    ChapterName = l.Chapter != null ? l.Chapter.Name : null,
                    SubjectId = l.Chapter != null ? (int?)l.Chapter.SubjectId : null,
                    SubjectName = l.Chapter != null && l.Chapter.Subject != null ? l.Chapter.Subject.Name : null,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt,

                    QuestionCount = ctx.Questions.Count(q => q.LessonId == l.Id),
                    ExamCount = ctx.Exams.Count(e => e.LessonId == l.Id),
                    AttemptCount = ctx.ExamHistories.Count(h => h.Exam != null && h.Exam.LessonId == l.Id),
                    AverageScore = ctx.ExamHistories.Where(h => h.Exam != null && h.Exam.LessonId == l.Id).Select(h => (decimal?)h.Score).Average() ?? 0m,
                    IsAttemptedByCurrentUser = currentUserId > 0 && ctx.ExamHistories.Any(h => h.Exam != null && h.Exam.LessonId == l.Id && h.UserId == currentUserId)
                })
                .AsNoTracking();

            var items = await query.ToListAsync();
            return new PagedResponse<LessonFeedResponse>(items, page, pageSize, total);
        }

        // =================== OPTIMIZED: Lesson details ===================
        public async Task<LessonDetailOptimizedResponse> GetOptimizedLessonDetailsAsync(int lessonId, int currentUserId = 0)
        {
            var ctx = _unitOfWork.Context;

            var lesson = await ctx.Lessons.AsNoTracking()
                .Where(l => l.Id == lessonId)
                .Select(l => new
                {
                    l.Id,
                    l.Name,
                    l.ChapterId,
                    ChapterName = l.Chapter != null ? l.Chapter.Name : null,
                    SubjectId = l.Chapter != null ? (int?)l.Chapter.SubjectId : null,
                    SubjectName = l.Chapter != null && l.Chapter.Subject != null ? l.Chapter.Subject.Name : null,
                    l.CreatedAt,
                    l.UpdatedAt
                }).FirstOrDefaultAsync();

            if (lesson == null) throw new ArgumentException("Lesson not found");

            var questionIds = await ctx.Questions.AsNoTracking().Where(q => q.LessonId == lessonId).Select(q => q.Id).ToListAsync();
            var examIds = await ctx.Exams.AsNoTracking().Where(e => e.LessonId == lessonId).Select(e => e.Id).ToListAsync();

            var attemptCount = await ctx.ExamHistories.CountAsync(h => h.Exam != null && h.Exam.LessonId == lessonId);
            var avgScore = await ctx.ExamHistories.Where(h => h.Exam != null && h.Exam.LessonId == lessonId).Select(h => (decimal?)h.Score).AverageAsync() ?? 0m;
            var isAttempted = currentUserId > 0 && await ctx.ExamHistories.AnyAsync(h => h.Exam != null && h.Exam.LessonId == lessonId && h.UserId == currentUserId);

            return new LessonDetailOptimizedResponse
            {
                Id = lesson.Id,
                Name = lesson.Name,
                ChapterId = lesson.ChapterId,
                SubjectId = lesson.SubjectId,
                ChapterName = lesson.ChapterName,
                SubjectName = lesson.SubjectName,
                CreatedAt = lesson.CreatedAt,
                UpdatedAt = lesson.UpdatedAt,
                QuestionIds = questionIds,
                ExamIds = examIds,
                QuestionCount = questionIds.Count,
                ExamCount = examIds.Count,
                AttemptCount = attemptCount,
                AverageScore = avgScore,
                IsAttemptedByCurrentUser = isAttempted
            };
        }
    }
}