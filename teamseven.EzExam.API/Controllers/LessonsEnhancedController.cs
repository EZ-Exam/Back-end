using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.ServiceProvider;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/lessons-enhanced")]
    [Produces("application/json")]
    public class LessonsEnhancedController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;
        private readonly ILogger<LessonsEnhancedController> _logger;

        public LessonsEnhancedController(IServiceProviders serviceProvider, ILogger<LessonsEnhancedController> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        // Create/Upsert từ JSON
        [HttpPost]
        [Authorize] // đổi sang [AllowAnonymous] nếu cần test nhanh
        [SwaggerOperation(Summary = "Create/Upsert lesson enhanced", Description = "Create or update a LessonEnhanced with ordered questions")]
        [SwaggerResponse(201, "Lesson created/updated successfully.")]
        [SwaggerResponse(400, "Bad request.")]
        [SwaggerResponse(401, "Unauthorized.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> Upsert([FromBody] LessonEnhancedUpsertRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var res = await _serviceProvider.LessonEnhancedService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = res.id }, res);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid payload for LessonEnhanced upsert");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting LessonEnhanced");
                return StatusCode(500, "Internal server error");
            }
        }

        // Get by lesson id
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get lesson enhanced by id")]
        [SwaggerResponse(200, "Lesson retrieved successfully.")]
        [SwaggerResponse(404, "Lesson not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var res = await _serviceProvider.LessonEnhancedService.GetByIdAsync(id);
                return Ok(res);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Lesson {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LessonEnhanced {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // Get by question id
        [HttpGet("by-question/{questionId:int}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get lessons enhanced containing a question id")]
        [SwaggerResponse(200, "Lessons retrieved successfully.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetByQuestion([FromRoute] int questionId)
        {
            try
            {
                var res = await _serviceProvider.LessonEnhancedService.GetByQuestionIdAsync(questionId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lessons by questionId {QuestionId}", questionId);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(
        Summary = "Get all lesson-enhanced",
        Description = "Lấy toàn bộ lesson-enhanced; có thể lọc theo subjectId (string) và chọn kèm danh sách questions.")]
        [SwaggerResponse(200, "OK", typeof(IEnumerable<LessonEnhancedResponse>))]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? subjectId = null)
        {
            try
            {
                var res = await _serviceProvider.LessonEnhancedService.GetAllAsync(subjectId, true);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll LessonEnhanced failed. subjectId={SubjectId}, includeQuestions={IncludeQuestions}", subjectId, true);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("paged")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get lesson-enhanced (paged & filtered)",
            Description = "Phân trang + lọc theo subjectId (string) hoặc questionId (string); hỗ trợ search/sort; includeQuestions=true để trả thêm mảng câu hỏi theo Position."
        )]
        [SwaggerResponse(200, "OK", typeof(PagedResponse<LessonEnhancedResponse>))]
        [SwaggerResponse(400, "Invalid parameters.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? subjectId = null,
            [FromQuery] string? questionId = null,
            [FromQuery] string? search = null,
            [FromQuery] string? sort = null,              // title|createdAt|updatedAt : asc|desc
            [FromQuery] int? pageNumber = null,
            [FromQuery] int? pageSize = null,
            [FromQuery] int isSort = 0                   // 0 = default sort by Id asc; 1 = dùng 'sort'
)
        {
            try
            {
                // Validate basic params
                if ((pageNumber.HasValue && pageNumber < 1) ||
                    (pageSize.HasValue && pageSize < 1))
                {
                    _logger.LogWarning("Invalid pagination: pageNumber={PageNumber}, pageSize={PageSize}", pageNumber, pageSize);
                    return BadRequest(new { Message = "pageNumber and pageSize must be greater than 0." });
                }

                if (isSort is not (0 or 1))
                {
                    _logger.LogWarning("Invalid isSort: {IsSort}", isSort);
                    return BadRequest(new { Message = "isSort must be 0 or 1." });
                }

                if (isSort == 1 && !string.IsNullOrWhiteSpace(sort) && !IsValidSort(sort))
                {
                    _logger.LogWarning("Invalid sort: {Sort}", sort);
                    return BadRequest(new { Message = "Invalid sort. Use one of: title|createdAt|updatedAt with :asc or :desc (e.g. createdAt:desc)." });
                }

                var res = await _serviceProvider.LessonEnhancedService.GetPagedAsync(
                    pageNumber, pageSize, search, sort, subjectId, questionId, isSort, true);

                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Bad query in LessonsEnhancedController.GetPaged");
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get lesson-enhanced paged failed. subjectId={SubjectId}, questionId={QuestionId}", subjectId, questionId);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }
        private static bool IsValidSort(string sort)
        {
            var validFields = new[] { "title", "createdat", "updatedat" };
            var validOrders = new[] { "asc", "desc" };
            var parts = sort.ToLower().Split(':');
            return parts.Length == 2 && validFields.Contains(parts[0]) && validOrders.Contains(parts[1]);
        }
    }
}
