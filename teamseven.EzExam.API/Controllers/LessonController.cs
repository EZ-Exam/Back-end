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

        // =================== GET ALL LESSONS ===================

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all lessons", Description = "Retrieves all lessons")]
        [SwaggerResponse(200, "Lessons retrieved successfully.", typeof(IEnumerable<LessonDataResponse>))]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetAllLessons()
        {
            try
            {
                var lessons = await _serviceProvider.LessonService.GetAllLessonAsync();
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all lessons");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("by-chapter/{chapterId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get lessons by chapter ID", Description = "Retrieves lessons filtered by chapter ID")]
        [SwaggerResponse(200, "Lessons retrieved successfully.", typeof(IEnumerable<LessonDataResponse>))]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetLessonsByChapterId(int chapterId)
        {
            try
            {
                var lessons = await _serviceProvider.LessonService.GetLessonsByChapterIdAsync(chapterId);
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lessons by chapterId {ChapterId}", chapterId);
                return StatusCode(500, "Internal server error");
            }
        }

        // =================== GET LESSONS WITH PAGINATION ===================

        [HttpGet("paged")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get lessons with pagination", Description = "Retrieves lessons with pagination and filtering")]
        [SwaggerResponse(200, "Lessons retrieved successfully.", typeof(PagedResponse<LessonDataResponse>))]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetLessons(
            [FromQuery] int? pageNumber = null,
            [FromQuery] int? pageSize = null,
            [FromQuery] string? search = null,
            [FromQuery] string? sort = null,
            [FromQuery] int? chapterId = null,
            [FromQuery] int isSort = 0)
        {
            try
            {
                var lessons = await _serviceProvider.LessonService.GetLessonsAsync(
                    pageNumber, pageSize, search, sort, chapterId, isSort);
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lessons with pagination");
                return StatusCode(500, "Internal server error");
            }
        }

        // =================== GET LESSON BY ID ===================

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get lesson by ID", Description = "Retrieves a specific lesson by its ID")]
        [SwaggerResponse(200, "Lesson retrieved successfully.", typeof(LessonDataResponse))]
        [SwaggerResponse(404, "Lesson not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetLessonById(int id)
        {
            try
            {
                var lesson = await _serviceProvider.LessonService.GetLessonByIdAsync(id);
                return Ok(lesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lesson with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // =================== CREATE LESSON ===================

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Create new lesson", Description = "Creates a new lesson")]
        [SwaggerResponse(201, "Lesson created successfully.", typeof(LessonDataResponse))]
        [SwaggerResponse(400, "Bad request.")]
        [SwaggerResponse(401, "Unauthorized.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> CreateLesson([FromBody] CreateLessonRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _serviceProvider.LessonService.CreateLessonAsync(request);
                return StatusCode(201, "Lesson created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lesson");
                return StatusCode(500, "Internal server error");
            }
        }

        // =================== UPDATE LESSON ===================

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
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                request.Id = id;
                await _serviceProvider.LessonService.UpdateLessonAsync(request);
                return Ok("Lesson updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lesson with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // =================== DELETE LESSON ===================

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Delete lesson", Description = "Deletes a lesson")]
        [SwaggerResponse(200, "Lesson deleted successfully.")]
        [SwaggerResponse(401, "Unauthorized.")]
        [SwaggerResponse(404, "Lesson not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            try
            {
                await _serviceProvider.LessonService.DeleteLessonAsync(id);
                return Ok("Lesson deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lesson with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}