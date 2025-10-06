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
                .Where(c => c.QuestionId == questionId && c.IsApproved && !c.IsDeleted)
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
                .Where(c => c.QuestionId == questionId && c.IsApproved && !c.IsDeleted)
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
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<QuestionComment> CreateAsync(QuestionComment comment)
        {
            _context.QuestionComments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<QuestionComment> UpdateAsync(QuestionComment comment)
        {           
            var existingComment = await _context.QuestionComments
                .FirstOrDefaultAsync(c => c.Id == comment.Id);
            
            if (existingComment != null)
            {
                existingComment.Content = comment.Content;
                existingComment.Rating = comment.Rating;
                existingComment.IsHelpful = comment.IsHelpful;
                existingComment.IsApproved = comment.IsApproved;
                existingComment.IsDeleted = comment.IsDeleted;
                existingComment.DeletedAt = comment.DeletedAt;
                existingComment.DeletedBy = comment.DeletedBy;
                existingComment.UpdatedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();
                return existingComment;
            }
            
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
                .Where(c => !c.IsApproved && !c.IsDeleted)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.QuestionComments.AnyAsync(c => c.Id == id);
        }
    }
}
