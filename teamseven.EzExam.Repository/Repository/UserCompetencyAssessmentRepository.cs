using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class UserCompetencyAssessmentRepository : GenericRepository<UserCompetencyAssessment>
    {
        public UserCompetencyAssessmentRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        public async Task<List<UserCompetencyAssessment>> GetByUserIdAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.IsActive);
        }

        public async Task<List<UserCompetencyAssessment>> GetBySubjectAsync(int userId, int subjectId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.SubjectId == subjectId && x.IsActive);
        }

        public async Task<List<UserCompetencyAssessment>> GetByTopicAsync(int userId, string topic)
        {
            return await GetAllAsync(x => x.UserId == userId && x.Topic == topic && x.IsActive);
        }

        public async Task<List<UserCompetencyAssessment>> GetByDifficultyAsync(int userId, string difficultyLevel)
        {
            return await GetAllAsync(x => x.UserId == userId && x.DifficultyLevel == difficultyLevel && x.IsActive);
        }

        public async Task<UserCompetencyAssessment?> GetByUserSubjectTopicDifficultyAsync(int userId, int subjectId, string topic, string difficultyLevel)
        {
            return await GetFirstOrDefaultAsync(x => x.UserId == userId && x.SubjectId == subjectId && x.Topic == topic && x.DifficultyLevel == difficultyLevel && x.IsActive);
        }

        public async Task<List<UserCompetencyAssessment>> GetWeakAreasAsync(int userId, decimal maxCompetencyScore = 0.5m)
        {
            return await GetAllAsync(x => x.UserId == userId && x.CompetencyScore <= maxCompetencyScore && x.IsActive);
        }

        public async Task<List<UserCompetencyAssessment>> GetStrongAreasAsync(int userId, decimal minCompetencyScore = 0.8m)
        {
            return await GetAllAsync(x => x.UserId == userId && x.CompetencyScore >= minCompetencyScore && x.IsActive);
        }

        public async Task<List<UserCompetencyAssessment>> GetImprovingAreasAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.ImprovementTrend > 0 && x.IsActive);
        }

        public async Task<List<UserCompetencyAssessment>> GetDecliningAreasAsync(int userId)
        {
            return await GetAllAsync(x => x.UserId == userId && x.ImprovementTrend < 0 && x.IsActive);
        }

        public async Task<decimal> GetOverallCompetencyScoreAsync(int userId, int subjectId)
        {
            var assessments = await GetAllAsync(x => x.UserId == userId && x.SubjectId == subjectId && x.IsActive);
            return assessments.Any() ? assessments.Average(x => x.CompetencyScore) : 0;
        }
    }
}
