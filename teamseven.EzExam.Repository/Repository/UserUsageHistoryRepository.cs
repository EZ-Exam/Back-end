using Microsoft.EntityFrameworkCore;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class UserUsageHistoryRepository : GenericRepository<UserUsageHistory>, IUserUsageHistoryRepository
    {
        public UserUsageHistoryRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserUsageHistory>> GetUserUsageHistoryAsync(int userId, string? usageType = null, int? limit = null)
        {
            IQueryable<UserUsageHistory> query = _context.UserUsageHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.CreatedAt);

            if (!string.IsNullOrEmpty(usageType))
            {
                query = query.Where(h => h.UsageType == usageType);
            }

            if (limit.HasValue && limit > 0)
            {
                query = query.Take(limit.Value);
            }

            return await query.ToListAsync();
        }
    }
}
