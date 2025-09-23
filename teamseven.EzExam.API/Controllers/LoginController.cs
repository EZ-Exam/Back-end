using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Services.ServiceProvider;
using IServiceProviders = teamseven.EzExam.Services.Services.ServiceProvider.IServiceProviders;

namespace teamseven.EzExam.API.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;

        public LoginController(IServiceProviders serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Authenticates a user with email and password and returns a JWT token.
        /// </summary>
        /// <param name="request">The login request containing email and password.</param>
        /// <returns>A JWT token if authentication is successful; otherwise, an error message.</returns>
        [HttpPost]
        [SwaggerOperation(
            Summary = "User login with email and password",
            Description = "Authenticates a user using their email and password and returns a JWT token if successful."
        )]
        [SwaggerResponse(200, "Authentication successful, returns JWT token.", typeof(object))]
        [SwaggerResponse(401, "Authentication failed, invalid credentials.", typeof(object))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Invalid login request. Email and password are required." });
            }

            var (isSuccess, resultOrError) = await _serviceProvider.LoginService.ValidateUserAsync(request);
            if (!isSuccess)
            {
                return Unauthorized(new { message = resultOrError });
            }

            return Ok(new { token = resultOrError, message = "Login successful" });
        }

        /// <summary>
        /// Authenticates a user using Google OAuth and returns a JWT token.
        /// </summary>
        /// <param name="request">The Google token request containing the Google ID token.</param>
        /// <returns>A JWT token if authentication is successful; otherwise, an error message.</returns>
        [HttpPost("google-login")]
        [RequestSizeLimit(1_048_576)]
        [SwaggerOperation(
            Summary = "User login with Google OAuth",
            Description = "Authenticates a user using a Google ID token and returns a JWT token if successful. If the user does not exist, a new account is created."
        )]
        [SwaggerResponse(200, "Authentication successful, returns JWT token.", typeof(object))]
        [SwaggerResponse(401, "Authentication failed, invalid Google token.", typeof(object))]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleTokenRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.credential))
            {
                return BadRequest(new { message = "Invalid Google token request. Credential is required." });
            }

            try
            {
                var token = await _serviceProvider.AuthService.GoogleLoginAsync(request.credential);
                return Ok(new { token, message = "Google login successful" });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Google authentication failed.", error = ex.Message });
            }
        }
    }
}
