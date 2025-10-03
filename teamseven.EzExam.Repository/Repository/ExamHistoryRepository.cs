using Microsoft.EntityFrameworkCore;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public interface IExamHistoryRepository : IGenericRepository<ExamHistory>
    {
        Task<IEnumerable<ExamHistory>> GetHistoryByUserIdAsync(string userId);
        Task<IEnumerable<ExamHistory>> GetHistoryByExamIdAsync(string examId);
        Task<ExamHistory?> GetLatestHistoryByUserAsync(string userId);
        Task<decimal> GetAverageScoreByUserAsync(string userId);
        Task<int> GetTotalExamsCompletedByUserAsync(string userId);
    }

    public class ExamHistoryRepository : GenericRepository<ExamHistory>, IExamHistoryRepository
    {
        public ExamHistoryRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ExamHistory>> GetHistoryByUserIdAsync(string userId)
        {
            return await _context.ExamHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.SubmittedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamHistory>> GetHistoryByExamIdAsync(string examId)
        {
            return await _context.ExamHistories
                .Where(h => h.ExamId == examId)
                .OrderByDescending(h => h.SubmittedAt)
                .ToListAsync();
        }

        public async Task<ExamHistory?> GetLatestHistoryByUserAsync(string userId)
        {
            return await _context.ExamHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.SubmittedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<decimal> GetAverageScoreByUserAsync(string userId)
        {
            var histories = await _context.ExamHistories
                .Where(h => h.UserId == userId)
                .ToListAsync();

            return histories.Any() ? histories.Average(h => h.Score) : 0;
        }

        public async Task<int> GetTotalExamsCompletedByUserAsync(string userId)
        {
            return await _context.ExamHistories
                .CountAsync(h => h.UserId == userId);
        }
    }
}
