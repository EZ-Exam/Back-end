using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.ServiceProvider;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class TestSessionController : ControllerBase
    {
        private readonly IServiceProviders _serviceProviders;
        private readonly ILogger<TestSessionController> _logger;

        public TestSessionController(IServiceProviders serviceProviders, ILogger<TestSessionController> logger)
        {
            _serviceProviders = serviceProviders ?? throw new ArgumentNullException(nameof(serviceProviders));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Start a new test session
        /// </summary>
        /// <param name="request">Start test session request</param>
        /// <returns>Test session details</returns>
        [HttpPost("start")]
        [SwaggerOperation(Summary = "Start test session", Description = "Starts a new test session for a user and exam")]
        [SwaggerResponse(200, "Test session started successfully", typeof(TestSessionResponse))]
        [SwaggerResponse(400, "Invalid request data")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> StartSession([FromBody] StartTestSessionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var session = await _serviceProviders.TestSessionService.StartSessionAsync(request);
            if (session != null)
            {
                return Ok(session);
            }
            else
            {
                return BadRequest(new { message = "Failed to start test session or user already has an active session for this exam" });
            }
        }

        /// <summary>
        /// End a test session
        /// </summary>
        /// <param name="sessionId">Session ID</param>
        /// <param name="request">End test session request</param>
        /// <returns>Success status</returns>
        [HttpPut("{sessionId}/end")]
        [SwaggerOperation(Summary = "End test session", Description = "Ends a test session and records the results")]
        [SwaggerResponse(200, "Test session ended successfully")]
        [SwaggerResponse(400, "Invalid request data")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> EndSession(int sessionId, [FromBody] EndTestSessionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _serviceProviders.TestSessionService.EndSessionAsync(sessionId, request);
            if (result)
            {
                return Ok(new { message = "Test session ended successfully" });
            }
            else
            {
                return BadRequest(new { message = "Failed to end test session or session not found" });
            }
        }

        /// <summary>
        /// Pause a test session
        /// </summary>
        /// <param name="sessionId">Session ID</param>
        /// <returns>Success status</returns>
        [HttpPut("{sessionId}/pause")]
        [SwaggerOperation(Summary = "Pause test session", Description = "Pauses an active test session")]
        [SwaggerResponse(200, "Test session paused successfully")]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> PauseSession(int sessionId)
        {
            if (sessionId <= 0)
            {
                return BadRequest(new { message = "Invalid session ID" });
            }

            var result = await _serviceProviders.TestSessionService.PauseSessionAsync(sessionId);
            if (result)
            {
                return Ok(new { message = "Test session paused successfully" });
            }
            else
            {
                return BadRequest(new { message = "Failed to pause test session or session not found" });
            }
        }

        /// <summary>
        /// Resume a paused test session
        /// </summary>
        /// <param name="sessionId">Session ID</param>
        /// <returns>Success status</returns>
        [HttpPut("{sessionId}/resume")]
        [SwaggerOperation(Summary = "Resume test session", Description = "Resumes a paused test session")]
        [SwaggerResponse(200, "Test session resumed successfully")]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> ResumeSession(int sessionId)
        {
            if (sessionId <= 0)
            {
                return BadRequest(new { message = "Invalid session ID" });
            }

            var result = await _serviceProviders.TestSessionService.ResumeSessionAsync(sessionId);
            if (result)
            {
                return Ok(new { message = "Test session resumed successfully" });
            }
            else
            {
                return BadRequest(new { message = "Failed to resume test session or session not found" });
            }
        }

        /// <summary>
        /// Get test session details
        /// </summary>
        /// <param name="sessionId">Session ID</param>
        /// <returns>Test session details</returns>
        [HttpGet("{sessionId}")]
        [SwaggerOperation(Summary = "Get test session", Description = "Retrieves details of a specific test session")]
        [SwaggerResponse(200, "Successfully retrieved test session", typeof(TestSessionResponse))]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(404, "Test session not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetSession(int sessionId)
        {
            if (sessionId <= 0)
            {
                return BadRequest(new { message = "Invalid session ID" });
            }

            var session = await _serviceProviders.TestSessionService.GetSessionAsync(sessionId);
            if (session != null)
            {
                return Ok(session);
            }
            else
            {
                return NotFound(new { message = "Test session not found" });
            }
        }

        /// <summary>
        /// Get user's test sessions
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of test sessions</returns>
        [HttpGet("user/{userId}")]
        [SwaggerOperation(Summary = "Get user's test sessions", Description = "Retrieves all test sessions for a specific user")]
        [SwaggerResponse(200, "Successfully retrieved test sessions", typeof(List<TestSessionResponse>))]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetUserSessions(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { message = "Invalid user ID" });
            }

            var sessions = await _serviceProviders.TestSessionService.GetUserSessionsAsync(userId);
            return Ok(sessions);
        }

        /// <summary>
        /// Get user's active test sessions
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of active test sessions</returns>
        [HttpGet("user/{userId}/active")]
        [SwaggerOperation(Summary = "Get active test sessions", Description = "Retrieves all active test sessions for a specific user")]
        [SwaggerResponse(200, "Successfully retrieved active test sessions", typeof(List<TestSessionResponse>))]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetActiveSessions(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { message = "Invalid user ID" });
            }

            var sessions = await _serviceProviders.TestSessionService.GetActiveSessionsAsync(userId);
            return Ok(sessions);
        }

        /// <summary>
        /// Get user's completed test sessions
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of completed test sessions</returns>
        [HttpGet("user/{userId}/completed")]
        [SwaggerOperation(Summary = "Get completed test sessions", Description = "Retrieves all completed test sessions for a specific user")]
        [SwaggerResponse(200, "Successfully retrieved completed test sessions", typeof(List<TestSessionResponse>))]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetCompletedSessions(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { message = "Invalid user ID" });
            }

            var sessions = await _serviceProviders.TestSessionService.GetCompletedSessionsAsync(userId);
            return Ok(sessions);
        }

        /// <summary>
        /// Get active session for user and exam
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="examId">Exam ID</param>
        /// <returns>Active test session</returns>
        [HttpGet("user/{userId}/exam/{examId}/active")]
        [SwaggerOperation(Summary = "Get active session for exam", Description = "Retrieves the active test session for a specific user and exam")]
        [SwaggerResponse(200, "Successfully retrieved active session", typeof(TestSessionResponse))]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(404, "No active session found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetActiveSessionByExam(int userId, int examId)
        {
            if (userId <= 0 || examId <= 0)
            {
                return BadRequest(new { message = "Invalid user ID or exam ID" });
            }

            var session = await _serviceProviders.TestSessionService.GetActiveSessionByExamAsync(userId, examId);
            if (session != null)
            {
                return Ok(session);
            }
            else
            {
                return NotFound(new { message = "No active session found for this user and exam" });
            }
        }

        /// <summary>
        /// Submit an answer for a test session
        /// </summary>
        /// <param name="request">Submit answer request</param>
        /// <returns>Success status</returns>
        [HttpPost("submit-answer")]
        [SwaggerOperation(Summary = "Submit answer", Description = "Submits an answer for a test session")]
        [SwaggerResponse(200, "Answer submitted successfully")]
        [SwaggerResponse(400, "Invalid request data")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> SubmitAnswer([FromBody] SubmitAnswerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _serviceProviders.TestSessionService.SubmitAnswerAsync(request);
            if (result)
            {
                return Ok(new { message = "Answer submitted successfully" });
            }
            else
            {
                return BadRequest(new { message = "Failed to submit answer" });
            }
        }

        /// <summary>
        /// Get session answers
        /// </summary>
        /// <param name="sessionId">Session ID</param>
        /// <returns>List of session answers</returns>
        [HttpGet("{sessionId}/answers")]
        [SwaggerOperation(Summary = "Get session answers", Description = "Retrieves all answers for a specific test session")]
        [SwaggerResponse(200, "Successfully retrieved session answers", typeof(List<TestSessionAnswerResponse>))]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetSessionAnswers(int sessionId)
        {
            if (sessionId <= 0)
            {
                return BadRequest(new { message = "Invalid session ID" });
            }

            var answers = await _serviceProviders.TestSessionService.GetSessionAnswersAsync(sessionId);
            return Ok(answers);
        }

        /// <summary>
        /// Get user's average score
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Average score</returns>
        [HttpGet("user/{userId}/average-score")]
        [SwaggerOperation(Summary = "Get average score", Description = "Gets the average score for a user across all completed sessions")]
        [SwaggerResponse(200, "Successfully retrieved average score", typeof(decimal))]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetAverageScore(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { message = "Invalid user ID" });
            }

            var averageScore = await _serviceProviders.TestSessionService.GetAverageScoreAsync(userId);
            return Ok(new { averageScore });
        }

        /// <summary>
        /// Get total sessions count for user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Total sessions count</returns>
        [HttpGet("user/{userId}/total-count")]
        [SwaggerOperation(Summary = "Get total sessions count", Description = "Gets the total number of test sessions for a user")]
        [SwaggerResponse(200, "Successfully retrieved total sessions count", typeof(int))]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetTotalSessionsCount(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { message = "Invalid user ID" });
            }

            var totalCount = await _serviceProviders.TestSessionService.GetTotalSessionsCountAsync(userId);
            return Ok(new { totalCount });
        }
    }
}
