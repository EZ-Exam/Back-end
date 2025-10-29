using Microsoft.EntityFrameworkCore;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public interface IExamHistoryRepository : IGenericRepository<ExamHistory>
    {
        Task<IEnumerable<ExamHistory>> GetHistoryByUserIdAsync(int userId);
        Task<IEnumerable<ExamHistory>> GetHistoryByUserIdWithExamAsync(int userId);
        Task<IEnumerable<ExamHistory>> GetHistoryByExamIdAsync(int examId);
        Task<ExamHistory?> GetLatestHistoryByUserAsync(int userId);
        Task<decimal> GetAverageScoreByUserAsync(int userId);
        Task<int> GetTotalExamsCompletedByUserAsync(int userId);
    }

    public class ExamHistoryRepository : GenericRepository<ExamHistory>, IExamHistoryRepository
    {
        public ExamHistoryRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ExamHistory>> GetHistoryByUserIdWithExamAsync(int userId)
        {
            return await _context.ExamHistories
                .Include(eh => eh.Exam)
                    .ThenInclude(e => e.Subject)
                .Include(eh => eh.Exam)
                    .ThenInclude(e => e.Grade)
                .Include(eh => eh.Exam)
                    .ThenInclude(e => e.Lesson)
                        .ThenInclude(l => l.Chapter)
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.SubmittedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamHistory>> GetHistoryByUserIdAsync(int userId)
        {
            return await _context.ExamHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.SubmittedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamHistory>> GetHistoryByExamIdAsync(int examId)
        {
            return await _context.ExamHistories
                .Where(h => h.ExamId == examId)
                .OrderByDescending(h => h.SubmittedAt)
                .ToListAsync();
        }

        public async Task<ExamHistory?> GetLatestHistoryByUserAsync(int userId)
        {
            return await _context.ExamHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.SubmittedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<decimal> GetAverageScoreByUserAsync(int userId)
        {
            var histories = await _context.ExamHistories
                .Where(h => h.UserId == userId)
                .ToListAsync();

            return histories.Any() ? histories.Average(h => h.Score) : 0;
        }

        public async Task<int> GetTotalExamsCompletedByUserAsync(int userId)
        {
            return await _context.ExamHistories
                .CountAsync(h => h.UserId == userId);
        }
    }
}
