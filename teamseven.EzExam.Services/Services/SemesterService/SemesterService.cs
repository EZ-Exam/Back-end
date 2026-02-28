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

namespace teamseven.EzExam.Services.Services.SemesterService
{
    public class SemesterService : ISemesterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SemesterService> _logger;
        private readonly AutoMapper.IMapper _mapper;

        public SemesterService(IUnitOfWork unitOfWork, ILogger<SemesterService> logger, AutoMapper.IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<SemesterDataResponse>> GetAllSemesterAsync()
        {
            var semesters = await _unitOfWork.SemesterRepository.GetAllAsync();
            var grades = await _unitOfWork.GradeRepository.GetAllAsync();
            var responses = _mapper.Map<IEnumerable<SemesterDataResponse>>(semesters);
            foreach(var response in responses)
            {
                response.GradeName = grades.FirstOrDefault(g => g.Id == response.GradeId)?.Name;
            }
            return responses;
        }

        public async Task<SemesterDataResponse> GetSemesterByIdAsync(int id)
        {
            var semester = await _unitOfWork.SemesterRepository.GetByIdAsync(id);
            if (semester == null)
                throw new NotFoundException($"Semester with ID {id} not found.");

            var grade = await _unitOfWork.GradeRepository.GetByIdAsync(semester.GradeId);

            var response = _mapper.Map<SemesterDataResponse>(semester);
            response.GradeName = grade?.Name;
            return response;
        }

        public async Task<IEnumerable<SemesterDataResponse>> GetSemesterByGradeIdAsync(int gradeId)
        {
            var grade = await _unitOfWork.GradeRepository.GetByIdAsync(gradeId);
            if (grade == null)
            {
                _logger.LogWarning("Grade with ID {GradeId} not found.", gradeId);
                throw new NotFoundException($"Grade with ID {gradeId} not found.");
            }

            var semesters = await _unitOfWork.SemesterRepository.GetAllAsync();
            var filteredSemesters = semesters.Where(s => s.GradeId == gradeId);
            
            var responses = _mapper.Map<IEnumerable<SemesterDataResponse>>(filteredSemesters);
            foreach(var response in responses)
            {
                response.GradeName = grade.Name;
            }
            return responses;
        }

        public async Task CreateSemesterAsync(CreateSemesterRequest request)
        {
            var grade = await _unitOfWork.GradeRepository.GetByIdAsync(request.GradeId);
            if (grade == null)
            {
                _logger.LogWarning("Grade with ID {GradeId} not found.", request.GradeId);
                throw new NotFoundException($"Grade with ID {request.GradeId} not found.");
            }

            var semester = new Semester
            {
                Name = request.Name,
                GradeId = request.GradeId,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _unitOfWork.SemesterRepository.CreateAsync(semester);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                _logger.LogInformation("Created semester with ID {Id}.", semester.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating semester.");
                throw new ApplicationException("An error occurred while creating the semester.", ex);
            }
        }

        public async Task UpdateSemesterAsync(SemesterDataRequest request)
        {
            var semester = await _unitOfWork.SemesterRepository.GetByIdAsync(request.Id);
            if (semester == null)
                throw new NotFoundException("Semester not found");

            semester.Name = request.Name;
            semester.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SemesterRepository.UpdateAsync(semester);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            _logger.LogInformation("Updated semester with ID {Id}.", semester.Id);
        }

        public async Task DeleteSemesterAsync(int id)
        {
            var semester = await _unitOfWork.SemesterRepository.GetByIdAsync(id);
            if (semester == null)
            {
                _logger.LogWarning("Semester with ID {Id} not found for deletion.", id);
                throw new NotFoundException($"Semester with ID {id} not found.");
            }

            try
            {
                await _unitOfWork.SemesterRepository.RemoveAsync(semester);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                _logger.LogInformation("Deleted semester with ID {Id}.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting semester.");
                throw new ApplicationException("An error occurred while deleting the semester.", ex);
            }
        }
    }
}
