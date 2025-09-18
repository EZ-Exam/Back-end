using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class UserSubscriptionRepository : GenericRepository<UserSubscription>
    {
        private readonly teamsevenezexamdbContext _context;

        public UserSubscriptionRepository(teamsevenezexamdbContext context)
        {
            _context = context;
        }

        public async Task<List<UserSubscription>?> GetAllSubscriptionsAsync()
        {
            return await GetAllAsync();
        }

        public async Task<UserSubscription?> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<List<UserSubscription>?> GetByUserIdAsync(long userId)
        {
            return await _context.UserSubscriptions
                .AsNoTracking() // Read-only query
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }
        public async Task<UserSubscription> GetByPaymentGatewayTransactionIdAsync(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
            {
                throw new ArgumentNullException(nameof(transactionId), "PaymentGatewayTransactionId cannot be null or empty.");
            }

            return await _context.UserSubscriptions
                .FirstOrDefaultAsync(us => us.PaymentGatewayTransactionId == transactionId);
        }
        public async Task<List<UserSubscription>?> GetActiveSubscriptionsAsync(long userId)
        {
            return await _context.UserSubscriptions
                .AsNoTracking() // Read-only query
                .Where(x => x.UserId == userId && x.IsActive)
                .ToListAsync();
        }

        public async Task<UserSubscription?> GetActiveSubscriptionByUserIdAsync(int userId)
        {
            // Optimized query with proper indexing
            return await _context.UserSubscriptions
                .AsNoTracking() // Read-only query, no change tracking needed
                .Where(x => x.UserId == userId && x.IsActive)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserSubscription>?> GetSubscriptionsByUserIdAsync(int userId)
        {
            return await _context.UserSubscriptions
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<UserSubscription>?> GetExpiredSubscriptionsAsync()
        {
            return await _context.UserSubscriptions
                .AsNoTracking() // Read-only query
                .Where(x => x.IsActive && x.EndDate.HasValue && x.EndDate.Value < DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<int> AddAsync(UserSubscription subscription)
        {
            return await CreateAsync(subscription);
        }

        public async Task<int> UpdateAsync(UserSubscription subscription)
        {
            return await base.UpdateAsync(subscription);
        }

        public async Task<bool> DeleteAsync(UserSubscription subscription)
        {
            return await RemoveAsync(subscription);
        }
    }
}
