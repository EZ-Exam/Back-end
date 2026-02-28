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
    [Authorize]
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
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all subscription types", Description = "Retrieves all subscription types including inactive ones.")]
        [SwaggerResponse(200, "Subscription types retrieved successfully.", typeof(IEnumerable<SubscriptionTypeResponse>))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetAllSubscriptionTypes()
        {
            var subscriptionTypes = await _subscriptionTypeService.GetAllSubscriptionTypesAsync();
            return Ok(subscriptionTypes);
        }

        [HttpGet("active")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get active subscription types", Description = "Retrieves only active subscription types.")]
        [SwaggerResponse(200, "Active subscription types retrieved successfully.", typeof(IEnumerable<SubscriptionTypeResponse>))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetActiveSubscriptionTypes()
        {
            var subscriptionTypes = await _subscriptionTypeService.GetActiveSubscriptionTypesAsync();
            return Ok(subscriptionTypes);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get subscription type by ID", Description = "Retrieves a specific subscription type by its ID.")]
        [SwaggerResponse(200, "Subscription type retrieved successfully.", typeof(SubscriptionTypeResponse))]
        [SwaggerResponse(404, "Subscription type not found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetSubscriptionTypeById(int id)
        {
            var subscriptionType = await _subscriptionTypeService.GetSubscriptionTypeByIdAsync(id);
            return Ok(subscriptionType);
        }

        [HttpGet("code/{subscriptionCode}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get subscription type by code", Description = "Retrieves a specific subscription type by its code.")]
        [SwaggerResponse(200, "Subscription type retrieved successfully.", typeof(SubscriptionTypeResponse))]
        [SwaggerResponse(404, "Subscription type not found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetSubscriptionTypeByCode(string subscriptionCode)
        {
            var subscriptionType = await _subscriptionTypeService.GetSubscriptionTypeByCodeAsync(subscriptionCode);
            return Ok(subscriptionType);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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

            var subscriptionType = await _subscriptionTypeService.CreateSubscriptionTypeAsync(request);
            return CreatedAtAction(nameof(GetSubscriptionTypeById), new { id = subscriptionType.Id }, subscriptionType);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
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

            var subscriptionType = await _subscriptionTypeService.UpdateSubscriptionTypeAsync(id, request);
            return Ok(subscriptionType);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete subscription type", Description = "Deletes a subscription type. Admin access required.")]
        [SwaggerResponse(204, "Subscription type deleted successfully.")]
        [SwaggerResponse(404, "Subscription type not found.", typeof(object))]
        [SwaggerResponse(409, "Cannot delete subscription type with active subscriptions.", typeof(object))]
        public async Task<IActionResult> DeleteSubscriptionType(int id)
        {
            await _subscriptionTypeService.DeleteSubscriptionTypeAsync(id);
            return NoContent();
        }

        [HttpPatch("{id}/activate")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Activate subscription type", Description = "Activates a subscription type. Admin access required.")]
        [SwaggerResponse(200, "Subscription type activated successfully.", typeof(object))]
        [SwaggerResponse(404, "Subscription type not found.", typeof(object))]
        public async Task<IActionResult> ActivateSubscriptionType(int id)
        {
            await _subscriptionTypeService.ActivateSubscriptionTypeAsync(id);
            return Ok(new { Message = "Subscription type activated successfully." });
        }

        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Deactivate subscription type", Description = "Deactivates a subscription type. Admin access required.")]
        [SwaggerResponse(200, "Subscription type deactivated successfully.", typeof(object))]
        [SwaggerResponse(404, "Subscription type not found.", typeof(object))]
        public async Task<IActionResult> DeactivateSubscriptionType(int id)
        {
            await _subscriptionTypeService.DeactivateSubscriptionTypeAsync(id);
            return Ok(new { Message = "Subscription type deactivated successfully." });
        }

        [HttpGet("check-code/{subscriptionCode}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Check subscription code existence", Description = "Checks if a subscription code already exists. Admin access required.")]
        [SwaggerResponse(200, "Code existence checked successfully.", typeof(object))]
        [SwaggerResponse(400, "Invalid subscription code.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> CheckSubscriptionCodeExists(string subscriptionCode, [FromQuery] int? excludeId = null)
        {
            var exists = await _subscriptionTypeService.IsSubscriptionCodeExistsAsync(subscriptionCode, excludeId);
            return Ok(new { Exists = exists, Message = exists ? "Code already exists" : "Code is available" });
        }
    }
}
