using teamseven.EzExam.Repository.Dtos;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Interfaces
{
    public interface IUserSubscriptionService
    {
        Task AddSubscriptionAsync(UserSubscriptionRequest request);
        Task UpdateAsync(UserSubscriptionResponse subscription);
        Task<UserSubscriptionResponse> GetSubscriptionByIdAsync(int id);
        Task<UserSubscriptionResponse> GetByPaymentGatewayTransactionIdAsync(string transactionId);
    }
}
