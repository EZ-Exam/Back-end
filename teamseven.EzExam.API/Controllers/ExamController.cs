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
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get exams (paged & filtered)",
            Description = "Search name/description; filter by subjectId, lessonId, examTypeId, createdByUserId; paging + sort."
        )]
        [SwaggerResponse(200, "OK", typeof(PagedResponse<ExamResponse>))]
        public async Task<IActionResult> GetExams(
            [FromQuery] string? search = null,
            [FromQuery] string? sort = null,
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

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get exam by ID", Description = "Retrieves a single exam")]
        public async Task<IActionResult> GetExam(int id)
        {
            var exam = await _serviceProvider.ExamService.GetExamAsync(id);
            return Ok(exam);
        }
        [HttpGet("optimized/feed")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get optimized exams feed", Description = "Lightweight, metadata-only feed for exams (optimized). Use for catalog lists.")]
        public async Task<IActionResult> GetOptimizedExamsFeed(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] int? subjectId = null,
            [FromQuery] int? lessonId = null,
            [FromQuery] int? examTypeId = null,
            [FromQuery] int? createdByUserId = null)
        {
            if (page < 1 || pageSize < 1) return BadRequest(new { Message = "page and pageSize must be > 0" });

            var data = await _serviceProvider.ExamService.GetOptimizedExamsFeedAsync(page, pageSize, search, subjectId, lessonId, examTypeId, createdByUserId);

            return Ok(data);
        }

        [HttpGet("optimized/{id}/details")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get optimized exam details", Description = "Lightweight exam details (question ids + metadata) to minimize payload.")]
        public async Task<IActionResult> GetOptimizedExamDetails(int id, [FromQuery] int currentUserId = 0)
        {
            var data = await _serviceProvider.ExamService.GetOptimizedExamDetailsAsync(id, currentUserId);
            return Ok(data);
        }

        [HttpGet("optimized/user/{userId}/feed")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOptimizedExamsFeedByUser(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var data = await _serviceProvider.ExamService.GetOptimizedExamsFeedByUserAsync(
                userId, page, pageSize);
            return Ok(data);
        }

        [HttpGet("optimized/user/{userId}/{id}/details")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOptimizedExamDetailsByUser(int userId, int id, [FromQuery] int currentUserId = 0)
        {
            var data = await _serviceProvider.ExamService.GetOptimizedExamDetailsAsync(id, currentUserId);
            if (data.CreatedByUserId != userId)
                return NotFound(new { Message = "Exam not found for user." });
            return Ok(data);
        }
        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get exams by user ID", Description = "Retrieves all exams created by a specific user")]
        public async Task<IActionResult> GetExamsByUserId(int userId)
        {
            var exams = await _serviceProvider.ExamService.GetExamsByUserIdAsync(userId);
            return Ok(exams);
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Create exam", Description = "Creates a new exam")]
        public async Task<IActionResult> CreateExam([FromBody] ExamRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _serviceProvider.ExamService.CreateExamAsync(request);
            return StatusCode(201, new { Id = id, Message = "Exam created successfully." });
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Update exam", Description = "Updates an existing exam")]
        public async Task<IActionResult> UpdateExam(int id, [FromBody] UpdateExamRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Id)
                return BadRequest(new { Message = "Route ID and request ID do not match." });

            var updatedExam = await _serviceProvider.ExamService.UpdateExamAsync(request);
            return Ok(updatedExam);
        }

        [HttpPost("questions")]
        [Authorize]
        [SwaggerOperation(Summary = "Assign question to exam")]
        public async Task<IActionResult> AddExamQuestion([FromBody] ExamQuestionRequest request)
        {
            await _serviceProvider.ExamService.CreateExamQuestionAsync(request);
            return Ok(new { Message = "Question added to exam." });
        }

        [HttpDelete("questions")]
        [Authorize]
        [SwaggerOperation(Summary = "Remove question from exam")]
        public async Task<IActionResult> RemoveExamQuestion([FromBody] ExamQuestionRequest request)
        {
            await _serviceProvider.ExamService.RemoveExamQuestion(request);
            return NoContent();
        }

        [HttpGet("{id}/questions")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get exam questions by ExamId")]
        public async Task<IActionResult> GetExamQuestions(int id)
        {
            var questions = await _serviceProvider.ExamService.GetExamQuestionByIdAsync(id);
            return Ok(questions);
        }

        [HttpGet("{id}/questions/detail")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get detailed exam questions with all fields (correctAnswer, options, explanation, etc.)")]
        public async Task<IActionResult> GetExamQuestionsDetail(int id)
        {
            var questions = await _serviceProvider.ExamService.GetExamQuestionsDetailAsync(id);
            return Ok(questions);
        }
        [HttpPatch("{id}/rename")]
        [Authorize]
        [SwaggerOperation(Summary = "Rename exam")]
        public async Task<IActionResult> RenameExam(int id, [FromQuery] string newName)
        {
            await _serviceProvider.ExamService.RenameExamAsync(id, newName);
            return Ok(new { Message = "Exam renamed successfully" });
        }

        [HttpPost("history")]
        [Authorize]
        [SwaggerOperation(Summary = "Create exam history")]
        public async Task<IActionResult> CreateExamHistory([FromBody] ExamHistoryRequest request)
        {
            await _serviceProvider.ExamService.CreateExamHistoryAsync(request);
            return StatusCode(201, new { Message = "Exam history recorded." });
        }

        [HttpDelete("history")]
        [Authorize]
        [SwaggerOperation(Summary = "Delete exam history")]
        public async Task<IActionResult> DeleteExamHistory([FromBody] ExamHistoryRequest request)
        {
            await _serviceProvider.ExamService.DeleteExamHistoryAsync(request);
            return NoContent();
        }

        [HttpGet("history/{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get exam history details")]
        public async Task<IActionResult> GetExamHistory(int id)
        {
            var result = await _serviceProvider.ExamService.GetExamHistoryResponseAsync(id);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Soft delete exam", Description = "Mark exam as deleted")]
        public async Task<IActionResult> SoftDeleteExam(int id)
        {
            await _serviceProvider.ExamService.SoftDeleteExamAsync(id);
            return Ok(new { Message = "Exam soft-deleted successfully." });
        }
        [HttpPatch("{id}/recover")]
        [Authorize]
        [SwaggerOperation(Summary = "Recover exam", Description = "Recover soft-deleted exam (IsDeleted = false)")]
        public async Task<IActionResult> RecoverExam(int id)
        {
            await _serviceProvider.ExamService.RecoverExamAsync(id);
            return Ok(new { Message = "Exam recovered successfully." });
        }

    }
}
