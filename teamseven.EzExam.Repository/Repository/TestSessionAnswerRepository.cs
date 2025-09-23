using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class TestSessionAnswerRepository : GenericRepository<TestSessionAnswer>
    {
        public TestSessionAnswerRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        public async Task<List<TestSessionAnswer>> GetBySessionIdAsync(int sessionId)
        {
            return await GetAllAsync(x => x.TestSessionId == sessionId);
        }

        public async Task<List<TestSessionAnswer>> GetByQuestionIdAsync(int questionId)
        {
            return await GetAllAsync(x => x.QuestionId == questionId);
        }

        public async Task<TestSessionAnswer?> GetBySessionAndQuestionAsync(int sessionId, int questionId)
        {
            return await GetFirstOrDefaultAsync(x => x.TestSessionId == sessionId && x.QuestionId == questionId);
        }

        public async Task<List<TestSessionAnswer>> GetCorrectAnswersAsync(int sessionId)
        {
            return await GetAllAsync(x => x.TestSessionId == sessionId && x.IsCorrect == true);
        }

        public async Task<List<TestSessionAnswer>> GetIncorrectAnswersAsync(int sessionId)
        {
            return await GetAllAsync(x => x.TestSessionId == sessionId && x.IsCorrect == false);
        }

        public async Task<List<TestSessionAnswer>> GetMarkedForReviewAsync(int sessionId)
        {
            return await GetAllAsync(x => x.TestSessionId == sessionId && x.IsMarkedForReview == true);
        }

        public async Task<List<TestSessionAnswer>> GetChangedAnswersAsync(int sessionId)
        {
            return await GetAllAsync(x => x.TestSessionId == sessionId && x.IsChanged == true);
        }

        public async Task<decimal> GetAverageTimePerQuestionAsync(int sessionId)
        {
            var answers = await GetAllAsync(x => x.TestSessionId == sessionId && x.TimeSpent > 0);
            return (decimal)(answers.Any() ? answers.Average(x => x.TimeSpent) : 0);
        }

        public async Task<int> GetCorrectAnswersCountAsync(int sessionId)
        {
            var answers = await GetAllAsync(x => x.TestSessionId == sessionId && x.IsCorrect == true);
            return answers.Count;
        }

        public async Task<int> GetTotalAnswersCountAsync(int sessionId)
        {
            var answers = await GetAllAsync(x => x.TestSessionId == sessionId);
            return answers.Count;
        }
    }
}
