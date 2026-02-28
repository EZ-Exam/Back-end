using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Services.Interfaces
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
        Task<string> GenerateJwtTokenWithSubscriptionAsync(User user);
        bool IsUserInRole(string authHeader, string role);
        bool IsUserInPlan(string token, string plan);

        Task<string> GoogleLoginAsync(string idToken);
    }
}
