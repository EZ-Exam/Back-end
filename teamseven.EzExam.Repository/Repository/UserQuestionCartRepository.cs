using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class UserQuestionCartRepository : GenericRepository<UserQuestionCart>
    {
        public UserQuestionCartRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        public async Task<List<UserQuestionCart>> GetByUserIdAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId);
        }

        public async Task<List<UserQuestionCart>> GetSelectedByUserIdAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.IsSelected);
        }

        public async Task<bool> IsQuestionInCartAsync(int userId, int questionId)
        {
            var cartItem = await GetFirstOrDefaultAsync(x => x.UserId == userId && x.QuestionId == questionId);
            return cartItem != null;
        }

        public async Task<UserQuestionCart?> GetByUserAndQuestionAsync(int userId, int questionId)
        {
            return await GetFirstOrDefaultAsync(x => x.UserId == userId && x.QuestionId == questionId);
        }

        public async Task<int> GetCartCountAsync(int userId)
        {
            var items = await GetAllAsync(x => x.UserId == userId && x.IsSelected);
            return items.Count;
        }

        public async Task<List<UserQuestionCart>> GetBySubjectAsync(int userId, int subjectId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.SubjectId == subjectId && x.IsSelected);
        }

        public async Task<List<UserQuestionCart>> GetByDifficultyAsync(int userId, string difficultyLevel)
        {
            return await GetAllAsync(x => x.UserId == userId && x.DifficultyPreference == difficultyLevel && x.IsSelected);
        }
    }
}
