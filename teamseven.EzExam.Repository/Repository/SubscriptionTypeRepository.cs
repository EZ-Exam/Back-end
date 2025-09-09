using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository.Context;

namespace teamseven.EzExam.Repository.Repository
{
    public class SubscriptionTypeRepository: GenericRepository<SubscriptionType>
    {
        private readonly teamsevenezexamdbContext _context;

        public SubscriptionTypeRepository(teamsevenezexamdbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateSubcritionTypeAsync(SubscriptionType subcritionType)
        {
            return await CreateAsync(subcritionType);
        }
    }
}
