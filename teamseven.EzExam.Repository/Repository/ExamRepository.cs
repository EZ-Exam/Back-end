using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class ExamRepository : GenericRepository<Exam>
    {
        private readonly teamsevenezexamdbContext _context;

        public ExamRepository(teamsevenezexamdbContext context)
        {
            _context = context;
        }

        public async Task<List<Exam>?> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<Exam?> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<List<Exam>?> GetByLessonIdAsync(int lessonId)
        {
            return await _context.Exams
                .Where(e => e.LessonId == lessonId && !e.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Exam>?> GetByCreatorAsync(long userId)
        {
            return await _context.Exams
                .Where(e => e.CreatedByUserId == userId)
                .ToListAsync();
        }

        public async Task<int> AddAsync(Exam exam)
        {
            return await CreateAsync(exam);
        }

        public async Task<int> UpdateAsync(Exam exam)
        {
            return await base.UpdateAsync(exam);
        }

        public async Task<bool> DeleteAsync(Exam exam)
        {
            return await RemoveAsync(exam);
        }
    }
}
