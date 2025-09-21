using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Services.ServiceProvider;
using teamseven.EzExam.Services.Extensions;
using Microsoft.AspNetCore.Authorization;
using teamseven.EzExam.Services.Object.Responses;
using Microsoft.Extensions.DependencyInjection;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/solutions")]
    [Produces("application/json")]
    public class SolutionController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;
        private readonly ILogger<SolutionController> _logger;

        public SolutionController(IServiceProviders serviceProvider, ILogger<SolutionController> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all solutions", Description = "Retrieves all solutions.")]
        [SwaggerResponse(200, "Solutions retrieved successfully.", typeof(IEnumerable<SolutionDataResponse>))]
        public async Task<IActionResult> GetAll()
        {
            var data = await _serviceProvider.SolutionService.GetAllSolutionsAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get solution by ID", Description = "Retrieves a solution by ID.")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var data = await _serviceProvider.SolutionService.GetSolutionByIdAsync(id);
                return Ok(data);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Create a new solution", Description = "Creates a new solution.")]
        public async Task<IActionResult> Create([FromBody] CreateSolutionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceProvider.SolutionService.CreateSolutionAsync(request);
            return StatusCode(201, new { Message = "Solution created successfully." });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "SaleStaffPolicy")]
        [SwaggerOperation(Summary = "Update a solution", Description = "Updates a solution.")]
        public async Task<IActionResult> Update(int id, [FromBody] SolutionDataRequest request)
        {
            if (!ModelState.IsValid || request.Id != id)
                return BadRequest(new { Message = "Invalid request data." });

            try
            {
                await _serviceProvider.SolutionService.UpdateSolutionAsync(request);
                return Ok(new { Message = "Solution updated successfully." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "SaleStaffPolicy")]
        [SwaggerOperation(Summary = "Delete a solution", Description = "Deletes a solution.")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _serviceProvider.SolutionService.DeleteSolutionAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        [HttpPost("add-with-video")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Create a new video", Description = "Creates a new video.")]
        public async Task<IActionResult> AddSolutionWithVideo([FromForm] SolutionWithVideoRequest request)
        {
            await _serviceProvider.SolutionService.AddSolutionWithVideoAsync(request);
            return Ok(new { message = "Success" });
        }
        [HttpGet("{id}/video")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get video by solution ID", Description = "Returns the video file associated with the solution.")]
        public async Task<IActionResult> GetSolutionVideo(int id)
        {
            try
            {
                var solution = await _serviceProvider.SolutionService.GetSolutionByIdAsync(id);

                if (solution.VideoData == null || string.IsNullOrEmpty(solution.VideoContentType))
                {
                    return NotFound(new { Message = "Video not found for this solution." });
                }

                return File(solution.VideoData, solution.VideoContentType);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { Message = ex.Message });
            }
        }

    }
}
