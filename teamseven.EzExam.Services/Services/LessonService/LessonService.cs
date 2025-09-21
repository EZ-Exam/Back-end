using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var lessons = await _unitOfWork.LessonRepository.GetAllAsync();
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

            _logger.LogInformation("Created lesson with ID {Id}.", lesson.Id);
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

            _logger.LogInformation("Updated lesson with ID {Id}.", lesson.Id);
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

            _logger.LogInformation("Deleted lesson with ID {Id}.", lesson.Id);
        }
    }
}