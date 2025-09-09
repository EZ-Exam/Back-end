using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;

namespace teamseven.EzExam.Services.Services.QuestionReportService
{
    public class QuestionReportService : IQuestionReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QuestionReportService> _logger;

        public QuestionReportService(IUnitOfWork unitOfWork, ILogger<QuestionReportService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<QuestionReport>> GetAllReportsAsync()
        {
            return await _unitOfWork.QuestionReportRepository.GetAllReportsAsync();
        }

        public async Task<QuestionReport> GetReportByIdAsync(int id)
        {
            var report = await _unitOfWork.QuestionReportRepository.GetReportByIdAsync(id);
            if (report == null)
            {
                _logger.LogWarning("Report with ID {Id} not found.", id);
                throw new KeyNotFoundException($"Report with ID {id} not found.");
            }

            return report;
        }

        public async Task CreateReportAsync(CreateQuestionReportRequest request)
        {
            var question = await _unitOfWork.QuestionRepository.GetByIdAsync(request.QuestionId);
            if (question == null)
            {
                _logger.LogWarning("Question with ID {Id} not found.", request.QuestionId);
                throw new NotFoundException($"Question with ID {request.QuestionId} not found.");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.ReportedByUserId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found.", request.ReportedByUserId);
                throw new NotFoundException($"User with ID {request.ReportedByUserId} not found.");
            }

            var report = new QuestionReport
            {
                QuestionId = request.QuestionId,
                ReportedByUserId = request.ReportedByUserId,
                Reason = request.Reason,
                ReportDate = DateTime.UtcNow
            };

            await _unitOfWork.QuestionReportRepository.CreateReportAsync(report);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            _logger.LogInformation("Created question report with ID {Id}.", report.Id);
        }

        public async Task UpdateReportAsync(UpdateQuestionReportRequest request)
        {
            var existing = await _unitOfWork.QuestionReportRepository.GetReportByIdAsync(request.Id);
            if (existing == null)
            {
                _logger.LogWarning("Report with ID {Id} not found for update.", request.Id);
                throw new KeyNotFoundException($"Report with ID {request.Id} not found.");
            }

            if (!string.IsNullOrWhiteSpace(request.Reason))
            {
                existing.Reason = request.Reason;
            }

            existing.IsHandled = request.IsHandled; 
            existing.ReportDate = DateTime.UtcNow; 

            await _unitOfWork.QuestionReportRepository.UpdateReportAsync(existing);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            _logger.LogInformation("Updated report with ID {Id}", request.Id);
        }

    }
}
