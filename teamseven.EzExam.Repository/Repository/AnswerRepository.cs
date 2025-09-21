using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class AnswerRepository : GenericRepository<Answer>
    {
        private readonly teamsevenezexamdbContext _context;

        public AnswerRepository(teamsevenezexamdbContext context)
        {
            _context = context;
        }

        public async Task<List<Answer>?> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<Answer?> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<List<Answer>?> GetByQuestionIdAsync(int questionId)
        {
            return await _context.Answers
                .Where(a => a.QuestionId == questionId && a.IsActive)
                .OrderBy(a => a.Order)
                .ToListAsync();
        }

        public async Task<int> AddAsync(Answer answer)
        {
            return await CreateAsync(answer);
        }

        public async Task<int> UpdateAsync(Answer answer)
        {
            return await base.UpdateAsync(answer);
        }

        public async Task<bool> DeleteAsync(Answer answer)
        {
            return await RemoveAsync(answer);
        }
    }
}
