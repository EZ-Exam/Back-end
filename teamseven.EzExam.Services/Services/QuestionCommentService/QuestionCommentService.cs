using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Dtos;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;

namespace teamseven.EzExam.Services.Services.QuestionCommentService
{
    public class QuestionCommentService : IQuestionCommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QuestionCommentService> _logger;

        public QuestionCommentService(IUnitOfWork unitOfWork, ILogger<QuestionCommentService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<QuestionCommentResponse> CreateCommentAsync(CreateQuestionCommentRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("CreateQuestionCommentRequest is null.");
                throw new ArgumentNullException(nameof(request), "Comment request cannot be null.");
            }

            // Verify question exists
            var question = await _unitOfWork.QuestionRepository.GetByIdAsync(request.QuestionId);
            if (question == null)
            {
                _logger.LogWarning("Question with ID {QuestionId} not found.", request.QuestionId);
                throw new NotFoundException($"Question with ID {request.QuestionId} not found.");
            }

            // Verify user exists
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", request.UserId);
                throw new NotFoundException($"User with ID {request.UserId} not found.");
            }

            // Verify parent comment exists if provided
            if (request.ParentCommentId.HasValue)
            {
                var parentComment = await _unitOfWork.QuestionCommentRepository.GetByIdAsync(request.ParentCommentId.Value);
                if (parentComment == null)
                {
                    _logger.LogWarning("Parent comment with ID {ParentCommentId} not found.", request.ParentCommentId);
                    throw new NotFoundException($"Parent comment with ID {request.ParentCommentId} not found.");
                }
                
                // Verify parent comment belongs to the same question
                if (parentComment.QuestionId != request.QuestionId)
                {
                    _logger.LogWarning("Parent comment {ParentCommentId} does not belong to question {QuestionId}.", request.ParentCommentId, request.QuestionId);
                    throw new ArgumentException($"Parent comment {request.ParentCommentId} does not belong to question {request.QuestionId}.");
                }
                
                // Verify parent comment is approved
                if (!parentComment.IsApproved)
                {
                    _logger.LogWarning("Parent comment {ParentCommentId} is not approved.", request.ParentCommentId);
                    throw new ArgumentException($"Cannot reply to unapproved comment {request.ParentCommentId}.");
                }
            }

            var comment = new QuestionComment
            {
                QuestionId = request.QuestionId,
                UserId = request.UserId,
                Content = request.Content,
                ParentCommentId = request.ParentCommentId,
                Rating = request.Rating,
                IsHelpful = false,
                IsApproved = true, // Auto-approve for now, can be changed to false for moderation
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                var createdComment = await _unitOfWork.QuestionCommentRepository.CreateAsync(comment);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                _logger.LogInformation("Comment with ID {CommentId} created successfully.", createdComment.Id);

                return await MapToResponseAsync(createdComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment: {Message}", ex.Message);
                throw new ApplicationException("An error occurred while creating the comment.", ex);
            }
        }

        public async Task<QuestionCommentResponse> UpdateCommentAsync(UpdateQuestionCommentRequest request, int userId, int roleId)
        {
            if (request == null)
            {
                _logger.LogWarning("UpdateQuestionCommentRequest is null.");
                throw new ArgumentNullException(nameof(request));
            }

            var existingComment = await _unitOfWork.QuestionCommentRepository.GetByIdAsync(request.Id);
            if (existingComment == null)
            {
                _logger.LogWarning("Comment with ID {CommentId} not found.", request.Id);
                throw new NotFoundException($"Comment with ID {request.Id} not found.");
            }

            // Check if user can modify this comment
            if (!await CanUserModifyCommentAsync(request.Id, userId, roleId))
            {
                _logger.LogWarning("User {UserId} cannot modify comment {CommentId}.", userId, request.Id);
                throw new UnauthorizedAccessException("You are not authorized to modify this comment.");
            }

            existingComment.Content = request.Content;
            if (request.Rating.HasValue) existingComment.Rating = request.Rating.Value;
            if (request.IsHelpful.HasValue) existingComment.IsHelpful = request.IsHelpful.Value;
            existingComment.UpdatedAt = DateTime.UtcNow;

            try
            {
                var updatedComment = await _unitOfWork.QuestionCommentRepository.UpdateAsync(existingComment);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                _logger.LogInformation("Comment with ID {CommentId} updated successfully.", updatedComment.Id);

                return await MapToResponseAsync(updatedComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment with ID {CommentId}: {Message}", request.Id, ex.Message);
                throw new ApplicationException($"Error updating comment with ID {request.Id}", ex);
            }
        }

        public async Task SoftDeleteCommentAsync(int commentId, int deletedByUserId)
        {
            var comment = await _unitOfWork.QuestionCommentRepository.GetByIdIncludingDeletedAsync(commentId);
            if (comment == null)
            {
                _logger.LogWarning("Comment with ID {CommentId} not found.", commentId);
                throw new NotFoundException($"Comment with ID {commentId} not found.");
            }

            // Check if comment is already deleted
            if (comment.IsDeleted)
            {
                _logger.LogWarning("Comment with ID {CommentId} is already deleted.", commentId);
                throw new ArgumentException($"Comment with ID {commentId} is already deleted.");
            }

            try
            {
                _logger.LogInformation("Before soft delete - Comment ID {CommentId}, IsDeleted: {IsDeleted}, DeletedAt: {DeletedAt}", 
                    commentId, comment.IsDeleted, comment.DeletedAt);

                comment.IsDeleted = true;
                comment.DeletedAt = DateTime.UtcNow;
                comment.DeletedBy = deletedByUserId;
                comment.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("After setting values - Comment ID {CommentId}, IsDeleted: {IsDeleted}, DeletedAt: {DeletedAt}", 
                    commentId, comment.IsDeleted, comment.DeletedAt);

                await _unitOfWork.QuestionCommentRepository.UpdateAsync(comment);
                var saveResult = await _unitOfWork.SaveChangesWithTransactionAsync();
                
                _logger.LogInformation("Save result: {SaveResult} rows affected", saveResult);
                _logger.LogInformation("Comment with ID {CommentId} soft deleted successfully by user {DeletedByUserId}.", commentId, deletedByUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft deleting comment with ID {CommentId}: {Message}", commentId, ex.Message);
                throw new ApplicationException($"Error soft deleting comment with ID {commentId}", ex);
            }
        }

        public async Task<List<QuestionCommentResponse>> GetCommentsByQuestionIdAsync(int questionId)
        {
            try
            {
                var comments = await _unitOfWork.QuestionCommentRepository.GetByQuestionIdWithRepliesAsync(questionId);
                
                // Group comments into main comments and replies
                var mainComments = comments.Where(c => c.ParentCommentId == null).ToList();
                var replies = comments.Where(c => c.ParentCommentId != null).ToList();

                var responses = new List<QuestionCommentResponse>();

                foreach (var mainComment in mainComments)
                {
                    var mainCommentResponse = await MapToResponseAsync(mainComment);
                    
                    // Add replies for this main comment
                    var commentReplies = replies.Where(r => r.ParentCommentId == mainComment.Id).ToList();
                    mainCommentResponse.ReplyCount = commentReplies.Count;
                    mainCommentResponse.Replies = new List<QuestionCommentResponse>();
                    
                    foreach (var reply in commentReplies)
                    {
                        mainCommentResponse.Replies.Add(await MapToResponseAsync(reply));
                    }

                    responses.Add(mainCommentResponse);
                }

                return responses.OrderBy(c => c.CreatedAt).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving comments for QuestionId {QuestionId}: {Message}", questionId, ex.Message);
                throw new ApplicationException($"Error retrieving comments for question {questionId}.", ex);
            }
        }

        public async Task<QuestionCommentResponse> GetCommentByIdAsync(int commentId)
        {
            var comment = await _unitOfWork.QuestionCommentRepository.GetByIdAsync(commentId);
            if (comment == null)
            {
                _logger.LogWarning("Comment with ID {CommentId} not found.", commentId);
                throw new NotFoundException($"Comment with ID {commentId} not found.");
            }

            return await MapToResponseAsync(comment);
        }

        public async Task<QuestionCommentResponse> ApproveCommentAsync(ApproveQuestionCommentRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("ApproveQuestionCommentRequest is null.");
                throw new ArgumentNullException(nameof(request));
            }

            var comment = await _unitOfWork.QuestionCommentRepository.GetByIdAsync(request.Id);
            if (comment == null)
            {
                _logger.LogWarning("Comment with ID {CommentId} not found.", request.Id);
                throw new NotFoundException($"Comment with ID {request.Id} not found.");
            }

            comment.IsApproved = request.IsApproved;
            comment.UpdatedAt = DateTime.UtcNow;

            try
            {
                var updatedComment = await _unitOfWork.QuestionCommentRepository.UpdateAsync(comment);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                _logger.LogInformation("Comment with ID {CommentId} approval status updated to {IsApproved}.", request.Id, request.IsApproved);

                return await MapToResponseAsync(updatedComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment approval status with ID {CommentId}: {Message}", request.Id, ex.Message);
                throw new ApplicationException($"Error updating comment approval status with ID {request.Id}", ex);
            }
        }

        public async Task<List<QuestionCommentResponse>> GetPendingApprovalCommentsAsync()
        {
            try
            {
                var comments = await _unitOfWork.QuestionCommentRepository.GetPendingApprovalAsync();
                var responses = new List<QuestionCommentResponse>();

                foreach (var comment in comments)
                {
                    responses.Add(await MapToResponseAsync(comment));
                }

                return responses.OrderBy(c => c.CreatedAt).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending approval comments: {Message}", ex.Message);
                throw new ApplicationException("Error retrieving pending approval comments.", ex);
            }
        }

        public async Task<bool> CanUserModifyCommentAsync(int commentId, int userId, int roleId)
        {
            var comment = await _unitOfWork.QuestionCommentRepository.GetByIdAsync(commentId);
            if (comment == null) return false;

            if (roleId == 3)
            {
                return true;
            }

            // Nếu user là role 1 (User) và là chủ comment thì cho phép
            if (roleId == 1 && comment.UserId == userId)
            {
                return true;
            }

            return false;
        }


        private async Task<QuestionCommentResponse> MapToResponseAsync(QuestionComment comment)
        {
            return new QuestionCommentResponse
            {
                Id = comment.Id,
                QuestionId = comment.QuestionId,
                UserId = comment.UserId,
                Content = comment.Content,
                ParentCommentId = comment.ParentCommentId,
                Rating = comment.Rating,
                IsHelpful = comment.IsHelpful,
                IsApproved = comment.IsApproved,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                UserName = comment.User?.Email ?? "Unknown",
                UserEmail = comment.User?.Email ?? "Unknown",
                ReplyCount = 0,
                Replies = new List<QuestionCommentResponse>()
            };
        }
    }
}
