using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Models;
namespace teamseven.EzExam.Repository.Repository.Interfaces
{
        public interface IUserSocialProviderRepository
        {
            Task<UserSocialProvider> GetByProviderAsync(string providerName, string providerId);
            Task AddAsync(UserSocialProvider userSocialProvider);
        }
}
