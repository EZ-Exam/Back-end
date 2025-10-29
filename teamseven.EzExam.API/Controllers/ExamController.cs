using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Services.ServiceProvider;
using teamseven.EzExam.Services.Object.Responses;
using static teamseven.EzExam.Services.Services.ExamService.IExamService;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/exams")]
    [Produces("application/json")]
    public class ExamController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;
        private readonly ILogger<ExamController> _logger;

        public ExamController(IServiceProviders serviceProvider, ILogger<ExamController> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        // API/Controllers/ExamsController.cs
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get exams (paged & filtered)",
            Description = "Search name/description; filter by subjectId, lessonId, examTypeId, createdByUserId; paging + sort."
        )]
        [SwaggerResponse(200, "OK", typeof(PagedResponse<ExamResponse>))]
        public async Task<IActionResult> GetExams(
            [FromQuery] string? search = null,
            [FromQuery] string? sort = null,          // name|createdAt|updatedAt|totalQuestions|timeLimit : asc|desc
            [FromQuery] int? subjectId = null,
            [FromQuery] int? lessonId = null,
            [FromQuery] int? examTypeId = null,
            [FromQuery] int? createdByUserId = null,
            [FromQuery] int? pageNumber = null,
            [FromQuery] int? pageSize = null,
            [FromQuery] int isSort = 0
        )
        {
            if (pageNumber.HasValue && pageNumber < 1 || pageSize.HasValue && pageSize < 1)
                return BadRequest(new { Message = "pageNumber and pageSize must be > 0." });

            if (isSort is not (0 or 1))
                return BadRequest(new { Message = "isSort must be 0 or 1." });

            if (isSort == 1 && !string.IsNullOrWhiteSpace(sort) && !IsValidSort(sort))
                return BadRequest(new { Message = "Invalid sort. Use: name|createdAt|updatedAt|totalQuestions|timeLimit with :asc|:desc" });

            var data = await _serviceProvider.ExamService.GetExamsAsync(
                pageNumber, pageSize, search, sort,
                subjectId, lessonId, examTypeId, createdByUserId, isSort);

            return Ok(data);
        }

        private static bool IsValidSort(string sort)
        {
            var fields = new[] { "name", "createdat", "updatedat", "totalquestions", "timelimit" };
            var orders = new[] { "asc", "desc" };
            var p = sort.ToLower().Split(':');
            return p.Length == 2 && fields.Contains(p[0]) && orders.Contains(p[1]);
        }

        //// =================== GET ALL EXAMS ===================

        //[HttpGet]
        //[AllowAnonymous]
        //[SwaggerOperation(Summary = "Get all exams", Description = "Retrieves all exams")]
        //public async Task<IActionResult> GetAllExams()
        //{
        //    var exams = await _serviceProvider.ExamService.GetAllExamAsync();
        //    return Ok(exams);
        //}

        // =================== GET EXAM BY ID ===================

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get exam by ID", Description = "Retrieves a single exam")]
        public async Task<IActionResult> GetExam(int id)
        {
            try
            {
                var exam = await _serviceProvider.ExamService.GetExamAsync(id);
                return Ok(exam);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { Message = ex.Message });
            }
        }
        // =================== GET EXAM BY USERID ===================
        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get exams by user ID", Description = "Retrieves all exams created by a specific user")]
        public async Task<IActionResult> GetExamsByUserId(int userId)
        {
            try
            {
                var exams = await _serviceProvider.ExamService.GetExamsByUserIdAsync(userId);
                return Ok(exams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exams by userId");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }


        // =================== CREATE EXAM ===================

        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Create exam", Description = "Creates a new exam")]
        public async Task<IActionResult> CreateExam([FromBody] ExamRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var id = await _serviceProvider.ExamService.CreateExamAsync(request);
                return StatusCode(201, new { Id = id, Message = "Exam created successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exam");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        // =================== UPDATE EXAM ===================

        [HttpPut("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Update exam", Description = "Updates an existing exam")]
        public async Task<IActionResult> UpdateExam(int id, [FromBody] UpdateExamRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Id)
                return BadRequest(new { Message = "Route ID and request ID do not match." });

            try
            {
                var updatedExam = await _serviceProvider.ExamService.UpdateExamAsync(request);
                return Ok(updatedExam);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Exam not found");
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exam");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        // =================== ADD QUESTION TO EXAM ===================

        [HttpPost("questions")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Assign question to exam")]
        public async Task<IActionResult> AddExamQuestion([FromBody] ExamQuestionRequest request)
        {
            try
            {
                await _serviceProvider.ExamService.CreateExamQuestionAsync(request);
                return Ok(new { Message = "Question added to exam." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning question");
                return BadRequest(new { Message = ex.Message });
            }
        }

        // =================== REMOVE QUESTION FROM EXAM ===================

        [HttpDelete("questions")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Remove question from exam")]
        public async Task<IActionResult> RemoveExamQuestion([FromBody] ExamQuestionRequest request)
        {
            try
            {
                await _serviceProvider.ExamService.RemoveExamQuestion(request);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { Message = ex.Message });
            }
        }

        // =================== GET QUESTIONS BY EXAM ID ===================

        [HttpGet("{id}/questions")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get exam questions by ExamId")]
        public async Task<IActionResult> GetExamQuestions(int id)
        {
            var questions = await _serviceProvider.ExamService.GetExamQuestionByIdAsync(id);
            return Ok(questions);
        }

        // =================== GET DETAILED QUESTIONS BY EXAM ID ===================

        [HttpGet("{id}/questions/detail")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get detailed exam questions with all fields (correctAnswer, options, explanation, etc.)")]
        public async Task<IActionResult> GetExamQuestionsDetail(int id)
        {
            try
            {
                var questions = await _serviceProvider.ExamService.GetExamQuestionsDetailAsync(id);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving detailed exam questions for exam {ExamId}", id);
                return StatusCode(500, new { Message = "Error retrieving exam questions", Detail = ex.Message });
            }
        }
        [HttpPut("rename/{examId}")]
        public async Task<IActionResult> RenameExam(int examId, string newName)
        {
            try
            {
                await _serviceProvider.ExamService.RenameExamAsync(examId, newName);
                return Ok(new { Message = "Exam renamed successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal error", Detail = ex.Message });
            }
        }

        // =================== CREATE EXAM HISTORY ===================

        [HttpPost("history")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Create exam history")]
        public async Task<IActionResult> CreateExamHistory([FromBody] ExamHistoryRequest request)
        {
            try
            {
                await _serviceProvider.ExamService.CreateExamHistoryAsync(request);
                return StatusCode(201, new { Message = "Exam history recorded." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exam history");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        // =================== DELETE EXAM HISTORY ===================

        [HttpDelete("history")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Delete exam history")]
        public async Task<IActionResult> DeleteExamHistory([FromBody] ExamHistoryRequest request)
        {
            try
            {
                await _serviceProvider.ExamService.DeleteExamHistoryAsync(request);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exam history");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        // =================== GET EXAM HISTORY DETAILS ===================

        [HttpGet("history/{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get exam history details")]
        public async Task<IActionResult> GetExamHistory(int id)
        {
            try
            {
                var result = await _serviceProvider.ExamService.GetExamHistoryResponseAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam history");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }
        // =================== SOFT DELETE EXAM ===================
        [HttpPut("{id}/soft-delete")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Soft delete exam", Description = "Mark exam as deleted")]
        public async Task<IActionResult> SoftDeleteExam(int id)
        {
            try
            {
                await _serviceProvider.ExamService.SoftDeleteExamAsync(id);
                return Ok(new { Message = "Exam soft-deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Exam not found");
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft deleting exam");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }
        // =================== RECOVER EXAM ===================
        [HttpPut("{id}/recover")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Recover exam", Description = "Recover soft-deleted exam (IsDeleted = false)")]
        public async Task<IActionResult> RecoverExam(int id)
        {
            try
            {
                await _serviceProvider.ExamService.RecoverExamAsync(id);
                return Ok(new { Message = "Exam recovered successfully." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Exam not found");
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recovering exam");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

    }
}
