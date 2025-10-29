using Microsoft.EntityFrameworkCore;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public interface IStudentQuestionAttemptRepository : IGenericRepository<StudentQuestionAttempt>
    {
        Task<IEnumerable<StudentQuestionAttempt>> GetByQuizHistoryIdAsync(int quizHistoryId);
        Task<IEnumerable<StudentQuestionAttempt>> GetByUserIdAsync(int userId);
        Task<IEnumerable<StudentQuestionAttempt>> GetByQuestionIdAsync(int questionId);
        Task<IEnumerable<StudentQuestionAttempt>> GetByUserAndQuestionAsync(int userId, int questionId);
        Task<IEnumerable<StudentQuestionAttempt>> GetCorrectAttemptsByUserAsync(int userId);
        Task<IEnumerable<StudentQuestionAttempt>> GetIncorrectAttemptsByUserAsync(int userId);
        Task<IEnumerable<StudentQuestionAttempt>> GetByDifficultyLevelAsync(int userId, string difficultyLevel);
        Task<IEnumerable<StudentQuestionAttempt>> GetByTopicAsync(int userId, string topic);
        Task<decimal> GetAccuracyByUserAsync(int userId);
        Task<decimal> GetAccuracyByUserAndDifficultyAsync(int userId, string difficultyLevel);
        Task<decimal> GetAccuracyByUserAndTopicAsync(int userId, string topic);
    }

    public class StudentQuestionAttemptRepository : GenericRepository<StudentQuestionAttempt>, IStudentQuestionAttemptRepository
    {
        public StudentQuestionAttemptRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<StudentQuestionAttempt>> GetByQuizHistoryIdAsync(int quizHistoryId)
        {
            return await _context.StudentQuestionAttempts
                .Where(a => a.StudentQuizHistoryId == quizHistoryId)
                .Include(a => a.Question)
                .Include(a => a.SelectedAnswer)
                .Include(a => a.Chapter)
                .Include(a => a.Lesson)
                .OrderBy(a => a.QuestionOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentQuestionAttempt>> GetByUserIdAsync(int userId)
        {
            return await _context.StudentQuestionAttempts
                .Where(a => a.UserId == userId)
                .Include(a => a.Question)
                .Include(a => a.SelectedAnswer)
                .Include(a => a.StudentQuizHistory)
                    .ThenInclude(h => h.Exam)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentQuestionAttempt>> GetByQuestionIdAsync(int questionId)
        {
            return await _context.StudentQuestionAttempts
                .Where(a => a.QuestionId == questionId)
                .Include(a => a.User)
                .Include(a => a.SelectedAnswer)
                .Include(a => a.StudentQuizHistory)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentQuestionAttempt>> GetByUserAndQuestionAsync(int userId, int questionId)
        {
            return await _context.StudentQuestionAttempts
                .Where(a => a.UserId == userId && a.QuestionId == questionId)
                .Include(a => a.Question)
                .Include(a => a.SelectedAnswer)
                .Include(a => a.StudentQuizHistory)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentQuestionAttempt>> GetCorrectAttemptsByUserAsync(int userId)
        {
            return await _context.StudentQuestionAttempts
                .Where(a => a.UserId == userId && a.IsCorrect)
                .Include(a => a.Question)
                .Include(a => a.SelectedAnswer)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentQuestionAttempt>> GetIncorrectAttemptsByUserAsync(int userId)
        {
            return await _context.StudentQuestionAttempts
                .Where(a => a.UserId == userId && !a.IsCorrect)
                .Include(a => a.Question)
                .Include(a => a.SelectedAnswer)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentQuestionAttempt>> GetByDifficultyLevelAsync(int userId, string difficultyLevel)
        {
            return await _context.StudentQuestionAttempts
                .Where(a => a.UserId == userId && a.DifficultyLevel == difficultyLevel)
                .Include(a => a.Question)
                .Include(a => a.SelectedAnswer)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentQuestionAttempt>> GetByTopicAsync(int userId, string topic)
        {
            return await _context.StudentQuestionAttempts
                .Where(a => a.UserId == userId && a.Topic == topic)
                .Include(a => a.Question)
                .Include(a => a.SelectedAnswer)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetAccuracyByUserAsync(int userId)
        {
            var attempts = await _context.StudentQuestionAttempts
                .Where(a => a.UserId == userId)
                .ToListAsync();

            if (!attempts.Any()) return 0;

            var correctCount = attempts.Count(a => a.IsCorrect);
            return (decimal)correctCount / attempts.Count * 100;
        }

        public async Task<decimal> GetAccuracyByUserAndDifficultyAsync(int userId, string difficultyLevel)
        {
            var attempts = await _context.StudentQuestionAttempts
                .Where(a => a.UserId == userId && a.DifficultyLevel == difficultyLevel)
                .ToListAsync();

            if (!attempts.Any()) return 0;

            var correctCount = attempts.Count(a => a.IsCorrect);
            return (decimal)correctCount / attempts.Count * 100;
        }

        public async Task<decimal> GetAccuracyByUserAndTopicAsync(int userId, string topic)
        {
            var attempts = await _context.StudentQuestionAttempts
                .Where(a => a.UserId == userId && a.Topic == topic)
                .ToListAsync();

            if (!attempts.Any()) return 0;

            var correctCount = attempts.Count(a => a.IsCorrect);
            return (decimal)correctCount / attempts.Count * 100;
        }
    }
}
