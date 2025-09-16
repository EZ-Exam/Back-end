using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.UsageTrackingService;

namespace teamseven.EzExam.Controllers
{
    [ApiController]
    [Route("api/usage-tracking")]
    [Produces("application/json")]
    [Authorize] // Require authentication for all endpoints
    public class UsageTrackingController : ControllerBase
    {
        private readonly IUsageTrackingService _usageTrackingService;
        private readonly ILogger<UsageTrackingController> _logger;

        public UsageTrackingController(
            IUsageTrackingService usageTrackingService,
            ILogger<UsageTrackingController> logger)
        {
            _usageTrackingService = usageTrackingService;
            _logger = logger;
        }

        [HttpGet("status/{userId}")]
        [SwaggerOperation(Summary = "Get user subscription status", Description = "Retrieves the current subscription status and usage limits for a user.")]
        [SwaggerResponse(200, "Subscription status retrieved successfully.", typeof(UserSubscriptionStatusResponse))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetUserSubscriptionStatus(int userId)
        {
            try
            {
                var status = await _usageTrackingService.GetUserSubscriptionStatusAsync(userId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription status for user {UserId}: {Message}", userId, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving subscription status." });
            }
        }

        [HttpGet("tracking/{userId}")]
        [SwaggerOperation(Summary = "Get user usage tracking", Description = "Retrieves the usage tracking data for a user.")]
        [SwaggerResponse(200, "Usage tracking retrieved successfully.", typeof(IEnumerable<UsageTrackingResponse>))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetUserUsageTracking(int userId)
        {
            try
            {
                var tracking = await _usageTrackingService.GetUserUsageTrackingAsync(userId);
                return Ok(tracking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving usage tracking for user {UserId}: {Message}", userId, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving usage tracking." });
            }
        }

        [HttpGet("history/{userId}")]
        [SwaggerOperation(Summary = "Get user usage history", Description = "Retrieves the usage history for a user.")]
        [SwaggerResponse(200, "Usage history retrieved successfully.", typeof(IEnumerable<UsageHistoryResponse>))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetUserUsageHistory(int userId, [FromQuery] int? limit = null)
        {
            try
            {
                var history = await _usageTrackingService.GetUserUsageHistoryAsync(userId, limit);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving usage history for user {UserId}: {Message}", userId, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving usage history." });
            }
        }

        [HttpPost("increment")]
        [SwaggerOperation(Summary = "Increment usage", Description = "Increments the usage count for a specific action.")]
        [SwaggerResponse(200, "Usage incremented successfully.", typeof(object))]
        [SwaggerResponse(400, "Invalid request data or limit exceeded.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> IncrementUsage([FromBody] UsageTrackingRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for usage increment.");
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _usageTrackingService.IncrementUsageAsync(request);
                if (result)
                {
                    return Ok(new { Message = "Usage incremented successfully." });
                }
                return BadRequest(new { Message = "Cannot increment usage. Limit exceeded or invalid request." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing usage: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while incrementing usage." });
            }
        }

        [HttpPost("solution-view/{userId}/{solutionId}")]
        [SwaggerOperation(Summary = "Check and increment solution view", Description = "Checks if user can view solution and increments the view count.")]
        [SwaggerResponse(200, "Solution view incremented successfully.", typeof(object))]
        [SwaggerResponse(400, "Cannot view solution. Limit exceeded.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> CheckAndIncrementSolutionView(int userId, int solutionId)
        {
            try
            {
                var result = await _usageTrackingService.CheckAndIncrementSolutionViewAsync(userId, solutionId);
                if (result)
                {
                    return Ok(new { Message = "Solution view incremented successfully." });
                }
                return BadRequest(new { Message = "Cannot view solution. Daily limit exceeded." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking and incrementing solution view for user {UserId}, solution {SolutionId}: {Message}", 
                    userId, solutionId, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while processing solution view." });
            }
        }

        [HttpPost("ai-request/{userId}")]
        [SwaggerOperation(Summary = "Check and increment AI request", Description = "Checks if user can use AI and increments the request count.")]
        [SwaggerResponse(200, "AI request incremented successfully.", typeof(object))]
        [SwaggerResponse(400, "Cannot use AI. Limit exceeded or not enabled.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> CheckAndIncrementAIRequest(int userId, [FromBody] string? description = null)
        {
            try
            {
                var result = await _usageTrackingService.CheckAndIncrementAIRequestAsync(userId, description);
                if (result)
                {
                    return Ok(new { Message = "AI request incremented successfully." });
                }
                return BadRequest(new { Message = "Cannot use AI. Daily limit exceeded or AI not enabled for your subscription." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking and incrementing AI request for user {UserId}: {Message}", 
                    userId, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while processing AI request." });
            }
        }

        [HttpGet("can-perform/{userId}/{actionType}")]
        [SwaggerOperation(Summary = "Check if user can perform action", Description = "Checks if a user can perform a specific action based on their subscription limits.")]
        [SwaggerResponse(200, "Action permission checked successfully.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> CanUserPerformAction(int userId, string actionType)
        {
            try
            {
                var canPerform = await _usageTrackingService.CanUserPerformActionAsync(userId, actionType);
                return Ok(new { CanPerform = canPerform, Message = canPerform ? "Action allowed" : "Action not allowed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking action permission for user {UserId}, action {ActionType}: {Message}", 
                    userId, actionType, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while checking action permission." });
            }
        }

        [HttpPatch("reset/{userId}/{usageType}")]
        [Authorize(Roles = "Admin")] // Only admins can reset usage
        [SwaggerOperation(Summary = "Reset user usage", Description = "Resets the usage count for a specific user and usage type. Admin access required.")]
        [SwaggerResponse(200, "Usage reset successfully.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> ResetUserUsage(int userId, string usageType)
        {
            try
            {
                var result = await _usageTrackingService.ResetUserUsageAsync(userId, usageType);
                if (result)
                {
                    return Ok(new { Message = $"Usage reset successfully for user {userId}, type {usageType}." });
                }
                return StatusCode(500, new { Message = "Failed to reset usage." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting usage for user {UserId}, type {UsageType}: {Message}", 
                    userId, usageType, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while resetting usage." });
            }
        }
    }
}
