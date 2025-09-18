using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public interface ISubscriptionTypeRepository : IGenericRepository<SubscriptionType>
    {
        Task<SubscriptionType?> GetByCodeAsync(string code);
        Task<SubscriptionType?> GetBySubscriptionCodeAsync(string subscriptionCode);
        Task<IEnumerable<SubscriptionType>> GetActiveSubscriptionTypesAsync();
        Task<bool> IsSubscriptionCodeExistsAsync(string subscriptionCode, int? excludeId = null);
    }
}
