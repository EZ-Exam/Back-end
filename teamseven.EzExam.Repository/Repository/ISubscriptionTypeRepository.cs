using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository.Interfaces
{
    public interface ISubscriptionTypeRepository : IGenericRepository<SubscriptionType>
    {
        Task<SubscriptionType?> GetBySubscriptionCodeAsync(string subscriptionCode);
        Task<IEnumerable<SubscriptionType>> GetActiveSubscriptionTypesAsync();
        Task<bool> IsSubscriptionCodeExistsAsync(string subscriptionCode, int? excludeId = null);
    }
}
