using Microsoft.EntityFrameworkCore;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class UserUsageTrackingRepository : GenericRepository<UserUsageTracking>, IUserUsageTrackingRepository
    {
        public UserUsageTrackingRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        public async Task<UserUsageTracking?> GetUserUsageTrackingAsync(int userId, int subscriptionTypeId, string usageType, DateTime resetDate)
        {
            return await _context.UserUsageTrackings
                .FirstOrDefaultAsync(t => t.UserId == userId && t.SubscriptionTypeId == subscriptionTypeId && t.UsageType == usageType && t.ResetDate == resetDate.Date);
        }
    }
}
