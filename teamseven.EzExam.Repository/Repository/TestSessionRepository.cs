using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class TestSessionRepository : GenericRepository<TestSession>
    {
        public TestSessionRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        public async Task<List<TestSession>> GetByUserIdAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId);
        }

        public async Task<List<TestSession>> GetByExamIdAsync(int examId)
        {
            return await GetAllAsync(x => x.ExamId == examId);
        }

        public async Task<List<TestSession>> GetActiveSessionsAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.SessionStatus == "IN_PROGRESS");
        }

        public async Task<List<TestSession>> GetCompletedSessionsAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.SessionStatus == "COMPLETED");
        }

        public async Task<TestSession?> GetActiveSessionByExamAsync(int userId, int examId)
        {
            return await GetFirstOrDefaultAsync(x => x.UserId == userId && x.ExamId == examId && x.SessionStatus == "IN_PROGRESS");
        }

        public async Task<List<TestSession>> GetSessionsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await GetAllAsync(x => x.UserId == userId && x.CreatedAt >= startDate && x.CreatedAt <= endDate);
        }

        public async Task<List<TestSession>> GetPassedSessionsAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.IsPassed == true);
        }

        public async Task<List<TestSession>> GetFailedSessionsAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.IsPassed == false);
        }

        public async Task<decimal> GetAverageScoreByUserIdAsync(int userId)
        {
            var sessions = await GetAllAsync(x => x.UserId == userId && x.TotalScore.HasValue);
            return sessions.Any() ? sessions.Average(x => x.TotalScore!.Value) : 0;
        }

        public async Task<int> GetTotalSessionsCountAsync(int userId)
        {
            var sessions = await GetAllAsync(x => x.UserId == userId);
            return sessions.Count;
        }
    }
}
