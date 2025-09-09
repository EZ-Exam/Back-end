using Microsoft.AspNetCore.Mvc;
using teamseven.EzExam.Services.Interfaces;
using teamseven.EzExam.Services.Object.Requests;
using Swashbuckle.AspNetCore.Annotations;
using teamseven.EzExam.Services.Services.ServiceProvider;

namespace teamseven.EzExam.API.Controllers
{
    [Route("api/signup")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
       private readonly IServiceProviders _serviceProvider;

        public RegisterController(
            IServiceProviders serviceProvider )
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Register a new user and return JWT token.
        /// </summary>
        [HttpPost]
        [SwaggerOperation(
            Summary = "User registration",
            Description = "Registers a new user, authenticates, and returns a JWT token."
        )]
        public async Task<IActionResult> SignUp([FromBody] RegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Request body is invalid or missing." });
            }

            //if (_serviceProvider?.RegisterService == null)
            //{
            //    return StatusCode(500, new { message = "Service provider or RegisterService is not initialized." });
            //}

            var (isRegisterSuccess, registerError) = await _serviceProvider.RegisterService.RegisterUserAsync(request);
            if (!isRegisterSuccess)
            {
                return BadRequest(new { message = registerError });
            }

            return Ok(new { message = "Registration successful. Please login again." });
        }
        /// <summary>
        /// Change user role.
        /// </summary>
        [HttpPost("role")]
        [SwaggerOperation(
            Summary = "Change user role",
            Description = "Changes the role of a user if the correct secret key is provided."
        )]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleRequest request)
        {
            // Validation co b?n b?i Data Annotations và [ApiController]
            var (isSuccess, resultOrError) = await _serviceProvider.RegisterService.ChangeUserRoleAsync(request.UserId, request.RoleName, request.SecretKey);
            if (!isSuccess)
            {
                return BadRequest(new { message = resultOrError });
            }

            return Ok(new { message = resultOrError });
        }
    }
}
