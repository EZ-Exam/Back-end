using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public interface IUserUsageHistoryRepository : IGenericRepository<UserUsageHistory>
    {
        Task<IEnumerable<UserUsageHistory>> GetUserUsageHistoryAsync(int userId, string? usageType = null, int? limit = null);
    }
}
