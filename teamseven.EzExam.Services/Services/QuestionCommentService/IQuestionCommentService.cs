using System.Collections.Generic;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Dtos;
using teamseven.EzExam.Services.Object.Requests;

namespace teamseven.EzExam.Services.Services.QuestionCommentService
{
    public interface IQuestionCommentService
    {
        Task<QuestionCommentResponse> CreateCommentAsync(CreateQuestionCommentRequest request);
        Task<QuestionCommentResponse> UpdateCommentAsync(UpdateQuestionCommentRequest request, int userId, int roleId);
        Task SoftDeleteCommentAsync(int commentId, int deletedByUserId);
        Task<List<QuestionCommentResponse>> GetCommentsByQuestionIdAsync(int questionId);
        Task<QuestionCommentResponse> GetCommentByIdAsync(int commentId);
        Task<QuestionCommentResponse> ApproveCommentAsync(ApproveQuestionCommentRequest request);
        Task<List<QuestionCommentResponse>> GetPendingApprovalCommentsAsync();
        Task<bool> CanUserModifyCommentAsync(int commentId, int userId, int roleId);
    }
}
