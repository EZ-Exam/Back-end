using Microsoft.EntityFrameworkCore;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class UserUsageTrackingRepository : GenericRepository<UserUsageTracking>, IUserUsageTrackingRepository
    {
        public UserUsageTrackingRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserUsageTracking>> GetUserUsageByDateAsync(int userId, DateTime date)
        {
            return await _context.UserUsageTrackings
                .Where(ut => ut.UserId == userId && ut.ResetDate == date.Date)
                .ToListAsync();
        }

        public async Task<UserUsageTracking?> GetUserUsageByTypeAndDateAsync(int userId, string usageType, DateTime date)
        {
            return await _context.UserUsageTrackings
                .FirstOrDefaultAsync(ut => ut.UserId == userId && 
                                          ut.UsageType == usageType && 
                                          ut.ResetDate == date.Date);
        }

        public async Task<IEnumerable<UserUsageTracking>> GetExpiredUsageTrackingsAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.UserUsageTrackings
                .Where(ut => ut.ResetDate < today)
                .ToListAsync();
        }
    }
}
