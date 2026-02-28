using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.SubscriptionTypeService
{
    public interface ISubscriptionTypeService
    {
        Task<IEnumerable<SubscriptionTypeResponse>> GetAllSubscriptionTypesAsync();
        Task<IEnumerable<SubscriptionTypeResponse>> GetActiveSubscriptionTypesAsync();
        Task<SubscriptionTypeResponse> GetSubscriptionTypeByIdAsync(int id);
        Task<SubscriptionTypeResponse> GetSubscriptionTypeByCodeAsync(string subscriptionCode);
        Task<SubscriptionTypeResponse> CreateSubscriptionTypeAsync(SubscriptionTypeRequest request);
        Task<SubscriptionTypeResponse> UpdateSubscriptionTypeAsync(int id, SubscriptionTypeRequest request);
        Task DeleteSubscriptionTypeAsync(int id);
        Task ActivateSubscriptionTypeAsync(int id);
        Task DeactivateSubscriptionTypeAsync(int id);
        Task<bool> IsSubscriptionCodeExistsAsync(string subscriptionCode, int? excludeId = null);
    }
}