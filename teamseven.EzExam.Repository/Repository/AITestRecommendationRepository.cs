using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class AITestRecommendationRepository : GenericRepository<AITestRecommendation>
    {
        public AITestRecommendationRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        public async Task<List<AITestRecommendation>> GetByUserIdAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId);
        }

        public async Task<List<AITestRecommendation>> GetPendingByUserIdAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.IsAccepted == null);
        }

        public async Task<List<AITestRecommendation>> GetAcceptedByUserIdAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.IsAccepted == true);
        }

        public async Task<List<AITestRecommendation>> GetBySubjectAsync(int userId, int subjectId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.SubjectId == subjectId);
        }

        public async Task<List<AITestRecommendation>> GetExpiredRecommendationsAsync()
        {
            return await GetAllAsync(x => x.ExpiresAt.HasValue && x.ExpiresAt < DateTime.UtcNow);
        }

        public async Task<List<AITestRecommendation>> GetHighConfidenceRecommendationsAsync(int userId, decimal minConfidence = 0.8m)
        {
            return await GetAllAsync(x => x.UserId == userId && x.ConfidenceScore >= minConfidence);
        }

        public async Task<AITestRecommendation?> GetByGeneratedExamIdAsync(int examId)
        {
            return await GetFirstOrDefaultAsync(x => x.GeneratedExamId == examId);
        }

        public async Task<List<AITestRecommendation>> GetBasedOnWeakAreasAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.BasedOnWeakAreas == true);
        }
    }
}
