using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using teamseven.EzExam.Services.Object.Requests;

using teamseven.EzExam.Services.Services.UserService;
using teamseven.EzExam.Services.Services.ServiceProvider;
using Swashbuckle.AspNetCore.Annotations;

namespace teamseven.EzExam.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;

        public AccountController(IServiceProviders serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        [HttpGet]
        [Authorize(Policy = "DeliveringStaffPolicy")]
        public async Task<IActionResult> GetAllUser()
        {
            return Ok(await _serviceProvider.UserService.GetUsersAsync());

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
        [AllowAnonymous]
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
        [HttpPut("{id}/profile")]
        [AllowAnonymous]
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
