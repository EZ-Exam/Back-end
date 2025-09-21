using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.ServiceProvider;
using teamseven.EzExam.Services.Services.SubscriptionTypeService;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/subscription-types")]
    [Produces("application/json")]
    [Authorize] // Require authentication for all endpoints
    public class SubscriptionTypeController : ControllerBase
    {
        private readonly ISubscriptionTypeService _subscriptionTypeService;
        private readonly ILogger<SubscriptionTypeController> _logger;

        public SubscriptionTypeController(
            ISubscriptionTypeService subscriptionTypeService,
            ILogger<SubscriptionTypeController> logger)
        {
            _subscriptionTypeService = subscriptionTypeService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous] // Allow anonymous access for public subscription types
        [SwaggerOperation(Summary = "Get all subscription types", Description = "Retrieves all subscription types including inactive ones.")]
        [SwaggerResponse(200, "Subscription types retrieved successfully.", typeof(IEnumerable<SubscriptionTypeResponse>))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetAllSubscriptionTypes()
        {
            try
            {
                var subscriptionTypes = await _subscriptionTypeService.GetAllSubscriptionTypesAsync();
                return Ok(subscriptionTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all subscription types: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving subscription types." });
            }
        }

        [HttpGet("active")]
        [AllowAnonymous] // Allow anonymous access for active subscription types
        [SwaggerOperation(Summary = "Get active subscription types", Description = "Retrieves only active subscription types.")]
        [SwaggerResponse(200, "Active subscription types retrieved successfully.", typeof(IEnumerable<SubscriptionTypeResponse>))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetActiveSubscriptionTypes()
        {
            try
            {
                var subscriptionTypes = await _subscriptionTypeService.GetActiveSubscriptionTypesAsync();
                return Ok(subscriptionTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription types: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving active subscription types." });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous] // Allow anonymous access for individual subscription types
        [SwaggerOperation(Summary = "Get subscription type by ID", Description = "Retrieves a specific subscription type by its ID.")]
        [SwaggerResponse(200, "Subscription type retrieved successfully.", typeof(SubscriptionTypeResponse))]
        [SwaggerResponse(404, "Subscription type not found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetSubscriptionTypeById(int id)
        {
            try
            {
                var subscriptionType = await _subscriptionTypeService.GetSubscriptionTypeByIdAsync(id);
                return Ok(subscriptionType);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Subscription type with ID {Id} not found.", id);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription type with ID {Id}: {Message}", id, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving the subscription type." });
            }
        }

        [HttpGet("code/{subscriptionCode}")]
        [AllowAnonymous] // Allow anonymous access for subscription type by code
        [SwaggerOperation(Summary = "Get subscription type by code", Description = "Retrieves a specific subscription type by its code.")]
        [SwaggerResponse(200, "Subscription type retrieved successfully.", typeof(SubscriptionTypeResponse))]
        [SwaggerResponse(404, "Subscription type not found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetSubscriptionTypeByCode(string subscriptionCode)
        {
            try
            {
                var subscriptionType = await _subscriptionTypeService.GetSubscriptionTypeByCodeAsync(subscriptionCode);
                return Ok(subscriptionType);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid subscription code: {Code}", subscriptionCode);
                return BadRequest(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Subscription type with code '{Code}' not found.", subscriptionCode);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription type with code '{Code}': {Message}", subscriptionCode, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving the subscription type." });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Only admins can create subscription types
        [SwaggerOperation(Summary = "Create subscription type", Description = "Creates a new subscription type. Admin access required.")]
        [SwaggerResponse(201, "Subscription type created successfully.", typeof(SubscriptionTypeResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(409, "Subscription code already exists.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> CreateSubscriptionType([FromBody] SubscriptionTypeRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for subscription type creation.");
                return BadRequest(ModelState);
            }

            try
            {
                var subscriptionType = await _subscriptionTypeService.CreateSubscriptionTypeAsync(request);
                return CreatedAtAction(nameof(GetSubscriptionTypeById), new { id = subscriptionType.Id }, subscriptionType);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Subscription code conflict: {Message}", ex.Message);
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription type: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while creating the subscription type." });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Only admins can update subscription types
        [SwaggerOperation(Summary = "Update subscription type", Description = "Updates an existing subscription type. Admin access required.")]
        [SwaggerResponse(200, "Subscription type updated successfully.", typeof(SubscriptionTypeResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(404, "Subscription type not found.", typeof(object))]
        [SwaggerResponse(409, "Subscription code already exists.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> UpdateSubscriptionType(int id, [FromBody] SubscriptionTypeRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for subscription type update.");
                return BadRequest(ModelState);
            }

            try
            {
                var subscriptionType = await _subscriptionTypeService.UpdateSubscriptionTypeAsync(id, request);
                return Ok(subscriptionType);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Subscription type with ID {Id} not found for update.", id);
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Subscription code conflict during update: {Message}", ex.Message);
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription type with ID {Id}: {Message}", id, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while updating the subscription type." });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only admins can delete subscription types
        [SwaggerOperation(Summary = "Delete subscription type", Description = "Deletes a subscription type. Admin access required.")]
        [SwaggerResponse(200, "Subscription type deleted successfully.", typeof(object))]
        [SwaggerResponse(404, "Subscription type not found.", typeof(object))]
        [SwaggerResponse(409, "Cannot delete subscription type with active subscriptions.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> DeleteSubscriptionType(int id)
        {
            try
            {
                var result = await _subscriptionTypeService.DeleteSubscriptionTypeAsync(id);
                if (result)
                {
                    return Ok(new { Message = "Subscription type deleted successfully." });
                }
                return StatusCode(500, new { Message = "Failed to delete subscription type." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Subscription type with ID {Id} not found for deletion.", id);
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot delete subscription type with ID {Id}: {Message}", id, ex.Message);
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscription type with ID {Id}: {Message}", id, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while deleting the subscription type." });
            }
        }

        [HttpPatch("{id}/activate")]
        [Authorize(Roles = "Admin")] // Only admins can activate subscription types
        [SwaggerOperation(Summary = "Activate subscription type", Description = "Activates a subscription type. Admin access required.")]
        [SwaggerResponse(200, "Subscription type activated successfully.", typeof(object))]
        [SwaggerResponse(404, "Subscription type not found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> ActivateSubscriptionType(int id)
        {
            try
            {
                var result = await _subscriptionTypeService.ActivateSubscriptionTypeAsync(id);
                if (result)
                {
                    return Ok(new { Message = "Subscription type activated successfully." });
                }
                return StatusCode(500, new { Message = "Failed to activate subscription type." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Subscription type with ID {Id} not found for activation.", id);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating subscription type with ID {Id}: {Message}", id, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while activating the subscription type." });
            }
        }

        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "Admin")] // Only admins can deactivate subscription types
        [SwaggerOperation(Summary = "Deactivate subscription type", Description = "Deactivates a subscription type. Admin access required.")]
        [SwaggerResponse(200, "Subscription type deactivated successfully.", typeof(object))]
        [SwaggerResponse(404, "Subscription type not found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> DeactivateSubscriptionType(int id)
        {
            try
            {
                var result = await _subscriptionTypeService.DeactivateSubscriptionTypeAsync(id);
                if (result)
                {
                    return Ok(new { Message = "Subscription type deactivated successfully." });
                }
                return StatusCode(500, new { Message = "Failed to deactivate subscription type." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Subscription type with ID {Id} not found for deactivation.", id);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating subscription type with ID {Id}: {Message}", id, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while deactivating the subscription type." });
            }
        }

        [HttpGet("check-code/{subscriptionCode}")]
        [Authorize(Roles = "Admin")] // Only admins can check code existence
        [SwaggerOperation(Summary = "Check subscription code existence", Description = "Checks if a subscription code already exists. Admin access required.")]
        [SwaggerResponse(200, "Code existence checked successfully.", typeof(object))]
        [SwaggerResponse(400, "Invalid subscription code.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> CheckSubscriptionCodeExists(string subscriptionCode, [FromQuery] int? excludeId = null)
        {
            try
            {
                var exists = await _subscriptionTypeService.IsSubscriptionCodeExistsAsync(subscriptionCode, excludeId);
                return Ok(new { Exists = exists, Message = exists ? "Code already exists" : "Code is available" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid subscription code: {Code}", subscriptionCode);
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking subscription code existence: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while checking subscription code existence." });
            }
        }
    }
}
