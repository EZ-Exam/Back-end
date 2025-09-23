using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class UserQuestionAttemptRepository : GenericRepository<UserQuestionAttempt>
    {
        public UserQuestionAttemptRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        public async Task<List<UserQuestionAttempt>> GetByUserIdAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId);
        }

        public async Task<List<UserQuestionAttempt>> GetByQuestionIdAsync(int questionId)
        {
            return await GetAllAsync(x => x.QuestionId == questionId);
        }

        public async Task<List<UserQuestionAttempt>> GetByUserAndQuestionAsync(int userId, int questionId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.QuestionId == questionId);
        }

        public async Task<List<UserQuestionAttempt>> GetBySessionIdAsync(int sessionId)
        {
            return await GetAllAsync(x => x.SessionId == sessionId);
        }

        public async Task<List<UserQuestionAttempt>> GetByExamIdAsync(int examId)
        {
            return await GetAllAsync(x => x.ExamId == examId);
        }

        public async Task<List<UserQuestionAttempt>> GetBySubjectAsync(int userId, int subjectId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.SubjectId == subjectId);
        }

        public async Task<List<UserQuestionAttempt>> GetByDifficultyAsync(int userId, string difficultyLevel)
        {
            return await GetAllAsync(x => x.UserId == userId && x.DifficultyLevel == difficultyLevel);
        }

        public async Task<List<UserQuestionAttempt>> GetByAttemptTypeAsync(int userId, string attemptType)
        {
            return await GetAllAsync(x => x.UserId == userId && x.AttemptType == attemptType);
        }

        public async Task<List<UserQuestionAttempt>> GetCorrectAttemptsAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.IsCorrect == true);
        }

        public async Task<List<UserQuestionAttempt>> GetIncorrectAttemptsAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.IsCorrect == false);
        }

        public async Task<List<UserQuestionAttempt>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await GetAllAsync(x => x.UserId == userId && x.CreatedAt >= startDate && x.CreatedAt <= endDate);
        }

        public async Task<decimal> GetAccuracyRateAsync(int userId, int? subjectId = null, string? difficultyLevel = null)
        {
            var attempts = await GetAllAsync(x => x.UserId == userId && 
                (subjectId == null || x.SubjectId == subjectId) && 
                (difficultyLevel == null || x.DifficultyLevel == difficultyLevel));
            
            if (!attempts.Any()) return 0;
            
            var correctCount = attempts.Count(x => x.IsCorrect);
            return (decimal)correctCount / attempts.Count * 100;
        }

        public async Task<int> GetTotalAttemptsCountAsync(int userId, int? subjectId = null)
        {
            var attempts = await GetAllAsync(x => x.UserId == userId && (subjectId == null || x.SubjectId == subjectId));
            return attempts.Count;
        }

        public async Task<decimal> GetAverageTimePerQuestionAsync(int userId, int? subjectId = null)
        {
            var attempts = await GetAllAsync(x => x.UserId == userId && 
                (subjectId == null || x.SubjectId == subjectId) && 
                x.TimeSpent > 0);
            
            return (decimal)(attempts.Any() ? attempts.Average(x => x.TimeSpent) : 0);
        }
    }
}
