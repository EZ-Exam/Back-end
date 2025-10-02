using Microsoft.EntityFrameworkCore;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public interface IStudentQuizHistoryRepository : IGenericRepository<StudentQuizHistory>
    {
        Task<IEnumerable<StudentQuizHistory>> GetHistoryByUserIdAsync(int userId);
        Task<IEnumerable<StudentQuizHistory>> GetRecentHistoryByUserIdAsync(int userId, int count = 5);
        Task<IEnumerable<StudentQuizHistory>> GetHistoryByUserAndSubjectAsync(int userId, int subjectId);
        Task<StudentQuizHistory?> GetLatestHistoryByUserAsync(int userId);
        Task<IEnumerable<StudentQuizHistory>> GetHistoryByExamIdAsync(int examId);
        Task<decimal> GetAverageScoreByUserAsync(int userId, int? subjectId = null);
        Task<int> GetTotalQuizzesCompletedByUserAsync(int userId, int? subjectId = null);
    }

    public class StudentQuizHistoryRepository : GenericRepository<StudentQuizHistory>, IStudentQuizHistoryRepository
    {
        public StudentQuizHistoryRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<StudentQuizHistory>> GetHistoryByUserIdAsync(int userId)
        {
            return await _context.StudentQuizHistories
                .Where(h => h.UserId == userId)
                .Include(h => h.Exam)
                .Include(h => h.TestSession)
                .OrderByDescending(h => h.CompletedAt ?? h.StartedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentQuizHistory>> GetRecentHistoryByUserIdAsync(int userId, int count = 5)
        {
            return await _context.StudentQuizHistories
                .Where(h => h.UserId == userId && h.CompletedAt != null)
                .Include(h => h.Exam)
                    .ThenInclude(e => e.Subject)
                .Include(h => h.TestSession)
                .OrderByDescending(h => h.CompletedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentQuizHistory>> GetHistoryByUserAndSubjectAsync(int userId, int subjectId)
        {
            return await _context.StudentQuizHistories
                .Where(h => h.UserId == userId)
                .Include(h => h.Exam)
                .Where(h => h.Exam.SubjectId == subjectId)
                .Include(h => h.TestSession)
                .OrderByDescending(h => h.CompletedAt ?? h.StartedAt)
                .ToListAsync();
        }

        public async Task<StudentQuizHistory?> GetLatestHistoryByUserAsync(int userId)
        {
            return await _context.StudentQuizHistories
                .Where(h => h.UserId == userId && h.CompletedAt != null)
                .Include(h => h.Exam)
                    .ThenInclude(e => e.Subject)
                .Include(h => h.TestSession)
                .OrderByDescending(h => h.CompletedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<StudentQuizHistory>> GetHistoryByExamIdAsync(int examId)
        {
            return await _context.StudentQuizHistories
                .Where(h => h.ExamId == examId)
                .Include(h => h.User)
                .Include(h => h.TestSession)
                .OrderByDescending(h => h.CompletedAt ?? h.StartedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetAverageScoreByUserAsync(int userId, int? subjectId = null)
        {
            var query = _context.StudentQuizHistories
                .Where(h => h.UserId == userId && h.CompletedAt != null);

            if (subjectId.HasValue)
            {
                query = query.Include(h => h.Exam)
                    .Where(h => h.Exam.SubjectId == subjectId.Value);
            }

            var scores = await query.Select(h => h.TotalScore).ToListAsync();
            return scores.Any() ? scores.Average() : 0;
        }

        public async Task<int> GetTotalQuizzesCompletedByUserAsync(int userId, int? subjectId = null)
        {
            var query = _context.StudentQuizHistories
                .Where(h => h.UserId == userId && h.CompletedAt != null);

            if (subjectId.HasValue)
            {
                query = query.Include(h => h.Exam)
                    .Where(h => h.Exam.SubjectId == subjectId.Value);
            }

            return await query.CountAsync();
        }
    }
}
