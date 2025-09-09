using Microsoft.Extensions.Logging;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Dtos;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Helpers;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.GradeService
{
    public class GradeService : IGradeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GradeService> _logger;

        public GradeService(IUnitOfWork unitOfWork, ILogger<GradeService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<GradeDataResponse>> GetAllGradesAsync()
        {
            var grades = await _unitOfWork.GradeRepository.GetAllAsync();
            return grades.Select(g => new GradeDataResponse
            {
                Id = g.Id,
                Name = g.Name,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt
            });
        }

        public async Task<GradeDataResponse> GetGradeByIdAsync(int id)
        {
            var grade = await _unitOfWork.GradeRepository.GetByIdAsync(id);
            if (grade == null)
                throw new NotFoundException($"Grade with ID {id} not found.");

            return new GradeDataResponse
            {
                Id = grade.Id,
                Name = grade.Name,
                CreatedAt = grade.CreatedAt,
                UpdatedAt = grade.UpdatedAt
            };
        }

        public async Task CreateGradeAsync(CreateGradeRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("CreateGradeRequest is null.");
                throw new ArgumentNullException(nameof(request), "Grade creation request cannot be null.");
            }

            var grade = new Grade
            {
                Name = request.Name,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _unitOfWork.GradeRepository.CreateAsync(grade);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("Created grade with ID {GradeId}.", grade.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating grade: {Message}", ex.Message);
                throw new ApplicationException("An error occurred while creating the grade.", ex);
            }
        }

        public async Task UpdateGradeAsync(GradeDataRequest request)
        {
            int decodedId = request.GetDecodedId();   

            var grade = await _unitOfWork.GradeRepository.GetByIdAsync(decodedId);
            if (grade == null)
                throw new NotFoundException("Grade not found");

            grade.Name = request.Name;
            grade.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.GradeRepository.UpdateAsync(grade);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }


        public async Task DeleteGradeAsync(string encodedId)
        {
            int id = IdHelper.DecodeId(encodedId);
            var grade = await _unitOfWork.GradeRepository.GetByIdAsync(id);
            if (grade == null)
                throw new NotFoundException($"Grade with ID {id} not found.");

            await _unitOfWork.GradeRepository.RemoveAsync(grade);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            _logger.LogInformation("Deleted grade with ID {Id}.", grade.Id);
        }
    }
}
