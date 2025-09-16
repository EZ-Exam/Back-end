using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public interface IUserSocialProviderRepository : IGenericRepository<UserSocialProvider>
    {
        Task<UserSocialProvider?> GetByProviderAsync(string providerName, string providerId);
    }
}
