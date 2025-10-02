using Microsoft.EntityFrameworkCore;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public interface IStudentPerformanceSummaryRepository : IGenericRepository<StudentPerformanceSummary>
    {
        Task<StudentPerformanceSummary?> GetByUserIdAsync(int userId, int? subjectId = null);
        Task<IEnumerable<StudentPerformanceSummary>> GetAllByUserIdAsync(int userId);
        Task<StudentPerformanceSummary?> GetOrCreateAsync(int userId, int? subjectId = null, int? gradeId = null);
        Task<IEnumerable<StudentPerformanceSummary>> GetBySubjectIdAsync(int subjectId);
        Task<IEnumerable<StudentPerformanceSummary>> GetTopPerformersAsync(int? subjectId = null, int count = 10);
        Task<IEnumerable<StudentPerformanceSummary>> GetNeedingImprovementAsync(int? subjectId = null, decimal threshold = 60.0m);
    }

    public class StudentPerformanceSummaryRepository : GenericRepository<StudentPerformanceSummary>, IStudentPerformanceSummaryRepository
    {
        public StudentPerformanceSummaryRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        public async Task<StudentPerformanceSummary?> GetByUserIdAsync(int userId, int? subjectId = null)
        {
            var query = _context.StudentPerformanceSummaries
                .Where(s => s.UserId == userId);

            if (subjectId.HasValue)
            {
                query = query.Where(s => s.SubjectId == subjectId.Value);
            }
            else
            {
                query = query.Where(s => s.SubjectId == null);
            }

            return await query
                .Include(s => s.User)
                .Include(s => s.Subject)
                .Include(s => s.Grade)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<StudentPerformanceSummary>> GetAllByUserIdAsync(int userId)
        {
            return await _context.StudentPerformanceSummaries
                .Where(s => s.UserId == userId)
                .Include(s => s.User)
                .Include(s => s.Subject)
                .Include(s => s.Grade)
                .OrderByDescending(s => s.AverageScore)
                .ToListAsync();
        }

        public async Task<StudentPerformanceSummary?> GetOrCreateAsync(int userId, int? subjectId = null, int? gradeId = null)
        {
            var existing = await GetByUserIdAsync(userId, subjectId);
            if (existing != null)
            {
                return existing;
            }

            var newSummary = new StudentPerformanceSummary
            {
                UserId = userId,
                SubjectId = subjectId,
                GradeId = gradeId,
                TotalQuizzesCompleted = 0,
                RecentQuizzesCount = 5,
                AverageScore = 0.0m,
                AverageTimePerQuiz = 0.0m,
                AverageTimePerQuestion = 0.0m,
                OverallAccuracy = 0.0m,
                LearningVelocity = 0.0m,
                ConsistencyScore = 0.0m,
                ConfidenceLevel = 0.0m,
                TimeManagementScore = 0.0m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.StudentPerformanceSummaries.AddAsync(newSummary);
            await _context.SaveChangesAsync();
            return newSummary;
        }

        public async Task<IEnumerable<StudentPerformanceSummary>> GetBySubjectIdAsync(int subjectId)
        {
            return await _context.StudentPerformanceSummaries
                .Where(s => s.SubjectId == subjectId)
                .Include(s => s.User)
                .Include(s => s.Subject)
                .Include(s => s.Grade)
                .OrderByDescending(s => s.AverageScore)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentPerformanceSummary>> GetTopPerformersAsync(int? subjectId = null, int count = 10)
        {
            var query = _context.StudentPerformanceSummaries.AsQueryable();

            if (subjectId.HasValue)
            {
                query = query.Where(s => s.SubjectId == subjectId.Value);
            }

            return await query
                .Where(s => s.TotalQuizzesCompleted >= 3) // At least 3 quizzes completed
                .Include(s => s.User)
                .Include(s => s.Subject)
                .Include(s => s.Grade)
                .OrderByDescending(s => s.AverageScore)
                .ThenByDescending(s => s.ConsistencyScore)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentPerformanceSummary>> GetNeedingImprovementAsync(int? subjectId = null, decimal threshold = 60.0m)
        {
            var query = _context.StudentPerformanceSummaries.AsQueryable();

            if (subjectId.HasValue)
            {
                query = query.Where(s => s.SubjectId == subjectId.Value);
            }

            return await query
                .Where(s => s.AverageScore < threshold && s.TotalQuizzesCompleted >= 2)
                .Include(s => s.User)
                .Include(s => s.Subject)
                .Include(s => s.Grade)
                .OrderBy(s => s.AverageScore)
                .ThenBy(s => s.ImprovementTrend)
                .ToListAsync();
        }
    }
}
