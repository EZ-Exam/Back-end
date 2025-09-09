using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository.Context;

namespace teamseven.EzExam.Repository.Repository
{
    public class SolutionReportRepository : GenericRepository<SolutionReport>
    {
        private readonly teamsevenezexamdbContext _context;

        public SolutionReportRepository(teamsevenezexamdbContext context)
        {
            _context = context;
        }

        public async Task<List<SolutionReport>?> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<SolutionReport?> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<List<SolutionReport>?> GetBySolutionIdAsync(int solutionId)
        {
            return await _context.SolutionReports
                .Where(r => r.SolutionId == solutionId)
                .ToListAsync();
        }

        public async Task<List<SolutionReport>?> GetByReporterAsync(long userId)
        {
            return await _context.SolutionReports
                .Where(r => r.ReportedByUserId == userId)
                .ToListAsync();
        }

        public async Task<List<SolutionReport>?> GetByStatusAsync(string status)
        {
            return await _context.SolutionReports
                .Where(r => r.Status == status)
                .ToListAsync();
        }

        public async Task<int> AddAsync(SolutionReport report)
        {
            return await CreateAsync(report);
        }

        public async Task<int> UpdateAsync(SolutionReport report)
        {
            return await base.UpdateAsync(report);
        }

        public async Task<bool> DeleteAsync(SolutionReport report)
        {
            return await RemoveAsync(report);
        }
    }
}
