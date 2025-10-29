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

            try
            {
                var id = await _examHistoryService.CreateExamHistoryAsync(request);
                return StatusCode(201, new { Id = id, Message = "Exam history created successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exam history");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get exam history by ID", Description = "Retrieves a single exam history record")]
        public async Task<IActionResult> GetExamHistory(int id)
        {
            try
            {
                var history = await _examHistoryService.GetExamHistoryByIdAsync(id);
                if (history == null)
                    return NotFound(new { Message = "Exam history not found." });

                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam history");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get exam histories by user ID", Description = "Retrieves all exam histories for a specific user")]
        public async Task<IActionResult> GetExamHistoriesByUser(int userId)
        {
            try
            {
                var histories = await _examHistoryService.GetExamHistoriesByUserIdAsync(userId);
                return Ok(histories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam histories by user");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpGet("exam/{examId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get exam histories by exam ID", Description = "Retrieves all exam histories for a specific exam")]
        public async Task<IActionResult> GetExamHistoriesByExam(int examId)
        {
            try
            {
                var histories = await _examHistoryService.GetExamHistoriesByExamIdAsync(examId);
                return Ok(histories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam histories by exam");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all exam histories", Description = "Retrieves all exam history records")]
        public async Task<IActionResult> GetAllExamHistories()
        {
            try
            {
                var histories = await _examHistoryService.GetAllExamHistoriesAsync();
                return Ok(histories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all exam histories");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Update exam history", Description = "Updates an existing exam history record")]
        public async Task<IActionResult> UpdateExamHistory(int id, [FromBody] CreateExamHistoryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _examHistoryService.UpdateExamHistoryAsync(id, request);
                return Ok(new { Message = "Exam history updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exam history");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Delete exam history", Description = "Deletes an exam history record")]
        public async Task<IActionResult> DeleteExamHistory(int id)
        {
            try
            {
                await _examHistoryService.DeleteExamHistoryAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exam history");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpGet("{examId}/questions/detail")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get exam questions detail", Description = "Retrieves detailed information of all questions in an exam")]
        [SwaggerResponse(200, "Exam questions retrieved successfully", typeof(IEnumerable<ExamQuestionDetailResponse>))]
        [SwaggerResponse(404, "Exam not found")]
        public async Task<IActionResult> GetExamQuestionsDetail(int examId)
        {
            try
            {
                var questions = await _examHistoryService.GetExamQuestionsDetailAsync(examId);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam questions detail");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }
    }
}
