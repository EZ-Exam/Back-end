using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository.Context;

namespace teamseven.EzExam.Repository.Repository
{
    public class UserSocialProviderRepository : GenericRepository<UserSocialProvider>
    {
        private readonly teamsevenezexamdbContext _context;

        public UserSocialProviderRepository(teamsevenezexamdbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<UserSocialProvider>?> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<UserSocialProvider?> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<UserSocialProvider?> GetByProviderAsync(string providerName, string providerId)
        {
            return await _context.UserSocialProviders
                .FirstOrDefaultAsync(usp => usp.ProviderName == providerName && usp.ProviderId == providerId);
        }

        public async Task<List<UserSocialProvider>?> GetByUserIdAsync(int userId)
        {
            return await _context.UserSocialProviders
                .Where(usp => usp.UserId == userId)
                .ToListAsync();
        }

        public async Task<int> AddAsync(UserSocialProvider entity)
        {
            return await base.CreateAsync(entity);
        }

        public async Task<int> UpdateEntityAsync(UserSocialProvider entity)
        {
            return await base.UpdateAsync(entity);
        }

        public async Task<bool> DeleteEntityAsync(UserSocialProvider entity)
        {
            return await base.RemoveAsync(entity);
        }
    }
}
