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

        [HttpGet("total-count")]
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

        [HttpGet("my-profile")]
        [Authorize] // Require authentication
        [SwaggerOperation(Summary = "Get current user profile", Description = "Retrieves the profile information of the currently authenticated user.")]
        [SwaggerResponse(200, "Profile retrieved successfully.", typeof(UserResponse))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        [SwaggerResponse(404, "User not found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetMyProfile()
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

                var userProfile = await _serviceProvider.UserService.GetMyProfileAsync(currentUserId.Value);
                return Ok(userProfile);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found: {Message}", ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving user profile." });
            }
        }

        [HttpPut("premium/{userId}")]
        [AllowAnonymous]
        [SwaggerOperation(
        Summary = "Upgrade user to premium",
        Description = "If user's balance >= 10000 and user is not already premium, deduct 10000 and upgrade to premium")]
        [SwaggerResponse(200, "Upgraded successfully")]
        [SwaggerResponse(400, "Not enough balance or already premium")]
        [SwaggerResponse(404, "User not found")]
        public async Task<IActionResult> UpgradeToPremium(int userId)
        {
            try
            {
                var success = await _serviceProvider.UserService.UpgradeToPremiumAsync(userId);

                if (!success)
                    return BadRequest(new { Message = "User already premium or insufficient balance." });

                return Ok(new { Message = "User upgraded to premium successfully." });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error" });
            }
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
            try
            {
                var user = await _serviceProvider.UserService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the user");
            }
        }
        [HttpPut("{id}/soft-delete")]
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

            try
            {
                var userDto = await _serviceProvider.UserService.SoftDeleteUserAsync(id);
                return Ok(userDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while soft deleting the user");
            }
        }

        [HttpPut("{id}/restore-user")]
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

            try
            {
                var userDto = await _serviceProvider.UserService.RestoreUserAsync(id);
                return Ok(userDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while restoring the user");
            }
        }


        [HttpPut("{id}/profile")]
        [AllowAnonymous]
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

            try
            {
                var (isSuccess, resultOrError) = await _serviceProvider.UserService.UpdateUserProfileAsync(id, request);
                if (!isSuccess)
                {
                    return BadRequest(resultOrError);
                }
                return Ok(new { Message = resultOrError });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
