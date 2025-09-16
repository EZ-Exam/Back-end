using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public interface ISubscriptionTypeRepository : IGenericRepository<SubscriptionType>
    {
        Task<SubscriptionType?> GetByCodeAsync(string code);
        Task<IEnumerable<SubscriptionType>> GetActiveSubscriptionTypesAsync();
    }
}
