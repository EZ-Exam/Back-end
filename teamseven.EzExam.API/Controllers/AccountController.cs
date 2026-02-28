using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.UserService;
using teamseven.EzExam.Services.Services.ServiceProvider;
using teamseven.EzExam.Services.Services.JwtHelperService;
using Swashbuckle.AspNetCore.Annotations;

namespace teamseven.EzExam.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;
        private readonly ILogger<AccountController> _logger;
        private readonly IJwtHelperService _jwtHelperService;

        public AccountController(
            IServiceProviders serviceProvider,
            ILogger<AccountController> logger,
            IJwtHelperService jwtHelperService)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _jwtHelperService = jwtHelperService;
        }

        [HttpGet]
        [Authorize(Policy = "DeliveringStaffPolicy")]
        [SwaggerOperation(Summary = "Get all users", Description = "Retrieves a list of all users. Requires DeliveringStaffPolicy authorization.")]
        [SwaggerResponse(200, "Users retrieved successfully.", typeof(IEnumerable<UserResponse>))]
        [SwaggerResponse(401, "Unauthorized - Invalid token or insufficient permissions.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetAllUser()
        {
            return Ok(await _serviceProvider.UserService.GetUsersAsync());

        }

        [HttpGet("count")]
        [Authorize(Policy = "DeliveringStaffPolicy")]
        [SwaggerOperation(Summary = "Get total user, question, exam count", Description = "Retrieves the total number of users. Requires DeliveringStaffPolicy authorization.")]
        [SwaggerResponse(200, "Total user count retrieved successfully.", typeof(TotalUserResponse))]
        [SwaggerResponse(401, "Unauthorized - Invalid token or insufficient permissions.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetTotalCount()
        {
            var totalCount = await _serviceProvider.UserService.GetTotalUserAsync();
            return Ok(totalCount);
        }

        [HttpGet("me")]
        [Authorize]
        [SwaggerOperation(Summary = "Get current user profile", Description = "Retrieves the profile information of the currently authenticated user.")]
        [SwaggerResponse(200, "Profile retrieved successfully.", typeof(UserResponse))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        [SwaggerResponse(404, "User not found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetMyProfile()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            var currentUserId = _jwtHelperService.GetCurrentUserIdFromToken(authHeader);
            if (currentUserId == null)
            {
                _logger.LogWarning("Could not extract user ID from JWT token.");
                return Unauthorized(new { Message = "Invalid or missing user information in token." });
            }

            var userProfile = await _serviceProvider.UserService.GetMyProfileAsync(currentUserId.Value);
            return Ok(userProfile);
        }

        [HttpPost("{id}/premium")]
        [Authorize]
        [SwaggerOperation(
        Summary = "Upgrade user to premium",
        Description = "If user's balance >= 10000 and user is not already premium, deduct 10000 and upgrade to premium")]
        [SwaggerResponse(200, "Upgraded successfully")]
        [SwaggerResponse(400, "Not enough balance or already premium")]
        [SwaggerResponse(404, "User not found")]
        public async Task<IActionResult> UpgradeToPremium(int id)
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            var currentUserId = _jwtHelperService.GetCurrentUserIdFromToken(authHeader);
            if (currentUserId == null || currentUserId != id)
            {
                return StatusCode(403, new { Message = "Forbidden: Cannot upgrade another user's account." });
            }

            var success = await _serviceProvider.UserService.UpgradeToPremiumAsync(id);

            if (!success)
                return BadRequest(new { Message = "User already premium or insufficient balance." });

            return Ok(new { Message = "User upgraded to premium successfully." });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get user by ID", Description = "Retrieves a specific user by their ID.")]
        [SwaggerResponse(200, "User retrieved successfully.", typeof(UserResponse))]
        [SwaggerResponse(400, "Invalid user ID.", typeof(object))]
        [SwaggerResponse(404, "User not found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetUserById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID");
            }

            var user = await _serviceProvider.UserService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found");
            }
            return Ok(user);
        }
        [HttpDelete("{id}")]
        [Authorize(Policy = "DeliveringStaffPolicy")]
        [SwaggerOperation(Summary = "Soft delete user", Description = "Performs a soft delete on a user by setting their status to inactive.")]
        [SwaggerResponse(200, "User soft deleted successfully.", typeof(UserResponse))]
        [SwaggerResponse(400, "Invalid user ID.", typeof(object))]
        [SwaggerResponse(404, "User not found.", typeof(object))]
        [SwaggerResponse(409, "User already deleted.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> SoftDeleteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID");
            }

            var userDto = await _serviceProvider.UserService.SoftDeleteUserAsync(id);
            return Ok(userDto);
        }

        [HttpPatch("{id}/restore")]
        [Authorize(Policy = "DeliveringStaffPolicy")]
        [SwaggerOperation(Summary = "Restore user", Description = "Restore user by setting their status to active.")]
        [SwaggerResponse(200, "User restored successfully", typeof(UserResponse))]
        [SwaggerResponse(404, "User not found", typeof(object))]
        [SwaggerResponse(400, "Invalid user ID", typeof(object))]
        [SwaggerResponse(409, "User already restored.", typeof(object))]
        [SwaggerResponse(500, "Internal server error", typeof(object))]
        public async Task<IActionResult> RestoreUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID");
            }

            var userDto = await _serviceProvider.UserService.RestoreUserAsync(id);
            return Ok(userDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Update user profile", Description = "Updates the profile information of a specific user.")]
        [SwaggerResponse(200, "Profile updated successfully.", typeof(object))]
        [SwaggerResponse(400, "Invalid user ID or request data.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UpdateUserProfileRequest request)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            var currentUserId = _jwtHelperService.GetCurrentUserIdFromToken(authHeader);
            if (currentUserId == null || currentUserId != id)
            {
                return StatusCode(403, new { Message = "Forbidden: Cannot update another user's profile." });
            }

            var (isSuccess, resultOrError) = await _serviceProvider.UserService.UpdateUserProfileAsync(id, request);
            if (!isSuccess)
            {
                return BadRequest(resultOrError);
            }
            return Ok(new { Message = resultOrError });
        }
    }
}
