using Microsoft.EntityFrameworkCore;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class SubscriptionTypeRepository : GenericRepository<SubscriptionType>, ISubscriptionTypeRepository
    {
        // HARDCODED SUBSCRIPTION TYPES - NO DATABASE QUERIES
        // BASIC and FREE are merged as one subscription type
        private static readonly Dictionary<int, SubscriptionType> _hardcodedSubscriptionTypes = new()
        {
            { 1, new SubscriptionType { Id = 1, SubscriptionName = "FREE", SubscriptionCode = "FREE", SubscriptionPrice = 0, MaxAIRequests = 100, MaxSolutionViews = 500, IsActive = true, IsAIEnabled = true, Features = "Free/Basic features with moderate limits", Description = "Free subscription with basic features - same as BASIC" } },
            { 2, new SubscriptionType { Id = 2, SubscriptionName = "BASIC", SubscriptionCode = "BASIC", SubscriptionPrice = 0, MaxAIRequests = 100, MaxSolutionViews = 500, IsActive = true, IsAIEnabled = true, Features = "Free/Basic features with moderate limits", Description = "Basic subscription - same as FREE, no cost" } },
            { 3, new SubscriptionType { Id = 3, SubscriptionName = "PREMIUM", SubscriptionCode = "PREMIUM", SubscriptionPrice = 199000, MaxAIRequests = 500, MaxSolutionViews = 2000, IsActive = true, IsAIEnabled = true, Features = "Premium features with high limits", Description = "Premium subscription with advanced features" } },
            { 4, new SubscriptionType { Id = 4, SubscriptionName = "UNLIMITED", SubscriptionCode = "UNLIMITED", SubscriptionPrice = 399000, MaxAIRequests = -1, MaxSolutionViews = -1, IsActive = true, IsAIEnabled = true, Features = "Unlimited features", Description = "Unlimited subscription with no limits" } }
        };

        public SubscriptionTypeRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        // HIDE GetByIdAsync to use hardcoded data
        public new async Task<SubscriptionType?> GetByIdAsync(int id)
        {
            // Return hardcoded data instead of database query
            if (_hardcodedSubscriptionTypes.ContainsKey(id))
            {
                return _hardcodedSubscriptionTypes[id];
            }
            return null;
        }

        // HIDE GetAllAsync to use hardcoded data
        public new async Task<IEnumerable<SubscriptionType>> GetAllAsync()
        {
            // Return hardcoded data instead of database query
            return _hardcodedSubscriptionTypes.Values;
        }

        public new async Task<SubscriptionType?> GetByCodeAsync(string code)
        {
            // Use hardcoded data instead of database query
            return _hardcodedSubscriptionTypes.Values.FirstOrDefault(st => st.SubscriptionCode == code);
        }

        public new async Task<SubscriptionType?> GetBySubscriptionCodeAsync(string subscriptionCode)
        {
            // Use hardcoded data instead of database query
            return _hardcodedSubscriptionTypes.Values.FirstOrDefault(st => st.SubscriptionCode == subscriptionCode);
        }

        public new async Task<IEnumerable<SubscriptionType>> GetActiveSubscriptionTypesAsync()
        {
            // Use hardcoded data instead of database query
            return _hardcodedSubscriptionTypes.Values.Where(st => st.IsActive);
        }

        public new async Task<bool> IsSubscriptionCodeExistsAsync(string subscriptionCode, int? excludeId = null)
        {
            // Use hardcoded data instead of database query
            var exists = _hardcodedSubscriptionTypes.Values.Any(st => st.SubscriptionCode == subscriptionCode);
            
            if (excludeId.HasValue)
            {
                exists = _hardcodedSubscriptionTypes.Values.Any(st => st.SubscriptionCode == subscriptionCode && st.Id != excludeId.Value);
            }
            
            return exists;
        }
    }
}