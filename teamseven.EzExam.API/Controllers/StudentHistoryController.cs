using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Services.ServiceProvider;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/student-history")]
    [Produces("application/json")]
    public class StudentHistoryController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;
        private readonly ILogger<StudentHistoryController> _logger;

        public StudentHistoryController(IServiceProviders serviceProvider, ILogger<StudentHistoryController> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [HttpPost("quiz")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Create quiz history", Description = "Creates a new quiz history record")]
        public async Task<IActionResult> CreateQuizHistory([FromBody] CreateStudentQuizHistoryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _serviceProvider.StudentHistoryService.CreateQuizHistoryAsync(request);
            return StatusCode(201, new { Id = id, Message = "Quiz history created successfully." });
        }

        [HttpGet("quiz/{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get quiz history by ID", Description = "Retrieves a single quiz history record")]
        public async Task<IActionResult> GetQuizHistory(int id)
        {
            var history = await _serviceProvider.StudentHistoryService.GetQuizHistoryByIdAsync(id);
            if (history == null)
                return NotFound(new { Message = "Quiz history not found." });

            return Ok(history);
        }

        [HttpGet("quiz/user/{userId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get quiz histories by user ID", Description = "Retrieves all quiz histories for a specific user")]
        public async Task<IActionResult> GetQuizHistoriesByUser(int userId)
        {
            var histories = await _serviceProvider.StudentHistoryService.GetQuizHistoriesByUserIdAsync(userId);
            return Ok(histories);
        }

        [HttpGet("quiz/user/{userId}/recent")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get recent quiz histories", Description = "Retrieves recent quiz histories for a user")]
        public async Task<IActionResult> GetRecentQuizHistories(int userId, [FromQuery] int count = 5)
        {
            var histories = await _serviceProvider.StudentHistoryService.GetRecentQuizHistoriesByUserIdAsync(userId, count);
            return Ok(histories);
        }

        [HttpGet("quiz/user/{userId}/subject/{subjectId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get quiz histories by user and subject", Description = "Retrieves quiz histories for a user in a specific subject")]
        public async Task<IActionResult> GetQuizHistoriesByUserAndSubject(int userId, int subjectId)
        {
            var histories = await _serviceProvider.StudentHistoryService.GetQuizHistoriesByUserAndSubjectAsync(userId, subjectId);
            return Ok(histories);
        }

        [HttpGet("quiz/exam/{examId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get quiz histories by exam ID", Description = "Retrieves all quiz histories for a specific exam")]
        public async Task<IActionResult> GetQuizHistoriesByExam(int examId)
        {
            var histories = await _serviceProvider.StudentHistoryService.GetQuizHistoriesByExamIdAsync(examId);
            return Ok(histories);
        }

        [HttpPut("quiz/{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Update quiz history", Description = "Updates an existing quiz history record")]
        public async Task<IActionResult> UpdateQuizHistory(int id, [FromBody] CreateStudentQuizHistoryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceProvider.StudentHistoryService.UpdateQuizHistoryAsync(id, request);
            return Ok(new { Message = "Quiz history updated successfully." });
        }

        [HttpDelete("quiz/{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Delete quiz history", Description = "Deletes a quiz history record")]
        public async Task<IActionResult> DeleteQuizHistory(int id)
        {
            await _serviceProvider.StudentHistoryService.DeleteQuizHistoryAsync(id);
            return NoContent();
        }

        [HttpGet("performance/user/{userId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get performance summary", Description = "Retrieves performance summary for a user")]
        public async Task<IActionResult> GetPerformanceSummary(int userId, [FromQuery] int? subjectId = null)
        {
            var summary = await _serviceProvider.StudentHistoryService.GetPerformanceSummaryAsync(userId, subjectId);
            if (summary == null)
                return NotFound(new { Message = "Performance summary not found." });

            return Ok(summary);
        }

        [HttpGet("performance/user/{userId}/all")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all performance summaries", Description = "Retrieves all performance summaries for a user")]
        public async Task<IActionResult> GetAllPerformanceSummaries(int userId)
        {
            var summaries = await _serviceProvider.StudentHistoryService.GetAllPerformanceSummariesByUserAsync(userId);
            return Ok(summaries);
        }

        [HttpPost("performance/user/{userId}/update")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Update performance summary", Description = "Updates performance summary for a user")]
        public async Task<IActionResult> UpdatePerformanceSummary(int userId, [FromQuery] int? subjectId = null)
        {
            await _serviceProvider.StudentHistoryService.UpdatePerformanceSummaryAsync(userId, subjectId);
            return Ok(new { Message = "Performance summary updated successfully." });
        }

        [HttpPost("performance/user/{userId}/recalculate")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Recalculate performance summary", Description = "Recalculates performance summary for a user")]
        public async Task<IActionResult> RecalculatePerformanceSummary(int userId, [FromQuery] int? subjectId = null)
        {
            await _serviceProvider.StudentHistoryService.RecalculatePerformanceSummaryAsync(userId, subjectId);
            return Ok(new { Message = "Performance summary recalculated successfully." });
        }

        [HttpGet("analytics/competency/user/{userId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get student competency for AI", Description = "Retrieves comprehensive student competency data for AI recommendations")]
        public async Task<IActionResult> GetStudentCompetencyForAI(int userId, [FromQuery] int? subjectId = null)
        {
            var competency = await _serviceProvider.StudentHistoryService.GetStudentCompetencyForAIAsync(userId, subjectId);
            return Ok(competency);
        }

        [HttpGet("analytics/top-performers")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get top performers", Description = "Retrieves top performing students")]
        public async Task<IActionResult> GetTopPerformers([FromQuery] int? subjectId = null, [FromQuery] int count = 10)
        {
            var performers = await _serviceProvider.StudentHistoryService.GetTopPerformersAsync(subjectId, count);
            return Ok(performers);
        }

        [HttpGet("analytics/needs-improvement")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get students needing improvement", Description = "Retrieves students who need improvement")]
        public async Task<IActionResult> GetStudentsNeedingImprovement([FromQuery] int? subjectId = null, [FromQuery] decimal threshold = 60.0m)
        {
            var students = await _serviceProvider.StudentHistoryService.GetStudentsNeedingImprovementAsync(subjectId, threshold);
            return Ok(students);
        }

        [HttpPost("integration/test-session/{testSessionId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Create quiz history from test session", Description = "Creates quiz history from completed test session")]
        public async Task<IActionResult> CreateQuizHistoryFromTestSession(int testSessionId)
        {
            await _serviceProvider.StudentHistoryService.CreateQuizHistoryFromTestSessionAsync(testSessionId);
            return Ok(new { Message = "Quiz history created from test session successfully." });
        }

        [HttpGet("integration/recent-attempt/user/{userId}/exam/{examId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Check recent quiz attempt", Description = "Checks if user has recent quiz attempt for exam")]
        public async Task<IActionResult> HasRecentQuizAttempt(int userId, int examId, [FromQuery] int hoursWindow = 24)
        {
            var timeWindow = TimeSpan.FromHours(hoursWindow);
            var hasRecent = await _serviceProvider.StudentHistoryService.HasRecentQuizHistoryAsync(userId, examId, timeWindow);
            return Ok(new { HasRecentAttempt = hasRecent });
        }

        [HttpPost("batch/quiz-histories")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Create multiple quiz histories", Description = "Creates multiple quiz history records")]
        public async Task<IActionResult> CreateMultipleQuizHistories([FromBody] IEnumerable<CreateStudentQuizHistoryRequest> requests)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var count = await _serviceProvider.StudentHistoryService.CreateMultipleQuizHistoriesAsync(requests);
            return StatusCode(201, new { Count = count, Message = $"{count} quiz histories created successfully." });
        }

        [HttpPost("batch/performance-summaries/update")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Update multiple performance summaries", Description = "Updates performance summaries for multiple users")]
        public async Task<IActionResult> UpdateMultiplePerformanceSummaries([FromBody] IEnumerable<int> userIds, [FromQuery] int? subjectId = null)
        {
            await _serviceProvider.StudentHistoryService.UpdateMultiplePerformanceSummariesAsync(userIds, subjectId);
            return Ok(new { Message = "Performance summaries updated successfully." });
        }

        [HttpGet("quiz/{quizHistoryId}/questions")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get question attempts by quiz history", Description = "Retrieves all question attempts for a specific quiz history")]
        public async Task<IActionResult> GetQuestionAttemptsByQuizHistory(int quizHistoryId)
        {
            var attempts = await _serviceProvider.StudentHistoryService.GetQuestionAttemptsByQuizHistoryAsync(quizHistoryId);
            return Ok(attempts);
        }

        [HttpGet("questions/user/{userId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all question attempts by user", Description = "Retrieves all question attempts for a specific user")]
        public async Task<IActionResult> GetQuestionAttemptsByUser(int userId)
        {
            var attempts = await _serviceProvider.StudentHistoryService.GetQuestionAttemptsByUserAsync(userId);
            return Ok(attempts);
        }

        [HttpGet("questions/user/{userId}/correct")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get correct question attempts", Description = "Retrieves all correct question attempts for a user")]
        public async Task<IActionResult> GetCorrectAttemptsByUser(int userId)
        {
            var attempts = await _serviceProvider.StudentHistoryService.GetCorrectAttemptsByUserAsync(userId);
            return Ok(attempts);
        }

        [HttpGet("questions/user/{userId}/incorrect")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get incorrect question attempts", Description = "Retrieves all incorrect question attempts for a user")]
        public async Task<IActionResult> GetIncorrectAttemptsByUser(int userId)
        {
            var attempts = await _serviceProvider.StudentHistoryService.GetIncorrectAttemptsByUserAsync(userId);
            return Ok(attempts);
        }

        [HttpGet("analytics/accuracy/user/{userId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get user accuracy", Description = "Retrieves overall accuracy percentage for a user")]
        public async Task<IActionResult> GetUserAccuracy(int userId)
        {
            var accuracy = await _serviceProvider.StudentHistoryService.GetUserAccuracyAsync(userId);
            return Ok(new { UserId = userId, Accuracy = accuracy });
        }

        [HttpGet("analytics/accuracy/user/{userId}/difficulty/{difficultyLevel}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get user accuracy by difficulty", Description = "Retrieves accuracy percentage for a user by difficulty level")]
        public async Task<IActionResult> GetUserAccuracyByDifficulty(int userId, string difficultyLevel)
        {
            var accuracy = await _serviceProvider.StudentHistoryService.GetUserAccuracyByDifficultyAsync(userId, difficultyLevel);
            return Ok(new { UserId = userId, DifficultyLevel = difficultyLevel, Accuracy = accuracy });
        }

        [HttpGet("analytics/accuracy/user/{userId}/topic/{topic}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get user accuracy by topic", Description = "Retrieves accuracy percentage for a user by topic")]
        public async Task<IActionResult> GetUserAccuracyByTopic(int userId, string topic)
        {
            var accuracy = await _serviceProvider.StudentHistoryService.GetUserAccuracyByTopicAsync(userId, topic);
            return Ok(new { UserId = userId, Topic = topic, Accuracy = accuracy });
        }
    }
}
