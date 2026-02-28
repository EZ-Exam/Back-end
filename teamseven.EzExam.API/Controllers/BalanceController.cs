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

        [HttpPost]
        [SwaggerOperation(Summary = "Add balance to current user", Description = "Adds balance to the current authenticated user's account.")]
        [SwaggerResponse(200, "Balance added successfully.", typeof(BalanceResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid token or super secret key.", typeof(object))]
        [SwaggerResponse(404, "User not found.", typeof(object))]
        public async Task<IActionResult> AddBalance([FromBody] AddBalanceRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for add balance request.");
                return BadRequest(ModelState);
            }

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

        [HttpGet("me")]
        [SwaggerOperation(Summary = "Get current user balance info", Description = "Retrieves the current balance and info of the authenticated user.")]
        [SwaggerResponse(200, "Balance retrieved successfully.", typeof(BalanceResponse))]
        [SwaggerResponse(401, "Unauthorized - Invalid token.", typeof(object))]
        [SwaggerResponse(404, "User not found.", typeof(object))]
        public async Task<IActionResult> GetCurrentBalance()
        {
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

        [HttpPost("admin/deposit")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Admin deposit to any user account", Description = "Deposits balance to any user account using master super secret key. Admin-only operation.")]
        [SwaggerResponse(200, "Deposit successful.", typeof(BalanceResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid super secret key.", typeof(object))]
        [SwaggerResponse(404, "User not found.", typeof(object))]
        public async Task<IActionResult> MasterDeposit([FromBody] MasterDepositRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for master deposit request.");
                return BadRequest(ModelState);
            }

            var result = await _balanceService.MasterDepositAsync(request);
            return Ok(result);
        }

    }
}
