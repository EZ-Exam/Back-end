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
    [Route("api/semesters")]
    [Produces("application/json")]
    public class SemesterController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;
        private readonly ILogger<SemesterController> _logger;

        public SemesterController(IServiceProviders serviceProvider, ILogger<SemesterController> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all semesters")]
        [SwaggerResponse(200, "Semesters retrieved successfully.", typeof(IEnumerable<SemesterDataResponse>))]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetAllSemesters()
        {
            var semesters = await _serviceProvider.SemesterService.GetAllSemesterAsync();
            return Ok(semesters);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get semester by ID")]
        [SwaggerResponse(200, "Semester found.", typeof(SemesterDataResponse))]
        [SwaggerResponse(404, "Semester not found.")]
        public async Task<IActionResult> GetSemesterById(int id)
        {
            try
            {
                var semester = await _serviceProvider.SemesterService.GetSemesterByIdAsync(id);
                return Ok(semester);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet("by-grade/{gradeId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get semesters by grade ID", Description = "Returns all semesters for a given grade ID.")]
        [SwaggerResponse(200, "Semesters found.", typeof(IEnumerable<SemesterDataResponse>))]
        [SwaggerResponse(404, "Grade not found.")]
        public async Task<IActionResult> GetSemestersByGradeId(int gradeId)
        {
            try
            {
                var result = await _serviceProvider.SemesterService.GetSemesterByGradeIdAsync(gradeId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return NotFound(new { Message = ex.Message });
            }
        }


        [HttpPost]
        [SwaggerOperation(Summary = "Create a new semester", Description = "Creates a new semester with the provided details.")]
        [SwaggerResponse(201, "Semester created successfully.")]
        [SwaggerResponse(400, "Invalid request data.", typeof(ProblemDetails))]
        [SwaggerResponse(404, "Grade not found.", typeof(ProblemDetails))]
        [SwaggerResponse(500, "Internal server error.", typeof(ProblemDetails))]
        public async Task<IActionResult> CreateSemester([FromBody] CreateSemesterRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid CreateSemesterRequest.");
                return BadRequest(ModelState);
            }

            try
            {
                await _serviceProvider.SemesterService.CreateSemesterAsync(request);
                return StatusCode(201, new { Message = "Semester created successfully." });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while creating the semester." });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "SaleStaffPolicy")]
        [SwaggerOperation(Summary = "Update semester")]
        [SwaggerResponse(200, "Semester updated.")]
        [SwaggerResponse(400, "Invalid request.")]
        [SwaggerResponse(404, "Semester not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> UpdateSemester(int id, [FromBody] SemesterDataRequest request)
        {
            if (!ModelState.IsValid || id != request.Id)
                return BadRequest(new { Message = "Invalid data or ID mismatch." });

            try
            {
                await _serviceProvider.SemesterService.UpdateSemesterAsync(request);
                return Ok(new { Message = "Semester updated successfully." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating semester.");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a semester", Description = "Deletes a semester by its ID.")]
        [SwaggerResponse(204, "Semester deleted successfully.")]
        [SwaggerResponse(404, "Semester not found.", typeof(ProblemDetails))]
        [SwaggerResponse(500, "Internal server error.", typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteSemester(int id)
        {
            try
            {
                await _serviceProvider.SemesterService.DeleteSemesterAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while deleting the semester." });
            }
        }
    }
}
