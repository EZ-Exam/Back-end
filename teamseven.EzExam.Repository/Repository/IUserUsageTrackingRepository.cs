using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public interface IUserUsageTrackingRepository : IGenericRepository<UserUsageTracking>
    {
        Task<UserUsageTracking?> GetUserUsageTrackingAsync(int userId, int subscriptionTypeId, string usageType, DateTime resetDate);
    }
}
