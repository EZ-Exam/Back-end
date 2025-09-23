using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.BalanceService;
using teamseven.EzExam.Services.Services.JwtHelperService;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/balance")]
    [Produces("application/json")]
    [Authorize] 
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceService _balanceService;
        private readonly ILogger<BalanceController> _logger;
        private readonly IJwtHelperService _jwtHelperService;

        public BalanceController(
            IBalanceService balanceService,
            ILogger<BalanceController> logger,
            IJwtHelperService jwtHelperService)
        {
            _balanceService = balanceService;
            _logger = logger;
            _jwtHelperService = jwtHelperService;
        }

        [HttpPost("add")]
        [SwaggerOperation(Summary = "Add balance to current user", Description = "Adds balance to the current authenticated user's account.")]
        [SwaggerResponse(200, "Balance added successfully.", typeof(BalanceResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid token or super secret key.", typeof(object))]
        [SwaggerResponse(404, "User not found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> AddBalance([FromBody] AddBalanceRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for add balance request.");
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

                var result = await _balanceService.AddBalanceAsync(currentUserId.Value, request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt: {Message}", ex.Message);
                return Unauthorized(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found: {Message}", ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding balance: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while adding balance." });
            }
        }

        [HttpGet("current")]
        [SwaggerOperation(Summary = "Get current user balance", Description = "Retrieves the current balance of the authenticated user.")]
        [SwaggerResponse(200, "Balance retrieved successfully.", typeof(BalanceResponse))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        [SwaggerResponse(404, "User not found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetCurrentBalance()
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

                var balanceInfo = await _balanceService.GetUserBalanceInfoAsync(currentUserId.Value);
                return Ok(balanceInfo);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found: {Message}", ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current balance: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving balance." });
            }
        }

        [HttpGet("amount")]
        [SwaggerOperation(Summary = "Get current user balance amount", Description = "Retrieves only the balance amount of the authenticated user.")]
        [SwaggerResponse(200, "Balance amount retrieved successfully.", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        [SwaggerResponse(404, "User not found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> GetBalanceAmount()
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

                var balance = await _balanceService.GetUserBalanceAsync(currentUserId.Value);
                return Ok(new { Balance = balance });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found: {Message}", ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting balance amount: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving balance amount." });
            }
        }

        [HttpPost("master-deposit")]
        [AllowAnonymous] 
        [SwaggerOperation(Summary = "Master deposit to any user account", Description = "Deposits balance to any user account using master super secret key. This is an admin-only operation.")]
        [SwaggerResponse(200, "Deposit successful.", typeof(BalanceResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid super secret key.", typeof(object))]
        [SwaggerResponse(404, "User not found.", typeof(object))]
        [SwaggerResponse(500, "Internal server error.", typeof(object))]
        public async Task<IActionResult> MasterDeposit([FromBody] MasterDepositRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for master deposit request.");
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _balanceService.MasterDepositAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt for master deposit: {Message}", ex.Message);
                return Unauthorized(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found for master deposit: {Message}", ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in master deposit: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while processing master deposit." });
            }
        }

    }
}
