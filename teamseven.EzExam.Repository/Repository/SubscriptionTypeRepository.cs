using Microsoft.EntityFrameworkCore;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class SubscriptionTypeRepository : GenericRepository<SubscriptionType>, ISubscriptionTypeRepository
    {
        public SubscriptionTypeRepository(teamsevenezexamdbContext context) : base(context)
        {
        }

        public async Task<SubscriptionType?> GetByCodeAsync(string code)
        {
            return await _context.SubscriptionTypes.FirstOrDefaultAsync(st => st.SubscriptionCode == code);
        }

        public async Task<IEnumerable<SubscriptionType>> GetActiveSubscriptionTypesAsync()
        {
            return await _context.SubscriptionTypes.Where(st => st.IsActive).ToListAsync();
        }
    }
}