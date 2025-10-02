using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository.Context;

namespace teamseven.EzExam.Repository.Repository
{
    public interface IQuestionCommentRepository
    {
        Task<List<QuestionComment>> GetByQuestionIdAsync(int questionId);
        Task<List<QuestionComment>> GetByQuestionIdWithRepliesAsync(int questionId);
        Task<QuestionComment?> GetByIdAsync(int id);
        Task<QuestionComment> CreateAsync(QuestionComment comment);
        Task<QuestionComment> UpdateAsync(QuestionComment comment);
        Task DeleteAsync(int id);
        Task<List<QuestionComment>> GetPendingApprovalAsync();
        Task<bool> ExistsAsync(int id);
    }

    public class QuestionCommentRepository : GenericRepository<QuestionComment>, IQuestionCommentRepository
    {
        private readonly teamsevenezexamdbContext _context;

        public QuestionCommentRepository(teamsevenezexamdbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<QuestionComment>> GetByQuestionIdAsync(int questionId)
        {
            return await _context.QuestionComments
                .Include(c => c.User)
                .Include(c => c.ParentComment)
                .Where(c => c.QuestionId == questionId && c.IsApproved)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<QuestionComment>> GetByQuestionIdWithRepliesAsync(int questionId)
        {
            return await _context.QuestionComments
                .Include(c => c.User)
                .Include(c => c.ParentComment)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.User)
                .Where(c => c.QuestionId == questionId && c.IsApproved)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<QuestionComment?> GetByIdAsync(int id)
        {
            return await _context.QuestionComments
                .Include(c => c.User)
                .Include(c => c.ParentComment)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<QuestionComment> CreateAsync(QuestionComment comment)
        {
            _context.QuestionComments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<QuestionComment> UpdateAsync(QuestionComment comment)
        {
            // Tìm entity đã được track trong context
            var existingComment = await _context.QuestionComments
                .FirstOrDefaultAsync(c => c.Id == comment.Id);
            
            if (existingComment != null)
            {
                // Chỉ update những field cần thiết, không touch navigation properties
                existingComment.Content = comment.Content;
                existingComment.Rating = comment.Rating;
                existingComment.IsHelpful = comment.IsHelpful;
                existingComment.IsApproved = comment.IsApproved;
                existingComment.UpdatedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();
                return existingComment;
            }
            
            // Fallback nếu không tìm thấy existing entity
            comment.UpdatedAt = DateTime.UtcNow;
            _context.QuestionComments.Update(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task DeleteAsync(int id)
        {
            var comment = await _context.QuestionComments.FindAsync(id);
            if (comment != null)
            {
                _context.QuestionComments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<QuestionComment>> GetPendingApprovalAsync()
        {
            return await _context.QuestionComments
                .Include(c => c.User)
                .Include(c => c.Question)
                .Where(c => !c.IsApproved)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.QuestionComments.AnyAsync(c => c.Id == id);
        }
    }
}
