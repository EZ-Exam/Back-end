using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Services.Interfaces
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
        //Task<(bool IsSuccess, string AccessToken, string ErrorMessage)> RefreshAccessTokenAsync(string refreshToken);
        bool IsUserInRole(string authHeader, string role);
        bool IsUserInPlan(string token, string plan);

        Task<string> GoogleLoginAsync(string idToken);
    }
}
