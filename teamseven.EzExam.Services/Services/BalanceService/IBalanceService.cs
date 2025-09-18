using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.BalanceService
{
    public interface IBalanceService
    {
        Task<BalanceResponse> AddBalanceAsync(int userId, AddBalanceRequest request);
        Task<BalanceResponse> MasterDepositAsync(MasterDepositRequest request);
        Task<decimal> GetUserBalanceAsync(int userId);
        Task<BalanceResponse> GetUserBalanceInfoAsync(int userId);
    }
}
