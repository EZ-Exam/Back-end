using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Dtos;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.ServiceProvider;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/chapters")]
    [Produces("application/json")]
    public class ChapterController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;
        private readonly ILogger<ChapterController> _logger;

        public ChapterController(IServiceProviders serviceProvider, ILogger<ChapterController> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get chapters (filtered)", Description = "Optional filters: semesterId, subjectId.")]
        [SwaggerResponse(200, "Chapters retrieved successfully.", typeof(IEnumerable<ChapterDataResponse>))]
        public async Task<IActionResult> GetAllChapters(
            [FromQuery] int? semesterId = null,
            [FromQuery] int? subjectId = null)
        {
            if (semesterId.HasValue && subjectId.HasValue)
            {
                var result = await _serviceProvider.ChapterService.GetChaptersBySemesterAndSubjectAsync(semesterId.Value, subjectId.Value);
                return Ok(result);
            }
            if (semesterId.HasValue)
            {
                var chapters = await _serviceProvider.ChapterService.GetChaptersBySemesterIdAsync(semesterId.Value);
                return Ok(chapters);
            }
            var all = await _serviceProvider.ChapterService.GetAllChaptersAsync();
            return Ok(all);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get chapter by ID")]
        [SwaggerResponse(200, "Chapter found.", typeof(ChapterDataResponse))]
        [SwaggerResponse(404, "Chapter not found.")]
        public async Task<IActionResult> GetChapterById(int id)
        {
            var chapter = await _serviceProvider.ChapterService.GetChapterByIdAsync(id);
            return Ok(chapter);
        }

        [HttpPost]
        [Authorize(Policy = "SaleStaffPolicy")]
        [SwaggerOperation(Summary = "Create a new chapter", Description = "Creates a new chapter with the provided details.")]
        [SwaggerResponse(201, "Chapter created successfully.")]
        [SwaggerResponse(400, "Invalid request data.", typeof(ProblemDetails))]
        [SwaggerResponse(404, "Semester not found.", typeof(ProblemDetails))]
        [SwaggerResponse(500, "Internal server error.", typeof(ProblemDetails))]
        public async Task<IActionResult> CreateChapter([FromBody] CreateChapterRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid CreateChapterRequest.");
                return BadRequest(ModelState);
            }

            await _serviceProvider.ChapterService.CreateChapterAsync(request);
            return StatusCode(201, new { Message = "Chapter created successfully." });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "SaleStaffPolicy")]
        [SwaggerOperation(Summary = "Update chapter")]
        [SwaggerResponse(200, "Chapter updated.")]
        [SwaggerResponse(400, "Invalid request.")]
        [SwaggerResponse(404, "Chapter not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> UpdateChapter(int id, [FromBody] ChapterDataRequest request)
        {
            if (!ModelState.IsValid || id != request.Id)
                return BadRequest(new { Message = "Invalid data or ID mismatch." });

            await _serviceProvider.ChapterService.UpdateChapterAsync(request);
            return Ok(new { Message = "Chapter updated successfully." });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "SaleStaffPolicy")]
        [SwaggerOperation(Summary = "Delete a chapter", Description = "Deletes a chapter by its ID.")]
        [SwaggerResponse(204, "Chapter deleted successfully.")]
        [SwaggerResponse(404, "Chapter not found.", typeof(ProblemDetails))]
        [SwaggerResponse(500, "Internal server error.", typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            await _serviceProvider.ChapterService.DeleteChapterAsync(id);
            return NoContent();
        }
    }
}
