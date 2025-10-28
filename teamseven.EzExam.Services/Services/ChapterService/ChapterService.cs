using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.ChapterService
{
    public class ChapterService : IChapterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChapterService> _logger;

        public ChapterService(IUnitOfWork unitOfWork, ILogger<ChapterService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<ChapterDataResponse>> GetAllChaptersAsync()
        {
            var chapters = await _unitOfWork.ChapterRepository.GetAllAsync();
            return chapters.Select(c => new ChapterDataResponse
            {
                Id = c.Id,
                Name = c.Name,
                SemesterId = c.SemesterId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task<ChapterDataResponse> GetChapterByIdAsync(int id)
        {
            var chapter = await _unitOfWork.ChapterRepository.GetByIdAsync(id);
            if (chapter == null)
                throw new NotFoundException($"Chapter with ID {id} not found.");

            return new ChapterDataResponse
            {
                Id = chapter.Id,
                Name = chapter.Name,
                SemesterId = chapter.SemesterId,
                CreatedAt = chapter.CreatedAt,
                UpdatedAt = chapter.UpdatedAt
            };
        }

        public async Task<IEnumerable<ChapterDataResponse>> GetChaptersBySemesterIdAsync(int semesterId)
        {
            var chapters = await _unitOfWork.ChapterRepository.GetBySemesterIdAsync(semesterId) ?? new List<Chapter>();
            return chapters.Select(c => new ChapterDataResponse
            {
                Id = c.Id,
                Name = c.Name,
                SemesterId = c.SemesterId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task<IEnumerable<ChapterDataResponse>> GetChaptersBySemesterAndSubjectAsync(int semesterId, int subjectId)
        {
            var chapters = await _unitOfWork.ChapterRepository.GetBySemesterAndSubjectAsync(semesterId, subjectId)
                            ?? new List<Chapter>();

            return chapters.Select(c => new ChapterDataResponse
            {
                Id = c.Id,
                Name = c.Name,
                SemesterId = c.SemesterId,
                SubjectId = c.SubjectId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }


        public async Task CreateChapterAsync(CreateChapterRequest request)
        {
            var semester = await _unitOfWork.SemesterRepository.GetByIdAsync(request.SemesterId);
            if (semester == null)
            {
                _logger.LogWarning("Semester with ID {SemesterId} not found.", request.SemesterId);
                throw new NotFoundException($"Semester with ID {request.SemesterId} not found.");
            }

            var chapter = new Chapter
            {
                Name = request.Name,
                SemesterId = request.SemesterId,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _unitOfWork.ChapterRepository.CreateAsync(chapter);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                _logger.LogInformation("Created chapter with ID {Id}.", chapter.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating chapter.");
                throw new ApplicationException("An error occurred while creating the chapter.", ex);
            }
        }

        public async Task UpdateChapterAsync(ChapterDataRequest request)
        {
            var chapter = await _unitOfWork.ChapterRepository.GetByIdAsync(request.Id);
            if (chapter == null)
                throw new NotFoundException("Chapter not found");

            chapter.Name = request.Name;
            //chapter.SemesterId = request.SemesterId;
            chapter.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.ChapterRepository.UpdateAsync(chapter);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            _logger.LogInformation("Updated chapter with ID {Id}.", chapter.Id);
        }

        public async Task DeleteChapterAsync(int id)
        {
            var chapter = await _unitOfWork.ChapterRepository.GetByIdAsync(id);
            if (chapter == null)
            {
                _logger.LogWarning("Chapter with ID {Id} not found for deletion.", id);
                throw new NotFoundException($"Chapter with ID {id} not found.");
            }

            try
            {
                await _unitOfWork.ChapterRepository.RemoveAsync(chapter);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                _logger.LogInformation("Deleted chapter with ID {Id}.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting chapter.");
                throw new ApplicationException("An error occurred while deleting the chapter.", ex);
            }
        }
    }
}
