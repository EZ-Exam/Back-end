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
    [Route("api/lessons")]
    [Produces("application/json")]
    public class LessonController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;
        private readonly ILogger<LessonController> _logger;

        public LessonController(IServiceProviders serviceProvider, ILogger<LessonController> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get lessons (filtered & paged)", Description = "Optional filters: chapterId, gradeId, search, sort, pageNumber, pageSize.")]
        [SwaggerResponse(200, "Lessons retrieved successfully.", typeof(PagedResponse<LessonDataResponse>))]
        public async Task<IActionResult> GetLessons(
            [FromQuery] int? pageNumber = null,
            [FromQuery] int? pageSize = null,
            [FromQuery] string? search = null,
            [FromQuery] string? sort = null,
            [FromQuery] int? chapterId = null,
            [FromQuery] int? gradeId = null,
            [FromQuery] int isSort = 0)
        {
            if (chapterId.HasValue && pageNumber == null && pageSize == null)
            {
                // simple by-chapter list
                var byChapter = await _serviceProvider.LessonService.GetLessonsByChapterIdAsync(chapterId.Value);
                return Ok(byChapter);
            }

            var lessons = await _serviceProvider.LessonService.GetLessonsAsync(
                pageNumber, pageSize, search, sort, chapterId, isSort);
            return Ok(lessons);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get lesson by ID", Description = "Retrieves a specific lesson by its ID")]
        [SwaggerResponse(200, "Lesson retrieved successfully.", typeof(LessonDataResponse))]
        [SwaggerResponse(404, "Lesson not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetLessonById(int id)
        {
            var lesson = await _serviceProvider.LessonService.GetLessonByIdAsync(id);
            return Ok(lesson);
        }

        [HttpGet("feed")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get optimized lessons feed", Description = "Lightweight lessons feed with metadata.")]
        public async Task<IActionResult> GetOptimizedLessonsFeed(
            [FromQuery] int currentUserId = 0,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] int? chapterId = null)
        {
            if (page < 1 || pageSize < 1) return BadRequest(new { Message = "page and pageSize must be > 0" });

            var data = await _serviceProvider.LessonService.GetOptimizedLessonsFeedAsync(currentUserId, page, pageSize, search, chapterId);
            return Ok(data);
        }

        [HttpGet("{id}/details")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get lesson details", Description = "Detailed lesson info with question and exam ids.")]
        public async Task<IActionResult> GetOptimizedLessonDetails(int id, [FromQuery] int currentUserId = 0)
        {
            var data = await _serviceProvider.LessonService.GetOptimizedLessonDetailsAsync(id, currentUserId);
            return Ok(data);
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Create new lesson", Description = "Creates a new lesson")]
        [SwaggerResponse(201, "Lesson created successfully.", typeof(LessonDataResponse))]
        [SwaggerResponse(400, "Bad request.")]
        [SwaggerResponse(401, "Unauthorized.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> CreateLesson([FromBody] CreateLessonRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _serviceProvider.LessonService.CreateLessonAsync(request);
            return StatusCode(201, "Lesson created successfully");
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Update lesson", Description = "Updates an existing lesson")]
        [SwaggerResponse(200, "Lesson updated successfully.")]
        [SwaggerResponse(400, "Bad request.")]
        [SwaggerResponse(401, "Unauthorized.")]
        [SwaggerResponse(404, "Lesson not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] LessonDataRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            request.Id = id;
            await _serviceProvider.LessonService.UpdateLessonAsync(request);
            return Ok("Lesson updated successfully");
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Delete lesson", Description = "Deletes a lesson")]
        [SwaggerResponse(200, "Lesson deleted successfully.")]
        [SwaggerResponse(401, "Unauthorized.")]
        [SwaggerResponse(404, "Lesson not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            await _serviceProvider.LessonService.DeleteLessonAsync(id);
            return Ok("Lesson deleted successfully");
        }
    }
}