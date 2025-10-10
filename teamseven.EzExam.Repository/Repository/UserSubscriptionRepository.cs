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
        
        // HARDCODED USER SUBSCRIPTION DATA - NO DATABASE QUERIES
        // BASIC subscription is now free (same as FREE)
        private static readonly Dictionary<int, UserSubscription> _hardcodedUserSubscriptions = new()
        {
            { 1, new UserSubscription 
                { 
                    Id = 1, 
                    UserId = 1, 
                    SubscriptionTypeId = 2, // BASIC subscription
                    StartDate = DateTime.UtcNow.AddDays(-1), 
                    EndDate = DateTime.UtcNow.AddMonths(1), 
                    IsActive = true, 
                    Amount = 0, // BASIC is now free
                    PaymentStatus = "COMPLETED", 
                    PaymentGatewayTransactionId = "FREE_BASIC_TXN", 
                    CreatedAt = DateTime.UtcNow.AddDays(-1), 
                    UpdatedAt = DateTime.UtcNow.AddDays(-1) 
                } 
            }
        };

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

        public new async Task<List<UserSubscription>?> GetByUserIdAsync(long userId)
        {
            // Sử dụng database thực tế thay vì hardcoded data
            return await _context.UserSubscriptions
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
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
        public new async Task<List<UserSubscription>?> GetActiveSubscriptionsAsync(long userId)
        {
            // Use data 
            return await _context.UserSubscriptions
                .Where(x => x.UserId == userId && x.IsActive == true)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public new async Task<UserSubscription?> GetActiveSubscriptionByUserIdAsync(int userId)
        {
            // Sử dụng database thực tế thay vì hardcoded data
            return await _context.UserSubscriptions
                .Where(x => x.UserId == userId && x.IsActive == true)
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
            var existingSubscription = await _context.UserSubscriptions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == subscription.Id);
                
            if (existingSubscription == null)
            {
                throw new KeyNotFoundException($"Subscription with ID {subscription.Id} not found.");
            }

            // Update fields
            existingSubscription.IsActive = subscription.IsActive;
            existingSubscription.UpdatedAt = subscription.UpdatedAt;
            existingSubscription.PaymentStatus = subscription.PaymentStatus;
            existingSubscription.Amount = subscription.Amount;
            existingSubscription.EndDate = subscription.EndDate;

            // Attach và mark as modified
            _context.UserSubscriptions.Attach(existingSubscription);
            _context.Entry(existingSubscription).State = EntityState.Modified;

            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(UserSubscription subscription)
        {
            return await RemoveAsync(subscription);
        }
    }
}
