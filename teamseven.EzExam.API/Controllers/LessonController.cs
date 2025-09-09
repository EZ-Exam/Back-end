using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using teamseven.EzExam.Services;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.ServiceProvider;

namespace teamseven.EzExam.Controllers
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
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(
                   Summary = "Get lessons",
                   Description = "Retrieves a list of lessons with optional search, sort, filter, and pagination. Use 'search' to filter by name (e.g., 'chuy?n d?ng'), 'chapterId' to filter by chapter, 'isSort' (0 = no sort, 1 = sort), 'sort' (e.g., 'name:asc', 'createdAt:desc'), and 'pageNumber'/'pageSize' for pagination. If 'isSort' is 0 or not provided, lessons are sorted by 'Id' (ascending). If 'isSort' is 1, 'sort' parameter is used, defaulting to 'createdAt:desc' if 'sort' is invalid or not provided."
               )]
        [SwaggerResponse(200, "Lessons retrieved successfully.", typeof(PagedResponse<LessonDataResponse>))]
        [SwaggerResponse(400, "Invalid parameters.", typeof(ProblemDetails))]
        [SwaggerResponse(500, "Internal server error.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetLessons(
                   [FromQuery] string? search = null,
                   [FromQuery] string? sort = null,
                   [FromQuery] int? chapterId = null,
                   [FromQuery] int? pageNumber = null,
                   [FromQuery] int? pageSize = null,
                   [FromQuery] int isSort = 0)
        {
            try
            {
                // Validate pagination parameters
                if (pageNumber.HasValue && pageNumber < 1 || pageSize.HasValue && pageSize < 1)
                {
                    _logger.LogWarning("Invalid pagination parameters: pageNumber={PageNumber}, pageSize={PageSize}.", pageNumber, pageSize);
                    return BadRequest(new { Message = "pageNumber and pageSize must be greater than 0." });
                }

                // Validate isSort
                if (isSort != 0 && isSort != 1)
                {
                    _logger.LogWarning("Invalid isSort parameter: {IsSort}.", isSort);
                    return BadRequest(new { Message = "isSort must be 0 or 1." });
                }

                // Validate sort parameter when isSort=1
                if (isSort == 1 && !string.IsNullOrEmpty(sort) && !IsValidSortParameter(sort))
                {
                    _logger.LogWarning("Invalid sort parameter: {Sort}.", sort);
                    return BadRequest(new { Message = "Invalid sort parameter. Use format 'field:asc' or 'field:desc' with valid fields (name, createdAt, updatedAt)." });
                }

                var pagedLessons = await _serviceProvider.LessonService.GetLessonsAsync(
                    pageNumber,
                    pageSize,
                    search,
                    sort,
                    chapterId,
                    isSort);

                _logger.LogInformation("Retrieved {Count} lessons for page {PageNumber}.", pagedLessons.Items.Count, pagedLessons.PageNumber);
                return Ok(pagedLessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lessons: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving lessons." });
            }
        }


        private bool IsValidSortParameter(string sort)
        {
            var validFields = new[] { "name", "createdat", "updatedat" };
            var validOrders = new[] { "asc", "desc" };
            var parts = sort.ToLower().Split(':');
            return parts.Length == 2 && validFields.Contains(parts[0]) && validOrders.Contains(parts[1]);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get lesson by ID", Description = "Retrieves a lesson by its ID.")]
        [SwaggerResponse(200, "Lesson retrieved successfully.", typeof(LessonDataResponse))]
        [SwaggerResponse(404, "Lesson not found.", typeof(ProblemDetails))]
        [SwaggerResponse(500, "Internal server error.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetLessonById(int id)
        {
            try
            {
                var result = await _serviceProvider.LessonService.GetLessonByIdAsync(id);
                _logger.LogInformation("Lesson with ID {LessonId} retrieved successfully.", id);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found: {Message}", ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lesson with ID {LessonId}: {Message}", id, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving lesson." });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Create a new lesson", Description = "Creates a new lesson under the specified chapter.")]
        [SwaggerResponse(201, "Lesson created successfully.", typeof(LessonDataResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(ProblemDetails))]
        [SwaggerResponse(404, "Chapter not found.", typeof(ProblemDetails))]
        [SwaggerResponse(500, "Internal server error.", typeof(ProblemDetails))]
        public async Task<IActionResult> CreateLesson([FromBody] CreateLessonRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid request data for creating lesson.");
                return BadRequest(ModelState);
            }

            try
            {
                await _serviceProvider.LessonService.CreateLessonAsync(request);
                _logger.LogInformation("Lesson created successfully.");
                return StatusCode(201, new { Message = "Lesson created successfully." });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found: {Message}", ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lesson: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while creating lesson." });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "SaleStaffPolicy")]
        [SwaggerOperation(Summary = "Update a lesson", Description = "Updates a lesson by ID.")]
        [SwaggerResponse(200, "Lesson updated successfully.", typeof(LessonDataResponse))]
        [SwaggerResponse(400, "Invalid input.", typeof(ProblemDetails))]
        [SwaggerResponse(404, "Lesson not found.", typeof(ProblemDetails))]
        [SwaggerResponse(500, "Internal server error.", typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] LessonDataRequest request)
        {
            if (!ModelState.IsValid || id != request.Id)
            {
                _logger.LogWarning("Invalid request data or ID mismatch for updating lesson with ID {LessonId}.", id);
                return BadRequest(new { Message = "Invalid input or ID mismatch." });
            }

            try
            {
                await _serviceProvider.LessonService.UpdateLessonAsync(request);
                _logger.LogInformation("Lesson with ID {LessonId} updated successfully.", id);
                return Ok(new { Message = "Lesson updated successfully." });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found: {Message}", ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lesson with ID {LessonId}: {Message}", id, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while updating lesson." });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "SaleStaffPolicy")]
        [SwaggerOperation(Summary = "Delete a lesson", Description = "Deletes a lesson by its ID.")]
        [SwaggerResponse(204, "Lesson deleted successfully.")]
        [SwaggerResponse(404, "Lesson not found.", typeof(ProblemDetails))]
        [SwaggerResponse(500, "Internal server error.", typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            try
            {
                await _serviceProvider.LessonService.DeleteLessonAsync(id);
                _logger.LogInformation("Lesson with ID {LessonId} deleted successfully.", id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found: {Message}", ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lesson with ID {LessonId}: {Message}", id, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while deleting lesson." });
            }
        }
    }
}
