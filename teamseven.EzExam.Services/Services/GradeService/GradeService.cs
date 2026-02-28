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
        private readonly AutoMapper.IMapper _mapper;

        public GradeService(IUnitOfWork unitOfWork, ILogger<GradeService> logger, AutoMapper.IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GradeDataResponse>> GetAllGradesAsync()
        {
            var grades = await _unitOfWork.GradeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<GradeDataResponse>>(grades);
        }

        public async Task<GradeDataResponse> GetGradeByIdAsync(int id)
        {
            var grade = await _unitOfWork.GradeRepository.GetByIdAsync(id);
            if (grade == null)
                throw new NotFoundException($"Grade with ID {id} not found.");

            return _mapper.Map<GradeDataResponse>(grade);
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
