using Microsoft.EntityFrameworkCore;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class UserUsageHistoryRepository : GenericRepository<UserUsageHistory>, IUserUsageHistoryRepository
    {
        public UserUsageHistoryRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserUsageHistory>> GetUserUsageHistoryAsync(int userId, int? limit = null)
        {
            var query = _context.UserUsageHistories
                .Where(uh => uh.UserId == userId)
                .OrderByDescending(uh => uh.CreatedAt);

            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<UserUsageHistory>> GetUsageHistoryByTypeAsync(int userId, string usageType, int? limit = null)
        {
            var query = _context.UserUsageHistories
                .Where(uh => uh.UserId == userId && uh.UsageType == usageType)
                .OrderByDescending(uh => uh.CreatedAt);

            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<UserUsageHistory>> GetUsageHistoryByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _context.UserUsageHistories
                .Where(uh => uh.UserId == userId && 
                            uh.CreatedAt >= startDate && 
                            uh.CreatedAt <= endDate)
                .OrderByDescending(uh => uh.CreatedAt)
                .ToListAsync();
        }
    }
}
