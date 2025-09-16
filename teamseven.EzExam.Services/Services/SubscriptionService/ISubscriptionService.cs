using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.SubscriptionService
{
    public interface ISubscriptionService
    {
        Task<SubscribeResponse> SubscribeUserAsync(int userId, SubscribeRequest request);
        Task<bool> CheckAndExpireSubscriptionsAsync();
        Task<SubscribeResponse> GetUserCurrentSubscriptionAsync(int userId);
        Task<bool> CancelUserSubscriptionAsync(int userId);
        Task<List<SubscribeResponse>> GetUserSubscriptionHistoryAsync(int userId);
    }
}

