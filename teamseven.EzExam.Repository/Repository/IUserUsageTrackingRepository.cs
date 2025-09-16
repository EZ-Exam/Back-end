using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository.Interfaces
{
    public interface IUserUsageTrackingRepository : IGenericRepository<UserUsageTracking>
    {
        Task<IEnumerable<UserUsageTracking>> GetUserUsageByDateAsync(int userId, DateTime date);
        Task<UserUsageTracking?> GetUserUsageByTypeAndDateAsync(int userId, string usageType, DateTime date);
        Task<IEnumerable<UserUsageTracking>> GetExpiredUsageTrackingsAsync();
    }
}
