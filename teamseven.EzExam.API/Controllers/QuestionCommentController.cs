using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Dtos;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Services.ServiceProvider;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/question-comments")]
    [Produces("application/json")]
    public class QuestionCommentController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;
        private readonly ILogger<QuestionCommentController> _logger;

        public QuestionCommentController(IServiceProviders serviceProvider, ILogger<QuestionCommentController> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Create a new comment for a question
        /// </summary>
        /// <param name="request">Comment information</param>
        /// <returns>Created comment</returns>
        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Create a new comment for a question")]
        [SwaggerResponse(200, "Comment created successfully", typeof(QuestionCommentResponse))]
        [SwaggerResponse(400, "Invalid data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Question or user not found")]
        public async Task<IActionResult> CreateComment([FromBody] CreateQuestionCommentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _serviceProvider.QuestionCommentService.CreateCommentAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when creating comment");
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found when creating comment");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment");
                return StatusCode(500, "An error occurred while creating the comment.");
            }
        }

        /// <summary>
        /// Update a comment
        /// </summary>
        /// <param name="id">Comment ID</param>
        /// <param name="request">Update information</param>
        /// <returns>Updated comment</returns>
        [HttpPut]
        [Authorize]
        [SwaggerOperation(Summary = "Update a comment")]
        [SwaggerResponse(200, "Comment updated successfully", typeof(QuestionCommentResponse))]
        [SwaggerResponse(400, "Invalid data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden - insufficient permissions")]
        [SwaggerResponse(404, "Comment not found")]
        public async Task<IActionResult> UpdateComment(UpdateQuestionCommentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = GetCurrentUserId();
                var roleId = GetCurrentUserRoleId();

                if (userId == null || roleId == null)
                {
                    return Unauthorized("User ID or Role ID not found in token");
                }

                var result = await _serviceProvider.QuestionCommentService.UpdateCommentAsync(request, userId.Value, roleId.Value);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when updating comment");
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Comment not found when updating");
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access when updating comment");
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment");
                return StatusCode(500, "An error occurred while updating the comment.");
            }
        }

        /// <summary>
        /// Delete a comment (soft delete - moderator only)
        /// </summary>
        /// <param name="id">Comment ID</param>
        /// <returns>Delete result</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "SaleStaffPolicy")]
        [SwaggerOperation(Summary = "Delete a comment (soft delete - moderator only)")]
        [SwaggerResponse(200, "Comment deleted successfully")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden - moderator role required")]
        [SwaggerResponse(404, "Comment not found")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var roleId = GetCurrentUserRoleId();

                if (userId == null || roleId == null)
                {
                    return Unauthorized("User ID or Role ID not found in token");
                }

                // Kiểm tra role moderator (roleId = 3)
                if (roleId.Value != 3)
                {
                    return StatusCode(403, "Only moderators can delete comments");
                }

                await _serviceProvider.QuestionCommentService.SoftDeleteCommentAsync(id, userId.Value);
                return Ok(new { message = "Comment deleted successfully" });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Comment not found when deleting");
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access when deleting comment");
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment");
                return StatusCode(500, "An error occurred while deleting the comment.");
            }
        }

        /// <summary>
        /// Get comments by question ID (flat structure)
        /// </summary>
        /// <param name="questionId">Question ID</param>
        /// <returns>List of comments with replies</returns>
        [HttpGet("by-question/{questionId}")]
        [SwaggerOperation(Summary = "Get comments by question ID")]
        [SwaggerResponse(200, "List of comments", typeof(List<QuestionCommentResponse>))]
        [SwaggerResponse(404, "Question not found")]
        public async Task<IActionResult> GetCommentsByQuestionId(int questionId)
        {
            try
            {
                var result = await _serviceProvider.QuestionCommentService.GetCommentsByQuestionIdAsync(questionId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Question not found when getting comments");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments by question ID");
                return StatusCode(500, "An error occurred while retrieving comments.");
            }
        }

        /// <summary>
        /// Get available parent comment IDs for a question
        /// </summary>
        /// <param name="questionId">Question ID</param>
        /// <returns>List of available parent comment IDs</returns>
        [HttpGet("available-parents/{questionId}")]
        [SwaggerOperation(Summary = "Get available parent comment IDs")]
        [SwaggerResponse(200, "List of available parent comment IDs", typeof(List<object>))]
        public async Task<IActionResult> GetAvailableParentComments(int questionId)
        {
            try
            {
                var comments = await _serviceProvider.QuestionCommentService.GetCommentsByQuestionIdAsync(questionId);
                var availableParents = comments
                    .Where(c => c.IsApproved) // Chỉ comment đã được duyệt
                    .Select(c => new { 
                        Id = c.Id, 
                        Content = c.Content.Length > 50 ? c.Content.Substring(0, 50) + "..." : c.Content,
                        UserName = c.UserName,
                        CreatedAt = c.CreatedAt,
                        ReplyCount = c.ReplyCount
                    })
                    .ToList();
                
                return Ok(availableParents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available parent comments");
                return StatusCode(500, "An error occurred while retrieving available parent comments.");
            }
        }

        /// <summary>
        /// Get comment by ID
        /// </summary>
        /// <param name="id">Comment ID</param>
        /// <returns>Comment</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get comment by ID")]
        [SwaggerResponse(200, "Comment", typeof(QuestionCommentResponse))]
        [SwaggerResponse(404, "Comment not found")]
        public async Task<IActionResult> GetCommentById(int id)
        {
            try
            {
                var result = await _serviceProvider.QuestionCommentService.GetCommentByIdAsync(id);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Comment not found");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comment by ID");
                return StatusCode(500, "An error occurred while retrieving the comment.");
            }
        }

        /// <summary>
        /// Approve a comment (moderator/admin only)
        /// </summary>
        /// <param name="id">Comment ID</param>
        /// <param name="request">Approval information</param>
        /// <returns>Approved comment</returns>
        [HttpPut("{id}/approve")]
        [Authorize(Policy = "SaleStaffPolicy")]
        [SwaggerOperation(Summary = "Approve a comment ")]
        [SwaggerResponse(200, "Comment approved successfully", typeof(QuestionCommentResponse))]
        [SwaggerResponse(400, "Invalid data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden - moderator/admin role required")]
        [SwaggerResponse(404, "Comment not found")]
        public async Task<IActionResult> ApproveComment(int id, [FromBody] ApproveQuestionCommentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                request.Id = id;
                var result = await _serviceProvider.QuestionCommentService.ApproveCommentAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when approving comment");
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Comment not found when approving");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving comment");
                return StatusCode(500, "An error occurred while approving the comment.");
            }
        }

        /// <summary>
        /// Get pending approval comments (moderator/admin only)
        /// </summary>
        /// <returns>List of pending approval comments</returns>
        [HttpGet("pending-approval")]
        [Authorize(Policy = "SaleStaffPolicy")]
        [SwaggerOperation(Summary = "Get pending approval comments")]
        [SwaggerResponse(200, "List of pending approval comments", typeof(List<QuestionCommentResponse>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden - moderator/admin role required")]
        public async Task<IActionResult> GetPendingApprovalComments()
        {
            try
            {
                var result = await _serviceProvider.QuestionCommentService.GetPendingApprovalCommentsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending approval comments");
                return StatusCode(500, "An error occurred while retrieving pending approval comments.");
            }
        }

        /// <summary>
        /// Get userId from JWT token (needs implementation)
        /// </summary>
        /// <returns>User ID or null</returns>
        private int? GetCurrentUserId()
        {
            if (User?.Identity?.IsAuthenticated != true)
                return null;

            //get userid claim from token
            var userIdClaim = User.FindFirst("id");
            if (userIdClaim == null)
                return null;

            if (int.TryParse(userIdClaim.Value, out int userId))
                return userId;

            return null;
        }

        private int? GetCurrentUserRoleId()
        {
            if (User?.Identity?.IsAuthenticated != true)
                return null;

            var roleIdClaim = User.FindFirst("roleId"); 
            if (roleIdClaim == null)
                return null;

            if (int.TryParse(roleIdClaim.Value, out int roleId))
                return roleId;

            return null;
        }
    }
}
