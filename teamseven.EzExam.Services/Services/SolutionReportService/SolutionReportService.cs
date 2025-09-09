using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Dtos;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Services.Services.SolutionReportService
{
    public class SolutionReportService : ISolutionReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SolutionReportService> _logger;

        public SolutionReportService(IUnitOfWork unitOfWork, ILogger<SolutionReportService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task AddAsync(SolutionReportRequest request)
        {
            var solution = await _unitOfWork.SolutionRepository.GetByIdAsync(request.SolutionId);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.ReportedByUserId);

            if (solution == null)
                throw new KeyNotFoundException($"Solution {request.SolutionId} not found.");
            if (user == null)
                throw new KeyNotFoundException($"User {request.ReportedByUserId} not found.");

            var report = new SolutionReport
            {
                SolutionId = request.SolutionId,
                ReportedByUserId = request.ReportedByUserId,
                Reason = request.Reason,
                Status = "Pending",
                ReportDate = DateTime.UtcNow
            };

            await _unitOfWork.SolutionReportRepository.CreateAsync(report);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task<SolutionReportResponse> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.SolutionReportRepository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Report ID {id} not found.");

            return new SolutionReportResponse
            {
                Id = entity.Id,
                SolutionId = entity.SolutionId,
                ReportedByUserId = entity.ReportedByUserId,
                Reason = entity.Reason,
                Status = entity.Status,
                ReportDate = entity.ReportDate,
                ReporterEmail = entity.ReportedByUser?.Email,
                ReporterFullName = entity.ReportedByUser?.FullName
            };
        }

        public async Task<List<SolutionReportResponse>> GetBySolutionIdAsync(int solutionId)
        {
            var list = await _unitOfWork.SolutionReportRepository.GetBySolutionIdAsync(solutionId);
            return list.Select(r => new SolutionReportResponse
            {
                Id = r.Id,
                SolutionId = r.SolutionId,
                ReportedByUserId = r.ReportedByUserId,
                Reason = r.Reason,
                Status = r.Status,
                ReportDate = r.ReportDate,
                ReporterEmail = r.ReportedByUser?.Email,
                ReporterFullName = r.ReportedByUser?.FullName
            }).ToList();
        }

        public async Task UpdateStatusAsync(int id, string newStatus)
        {
            var entity = await _unitOfWork.SolutionReportRepository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException($"Report ID {id} not found.");

            entity.Status = newStatus;
            await _unitOfWork.SolutionReportRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.SolutionReportRepository.GetByIdAsync(id);
            if (entity != null)
            {
                await _unitOfWork.SolutionReportRepository.RemoveAsync(entity);
                await _unitOfWork.SaveChangesWithTransactionAsync();
            }
        }
    }
}
