using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.UsageTrackingService
{
    public interface IUsageTrackingService
    {
        Task<bool> CanUserPerformActionAsync(int userId, string actionType);
        Task<bool> IncrementUsageAsync(UsageTrackingRequest request);
        Task<UserSubscriptionStatusResponse> GetUserSubscriptionStatusAsync(int userId);
        Task<IEnumerable<UsageTrackingResponse>> GetUserUsageTrackingAsync(int userId);
        Task<IEnumerable<UsageHistoryResponse>> GetUserUsageHistoryAsync(int userId, int? limit = null);
        Task<bool> ResetUserUsageAsync(int userId, string usageType);
        Task<bool> CheckAndIncrementSolutionViewAsync(int userId, int solutionId);
        Task<bool> CheckAndIncrementAIRequestAsync(int userId, string description = null);
    }
}
