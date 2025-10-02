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
        /// Tạo comment mới cho câu hỏi
        /// </summary>
        /// <param name="request">Thông tin comment</param>
        /// <returns>Comment đã được tạo</returns>
        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Tạo comment mới cho câu hỏi")]
        [SwaggerResponse(200, "Comment được tạo thành công", typeof(QuestionCommentResponse))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ")]
        [SwaggerResponse(401, "Chưa đăng nhập")]
        [SwaggerResponse(404, "Không tìm thấy câu hỏi hoặc user")]
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
        /// Cập nhật comment
        /// </summary>
        /// <param name="id">ID của comment</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Comment đã được cập nhật</returns>
        [HttpPut]
        [Authorize]
        [SwaggerOperation(Summary = "Cập nhật comment")]
        [SwaggerResponse(200, "Comment được cập nhật thành công", typeof(QuestionCommentResponse))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ")]
        [SwaggerResponse(401, "Chưa đăng nhập hoặc không có quyền")]
        [SwaggerResponse(404, "Không tìm thấy comment")]
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
        /// Xóa comment
        /// </summary>
        /// <param name="id">ID của comment</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Xóa comment")]
        [SwaggerResponse(200, "Comment được xóa thành công")]
        [SwaggerResponse(401, "Chưa đăng nhập hoặc không có quyền")]
        [SwaggerResponse(404, "Không tìm thấy comment")]
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

                await _serviceProvider.QuestionCommentService.DeleteCommentAsync(id, userId.Value, roleId.Value);
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
        /// Lấy danh sách comment theo question ID (flat structure)
        /// </summary>
        /// <param name="questionId">ID của câu hỏi</param>
        /// <returns>Danh sách comment với replies</returns>
        [HttpGet("by-question/{questionId}")]
        [SwaggerOperation(Summary = "Lấy danh sách comment theo question ID")]
        [SwaggerResponse(200, "Danh sách comment", typeof(List<QuestionCommentResponse>))]
        [SwaggerResponse(404, "Không tìm thấy câu hỏi")]
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
        /// Lấy danh sách comment ID có thể dùng làm ParentCommentId cho một question
        /// </summary>
        /// <param name="questionId">ID của câu hỏi</param>
        /// <returns>Danh sách comment ID có thể reply</returns>
        [HttpGet("available-parents/{questionId}")]
        [SwaggerOperation(Summary = "Lấy danh sách comment ID có thể dùng làm ParentCommentId")]
        [SwaggerResponse(200, "Danh sách comment ID", typeof(List<object>))]
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
        /// Lấy comment theo ID
        /// </summary>
        /// <param name="id">ID của comment</param>
        /// <returns>Comment</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy comment theo ID")]
        [SwaggerResponse(200, "Comment", typeof(QuestionCommentResponse))]
        [SwaggerResponse(404, "Không tìm thấy comment")]
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
        /// Duyệt comment (chỉ moderator/admin)
        /// </summary>
        /// <param name="id">ID của comment</param>
        /// <param name="request">Thông tin duyệt</param>
        /// <returns>Comment đã được duyệt</returns>
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Moderator,Admin")]
        [SwaggerOperation(Summary = "Duyệt comment (chỉ moderator/admin)")]
        [SwaggerResponse(200, "Comment được duyệt thành công", typeof(QuestionCommentResponse))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ")]
        [SwaggerResponse(401, "Chưa đăng nhập")]
        [SwaggerResponse(403, "Không có quyền moderator/admin")]
        [SwaggerResponse(404, "Không tìm thấy comment")]
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
        /// Lấy danh sách comment chờ duyệt (chỉ moderator/admin)
        /// </summary>
        /// <returns>Danh sách comment chờ duyệt</returns>
        [HttpGet("pending-approval")]
        [Authorize(Roles = "Moderator,Admin")]
        [SwaggerOperation(Summary = "Lấy danh sách comment chờ duyệt (chỉ moderator/admin)")]
        [SwaggerResponse(200, "Danh sách comment chờ duyệt", typeof(List<QuestionCommentResponse>))]
        [SwaggerResponse(401, "Chưa đăng nhập")]
        [SwaggerResponse(403, "Không có quyền moderator/admin")]
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
        /// Lấy userId từ JWT token (cần implement)
        /// </summary>
        /// <returns>User ID hoặc null</returns>
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
