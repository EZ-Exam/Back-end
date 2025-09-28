using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Object.Requests;
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
    }
}
