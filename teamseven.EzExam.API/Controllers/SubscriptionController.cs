using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.JwtHelperService;
using teamseven.EzExam.Services.Services.SubscriptionService;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/subscriptions")]
    [Produces("application/json")]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<SubscriptionController> _logger;
        private readonly IJwtHelperService _jwtHelperService;

        public SubscriptionController(
            ISubscriptionService subscriptionService,
            ILogger<SubscriptionController> logger,
            IJwtHelperService jwtHelperService)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
            _jwtHelperService = jwtHelperService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Subscribe to a subscription plan", Description = "Subscribe the current user to a subscription plan.")]
        [SwaggerResponse(200, "Subscription successful.", typeof(SubscribeResponse))]
        [SwaggerResponse(400, "Invalid request data or subscription logic violation.", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        [SwaggerResponse(404, "User or subscription type not found.", typeof(object))]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for subscribe request.");
                return BadRequest(ModelState);
            }

            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            var currentUserId = _jwtHelperService.GetCurrentUserIdFromToken(authHeader);
            if (currentUserId == null)
            {
                _logger.LogWarning("Could not extract user ID from JWT token.");
                return Unauthorized(new { Message = "Invalid or missing user information in token." });
            }

            var result = await _subscriptionService.SubscribeUserAsync(currentUserId.Value, request);
            return Ok(result);
        }

        [HttpGet("me")]
        [SwaggerOperation(Summary = "Get current subscription", Description = "Retrieves the current active subscription for the authenticated user.")]
        [SwaggerResponse(200, "Current subscription retrieved successfully.", typeof(SubscribeResponse))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        [SwaggerResponse(404, "No active subscription found.", typeof(object))]
        public async Task<IActionResult> GetCurrentSubscription()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            var currentUserId = _jwtHelperService.GetCurrentUserIdFromToken(authHeader);
            if (currentUserId == null)
            {
                _logger.LogWarning("Could not extract user ID from JWT token.");
                return Unauthorized(new { Message = "Invalid or missing user information in token." });
            }

            var result = await _subscriptionService.GetUserCurrentSubscriptionAsync(currentUserId.Value);
            return Ok(result);
        }

        [HttpGet("me/history")]
        [SwaggerOperation(Summary = "Get subscription history", Description = "Retrieves the subscription history for the authenticated user.")]
        [SwaggerResponse(200, "Subscription history retrieved successfully.", typeof(List<SubscribeResponse>))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        public async Task<IActionResult> GetSubscriptionHistory()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            var currentUserId = _jwtHelperService.GetCurrentUserIdFromToken(authHeader);
            if (currentUserId == null)
            {
                _logger.LogWarning("Could not extract user ID from JWT token.");
                return Unauthorized(new { Message = "Invalid or missing user information in token." });
            }

            var result = await _subscriptionService.GetUserSubscriptionHistoryAsync(currentUserId.Value);
            return Ok(result);
        }

        [HttpDelete("me")]
        [SwaggerOperation(Summary = "Cancel current subscription", Description = "Cancels the current active subscription for the authenticated user.")]
        [SwaggerResponse(200, "Subscription cancelled successfully.", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        [SwaggerResponse(404, "No active subscription found.", typeof(object))]
        public async Task<IActionResult> CancelSubscription()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            var currentUserId = _jwtHelperService.GetCurrentUserIdFromToken(authHeader);
            if (currentUserId == null)
            {
                _logger.LogWarning("Could not extract user ID from JWT token.");
                return Unauthorized(new { Message = "Invalid or missing user information in token." });
            }

            var result = await _subscriptionService.CancelUserSubscriptionAsync(currentUserId.Value);
            if (!result)
            {
                return NotFound(new { Message = "No active subscription found to cancel." });
            }

            return Ok(new { Message = "Subscription cancelled successfully." });
        }
    }
}

