using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.UsageTrackingService;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/usage-tracking")]
    [Produces("application/json")]
    [Authorize] 
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

        [HttpGet("users/{userId}/subscription-status")]
        [SwaggerOperation(Summary = "Get user subscription status", Description = "Retrieves the current subscription status and usage limits for a user.")]
        [SwaggerResponse(200, "Subscription status retrieved successfully.", typeof(UserSubscriptionStatusResponse))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        public async Task<IActionResult> GetUserSubscriptionStatus(int userId)
        {
            var status = await _usageTrackingService.GetUserSubscriptionStatusAsync(userId);
            return Ok(status);
        }

        [HttpGet("users/{userId}")]
        [SwaggerOperation(Summary = "Get user usage tracking", Description = "Retrieves the usage tracking data for a user.")]
        [SwaggerResponse(200, "Usage tracking retrieved successfully.", typeof(IEnumerable<UsageTrackingResponse>))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        public async Task<IActionResult> GetUserUsageTracking(int userId)
        {
            var tracking = await _usageTrackingService.GetUserUsageTrackingAsync(userId);
            return Ok(tracking);
        }

        [HttpGet("users/{userId}/history")]
        [SwaggerOperation(Summary = "Get user usage history", Description = "Retrieves the usage history for a user.")]
        [SwaggerResponse(200, "Usage history retrieved successfully.", typeof(IEnumerable<UsageHistoryResponse>))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        public async Task<IActionResult> GetUserUsageHistory(int userId, [FromQuery] int? limit = null)
        {
            var history = await _usageTrackingService.GetUserUsageHistoryAsync(userId, limit);
            return Ok(history);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Increment usage", Description = "Increments the usage count for a specific action.")]
        [SwaggerResponse(200, "Usage incremented successfully.", typeof(object))]
        [SwaggerResponse(400, "Invalid request data or limit exceeded.", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        public async Task<IActionResult> IncrementUsage([FromBody] UsageTrackingRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for usage increment.");
                return BadRequest(ModelState);
            }

            var result = await _usageTrackingService.IncrementUsageAsync(request);
            if (result)
                return Ok(new { Message = "Usage incremented successfully." });

            return BadRequest(new { Message = "Cannot increment usage. Limit exceeded or invalid request." });
        }

        [HttpPost("solutions/{solutionId}/views")]
        [SwaggerOperation(Summary = "Check and increment solution view", Description = "Checks if user can view solution and increments the view count.")]
        [SwaggerResponse(200, "Solution view incremented successfully.", typeof(object))]
        [SwaggerResponse(400, "Cannot view solution. Limit exceeded.", typeof(object))]
        public async Task<IActionResult> CheckAndIncrementSolutionView(int userId, int solutionId)
        {
            var result = await _usageTrackingService.CheckAndIncrementSolutionViewAsync(userId, solutionId);
            if (result)
                return Ok(new { Message = "Solution view incremented successfully." });

            return BadRequest(new { Message = "Cannot view solution. Daily limit exceeded." });
        }

        [HttpPost("ai-requests")]
        [SwaggerOperation(Summary = "Check and increment AI request", Description = "Checks if user can use AI and increments the request count.")]
        [SwaggerResponse(200, "AI request incremented successfully.", typeof(object))]
        [SwaggerResponse(400, "Cannot use AI. Limit exceeded or not enabled.", typeof(object))]
        public async Task<IActionResult> CheckAndIncrementAIRequest(int userId, [FromBody] string? description = null)
        {
            var result = await _usageTrackingService.CheckAndIncrementAIRequestAsync(userId, description);
            if (result)
                return Ok(new { Message = "AI request incremented successfully." });

            return BadRequest(new { Message = "Cannot use AI. Daily limit exceeded or AI not enabled for your subscription." });
        }

        [HttpGet("users/{userId}/permissions/{actionType}")]
        [SwaggerOperation(Summary = "Check if user can perform action", Description = "Checks if a user can perform a specific action based on their subscription limits.")]
        [SwaggerResponse(200, "Action permission checked successfully.", typeof(object))]
        public async Task<IActionResult> CanUserPerformAction(int userId, string actionType)
        {
            var canPerform = await _usageTrackingService.CanUserPerformActionAsync(userId, actionType);
            return Ok(new { CanPerform = canPerform, Message = canPerform ? "Action allowed" : "Action not allowed" });
        }

        [HttpPatch("users/{userId}/usage/{usageType}/reset")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Reset user usage", Description = "Resets the usage count for a specific user and usage type. Admin access required.")]
        [SwaggerResponse(204, "Usage reset successfully.")]
        [SwaggerResponse(404, "No active subscription found.", typeof(object))]
        public async Task<IActionResult> ResetUserUsage(int userId, string usageType)
        {
            await _usageTrackingService.ResetUserUsageAsync(userId, usageType);
            return NoContent();
        }
    }
}
