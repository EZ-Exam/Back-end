using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.UserSocialProviderService
{
    public interface IUserSocialProviderService
    {
        Task<IEnumerable<UserSocialProviderDataResponse>> GetAllAsync();
        Task<UserSocialProviderDataResponse> GetByIdAsync(int id);
        Task CreateAsync(CreateUserSocialProviderRequest request);
        Task UpdateAsync(UserSocialProviderDataRequest request);
        Task DeleteAsync(int id);
    }
}
