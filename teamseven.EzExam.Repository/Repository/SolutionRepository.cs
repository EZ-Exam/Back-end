using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository.Context;

namespace teamseven.EzExam.Repository.Repository
{
    public class SolutionRepository : GenericRepository<Solution>
    {
        private readonly teamsevenezexamdbContext _context;

        public SolutionRepository(teamsevenezexamdbContext context)
        {
            _context = context;
        }

        public async Task<List<Solution>?> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<Solution?> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<List<Solution>?> GetByQuestionIdAsync(int questionId)
        {
            return await _context.Solutions
                .Where(s => s.QuestionId == questionId && s.IsDeleted != true)
                .ToListAsync();
        }

        public async Task<List<Solution>?> GetByCreatorAsync(long userId)
        {
            return await _context.Solutions
                .Where(s => s.CreatedByUserId == userId && s.IsDeleted != true)
                .ToListAsync();
        }

        public async Task<List<Solution>?> GetApprovedAsync()
        {
            return await _context.Solutions
                .Where(s => s.IsApproved && s.IsDeleted != true)
                .ToListAsync();
        }

        public async Task<int> AddAsync(Solution solution)
        {
            return await CreateAsync(solution);
        }

        public async Task<int> UpdateAsync(Solution solution)
        {
            return await base.UpdateAsync(solution);
        }

        public async Task<bool> DeleteAsync(Solution solution)
        {
            return await RemoveAsync(solution);
        }
    }
}
