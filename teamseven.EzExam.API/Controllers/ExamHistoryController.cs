using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using teamseven.EzExam.Services.Interfaces;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/exam-history")]
    [Produces("application/json")]
    public class ExamHistoryController : ControllerBase
    {
        private readonly IExamHistoryService _examHistoryService;
        private readonly ILogger<ExamHistoryController> _logger;

        public ExamHistoryController(IExamHistoryService examHistoryService, ILogger<ExamHistoryController> logger)
        {
            _examHistoryService = examHistoryService;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Create exam history", Description = "Creates a new exam history record")]
        public async Task<IActionResult> CreateExamHistory([FromBody] CreateExamHistoryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _examHistoryService.CreateExamHistoryAsync(request);
            return StatusCode(201, new { Id = id, Message = "Exam history created successfully." });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get exam history by ID", Description = "Retrieves a single exam history record")]
        public async Task<IActionResult> GetExamHistory(int id)
        {
            var history = await _examHistoryService.GetExamHistoryByIdAsync(id);
            if (history == null)
                return NotFound(new { Message = "Exam history not found." });

            return Ok(history);
        }

        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get exam histories by user ID", Description = "Retrieves all exam histories for a specific user")]
        public async Task<IActionResult> GetExamHistoriesByUser(int userId)
        {
            var histories = await _examHistoryService.GetExamHistoriesByUserIdAsync(userId);
            return Ok(histories);
        }

        [HttpGet("exam/{examId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get exam histories by exam ID", Description = "Retrieves all exam histories for a specific exam")]
        public async Task<IActionResult> GetExamHistoriesByExam(int examId)
        {
            var histories = await _examHistoryService.GetExamHistoriesByExamIdAsync(examId);
            return Ok(histories);
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all exam histories", Description = "Retrieves all exam history records")]
        public async Task<IActionResult> GetAllExamHistories()
        {
            var histories = await _examHistoryService.GetAllExamHistoriesAsync();
            return Ok(histories);
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Update exam history", Description = "Updates an existing exam history record")]
        public async Task<IActionResult> UpdateExamHistory(int id, [FromBody] CreateExamHistoryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _examHistoryService.UpdateExamHistoryAsync(id, request);
            return Ok(new { Message = "Exam history updated successfully." });
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Delete exam history", Description = "Deletes an exam history record")]
        public async Task<IActionResult> DeleteExamHistory(int id)
        {
            await _examHistoryService.DeleteExamHistoryAsync(id);
            return NoContent();
        }

        [HttpGet("{examId}/questions/detail")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get exam questions detail", Description = "Retrieves detailed information of all questions in an exam")]
        [SwaggerResponse(200, "Exam questions retrieved successfully", typeof(IEnumerable<ExamQuestionDetailResponse>))]
        [SwaggerResponse(404, "Exam not found")]
        public async Task<IActionResult> GetExamQuestionsDetail(int examId)
        {
            var questions = await _examHistoryService.GetExamQuestionsDetailAsync(examId);
            return Ok(questions);
        }
    }
}
