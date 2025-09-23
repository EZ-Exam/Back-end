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
    [Route("api/subscription")]
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

        [HttpPost("subscribe")]
        [SwaggerOperation(Summary = "Subscribe to a subscription plan", Description = "Subscribe the current user to a subscription plan. FREE subscription can be overridden by any paid package. Paid subscriptions cannot be overridden by other paid packages.")]
        [SwaggerResponse(200, "Subscription successful.", typeof(SubscribeResponse))]
        [SwaggerResponse(400, "Invalid request data or subscription logic violation.", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        [SwaggerResponse(404, "User or subscription type not found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for subscribe request.");
                return BadRequest(ModelState);
            }

            try
            {
                // Get current user ID from JWT token
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
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User or subscription type not found: {Message}", ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation: {Message}", ex.Message);
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing user: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while subscribing." });
            }
        }

        [HttpGet("current")]
        [SwaggerOperation(Summary = "Get current subscription", Description = "Retrieves the current active subscription for the authenticated user.")]
        [SwaggerResponse(200, "Current subscription retrieved successfully.", typeof(SubscribeResponse))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        [SwaggerResponse(404, "No active subscription found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetCurrentSubscription()
        {
            try
            {
                // Get current user ID from JWT token
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
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "No active subscription found: {Message}", ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current subscription: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving subscription." });
            }
        }

        [HttpGet("history")]
        [SwaggerOperation(Summary = "Get subscription history", Description = "Retrieves the subscription history for the authenticated user.")]
        [SwaggerResponse(200, "Subscription history retrieved successfully.", typeof(List<SubscribeResponse>))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetSubscriptionHistory()
        {
            try
            {
                // Get current user ID from JWT token
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription history: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving subscription history." });
            }
        }


        [HttpPost("cancel")]
        [SwaggerOperation(Summary = "Cancel current subscription", Description = "Cancels the current active subscription for the authenticated user.")]
        [SwaggerResponse(200, "Subscription cancelled successfully.", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        [SwaggerResponse(404, "No active subscription found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> CancelSubscription()
        {
            try
            {
                // Get current user ID from JWT token
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while cancelling subscription." });
            }
        }
    }
}

