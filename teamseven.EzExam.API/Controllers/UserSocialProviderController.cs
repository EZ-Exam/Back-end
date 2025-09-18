using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.ServiceProvider;

namespace teamseven.EzExam.Controllers
{
    [ApiController]
    [Route("api/user-social-providers")]
    [Produces("application/json")]
    public class UserSocialProviderController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;
        private readonly ILogger<UserSocialProviderController> _logger;

        public UserSocialProviderController(IServiceProviders serviceProvider, ILogger<UserSocialProviderController> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all user social providers", Description = "Retrieves a list of all user social providers.")]
        [SwaggerResponse(200, "User social providers retrieved successfully.", typeof(IEnumerable<UserSocialProviderDataResponse>))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetAll()
        {
            var result = await _serviceProvider.UserSocialProviderService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get a user social provider by ID", Description = "Retrieves a specific user social provider by their ID.")]
        [SwaggerResponse(200, "User social provider retrieved successfully.", typeof(UserSocialProviderDataResponse))]
        [SwaggerResponse(404, "User social provider not found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _serviceProvider.UserSocialProviderService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "DeliveringStaffPolicy")]
        [SwaggerOperation(Summary = "Create a new user social provider", Description = "Creates a new user social provider. Requires DeliveringStaffPolicy authorization.")]
        [SwaggerResponse(201, "User social provider created successfully.", typeof(object))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid token or insufficient permissions.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> Create([FromBody] CreateUserSocialProviderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceProvider.UserSocialProviderService.CreateAsync(request);
            return StatusCode(201, new { Message = "User social provider created successfully." });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "DeliveringStaffPolicy")]
        [SwaggerOperation(Summary = "Update an existing user social provider", Description = "Updates an existing user social provider. Requires DeliveringStaffPolicy authorization.")]
        [SwaggerResponse(200, "User social provider updated successfully.", typeof(object))]
        [SwaggerResponse(400, "Invalid request data or ID mismatch.", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid token or insufficient permissions.", typeof(object))]
        [SwaggerResponse(404, "User social provider not found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> Update(int id, [FromBody] UserSocialProviderDataRequest request)
        {
            if (!ModelState.IsValid || id != request.Id)
                return BadRequest(new { Message = "Invalid data or ID mismatch." });

            try
            {
                await _serviceProvider.UserSocialProviderService.UpdateAsync(request);
                return Ok(new { Message = "User social provider updated successfully." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeliveringStaffPolicy")]
        [SwaggerOperation(Summary = "Delete a user social provider")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _serviceProvider.UserSocialProviderService.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
