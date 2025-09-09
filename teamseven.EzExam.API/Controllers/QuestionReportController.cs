using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.ServiceProvider;
using teamseven.EzExam.Services.Extensions;

namespace teamseven.EzExam.Controllers
{
    [ApiController]
    [Route("api/question-reports")] 
    [Produces("application/json")]
    public class QuestionReportsController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;
        private readonly ILogger<QuestionReportsController> _logger;

        public QuestionReportsController(IServiceProviders serviceProvider, ILogger<QuestionReportsController> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all question reports")]
        [SwaggerResponse(200, "List retrieved successfully", typeof(IEnumerable<QuestionReportDataResponse>))]
        public async Task<IActionResult> GetAllReports()
        {
            var reports = await _serviceProvider.QuestionReportService.GetAllReportsAsync();
            return Ok(reports);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get question report by ID")]
        [SwaggerResponse(200, "Report found", typeof(QuestionReportDataResponse))]
        [SwaggerResponse(404, "Report not found")]
        public async Task<IActionResult> GetReportById([FromRoute] int id)
        {
            try
            {
                var report = await _serviceProvider.QuestionReportService.GetReportByIdAsync(id);
                return Ok(report);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Create a new question report")]
        [SwaggerResponse(201, "Report created successfully")]
        [SwaggerResponse(400, "Invalid request")]
        public async Task<IActionResult> CreateReport([FromBody] CreateQuestionReportRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _serviceProvider.QuestionReportService.CreateReportAsync(request);
                return StatusCode(201, new { Message = "Question report created successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating report.");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Update a question report")]
        [SwaggerResponse(200, "Report updated successfully")]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(404, "Report not found")]
        public async Task<IActionResult> UpdateReport([FromRoute] int id, [FromBody] UpdateQuestionReportRequest request)
        {
            if (!ModelState.IsValid || id != request.Id)
                return BadRequest(new { Message = "Invalid request data or ID mismatch." });

            try
            {
                await _serviceProvider.QuestionReportService.UpdateReportAsync(request);
                return Ok(new { Message = "Question report updated successfully." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating report.");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }
    }
}
