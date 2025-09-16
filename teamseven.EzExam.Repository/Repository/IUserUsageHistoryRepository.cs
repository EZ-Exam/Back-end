using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository.Interfaces
{
    public interface IUserUsageHistoryRepository : IGenericRepository<UserUsageHistory>
    {
        Task<IEnumerable<UserUsageHistory>> GetUserUsageHistoryAsync(int userId, int? limit = null);
        Task<IEnumerable<UserUsageHistory>> GetUsageHistoryByTypeAsync(int userId, string usageType, int? limit = null);
        Task<IEnumerable<UserUsageHistory>> GetUsageHistoryByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
    }
}
