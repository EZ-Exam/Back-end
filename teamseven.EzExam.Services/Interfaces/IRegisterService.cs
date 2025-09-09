using teamseven.EzExam.Services.Object.Requests;

namespace teamseven.EzExam.Services.Interfaces
{
    public interface IRegisterService
    {
        Task<(bool IsSuccess, string ResultOrError)> RegisterUserAsync(RegisterRequest request);
        Task<(bool IsSuccess, string ResultOrError)> ChangeUserRoleAsync(int userId, string roleName, string providedSecretKey);

    }
}
