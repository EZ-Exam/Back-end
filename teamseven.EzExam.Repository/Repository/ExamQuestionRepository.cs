using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class ExamQuestionRepository : GenericRepository<ExamQuestion>
    {
        private readonly teamsevenezexamdbContext _context;

        public ExamQuestionRepository(teamsevenezexamdbContext context)
        {
            _context = context;
        }

        public async Task<List<ExamQuestion>?> GetByExamIdAsync(int examId)
        {
            return await _context.ExamQuestions
                .Where(eq => eq.ExamId == examId)
                .Include(eq => eq.Exam)     // T?i Exam d? l?y ExamName
                .Include(eq => eq.Question) // T?i Question d? l?y QuestionContent
                .ToListAsync();
        }

        public async Task<List<ExamQuestion>?> GetByQuestionIdAsync(int questionId)
        {
            return await _context.ExamQuestions
                .Where(eq => eq.QuestionId == questionId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Exam>> GetByCreatorAsync(int userId)
        {
            return await _context.Exams
                .Where(e => e.CreatedByUserId == userId && !e.IsDeleted) // L?c theo userId và không b? xóa
                .Include(e => e.Lesson)        // T?i Lesson d? l?y LessonName
                .Include(e => e.ExamType)      // T?i ExamType d? l?y ExamTypeName
                .Include(e => e.CreatedByUser) // T?i CreatedByUser d? l?y Name ho?c Email
                .ToListAsync();
        }
        public async Task<int> AddAsync(ExamQuestion examQuestion)
        {
            return await CreateAsync(examQuestion);
        }

        public async Task<int> UpdateAsync(ExamQuestion examQuestion)
        {
            return await base.UpdateAsync(examQuestion);
        }

        public async Task<bool> DeleteAsync(ExamQuestion examQuestion)
        {
            return await RemoveAsync(examQuestion);
        }

        public async Task<ExamQuestion?> GetByIdAsync(int examId)
        {
            return await base.GetByIdAsync(examId);
        }

        public async Task<ExamQuestion?> GetByExamAndQuestionIdAsync(int examId, int questionId)
        {
            return await _context.ExamQuestions
                .FirstOrDefaultAsync(eq => eq.ExamId == examId && eq.QuestionId == questionId);
        }
    }
}
