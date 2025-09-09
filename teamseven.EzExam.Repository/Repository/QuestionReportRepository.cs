using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository.Context;

namespace teamseven.EzExam.Repository.Repository
{
    public class QuestionReportRepository : GenericRepository<QuestionReport>
    {
        private readonly teamsevenezexamdbContext _context;

        public QuestionReportRepository(teamsevenezexamdbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<QuestionReport>> GetAllReportsAsync()
        {
            return await _context.QuestionReports.ToListAsync();
        }

        public async Task<QuestionReport?> GetReportByIdAsync(int id)
        {
            return await _context.QuestionReports.FindAsync(id);
        }

        public async Task CreateReportAsync(QuestionReport report)
        {
            _context.QuestionReports.Add(report);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReportAsync(QuestionReport report)
        {
            _context.QuestionReports.Update(report);
            await _context.SaveChangesAsync();
        }

    }
}
