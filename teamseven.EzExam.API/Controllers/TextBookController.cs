using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.ServiceProvider;

namespace teamseven.EzExam.Controllers
{
    [ApiController]
    [Route("api/text-books")]
    [Produces("application/json")]
    public class TextBookController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;
        private readonly ILogger<TextBookController> _logger;

        public TextBookController(IServiceProviders serviceProvider, ILogger<TextBookController> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all textbooks")]
        [SwaggerResponse(200, "Textbooks retrieved successfully", typeof(IEnumerable<TextBookDataResponse>))]
        public async Task<IActionResult> GetAll()
        {
            var result = await _serviceProvider.TextBookService.GetAllTextBookAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get textbook by ID")]
        [SwaggerResponse(200, "Textbook found", typeof(TextBookDataResponse))]
        [SwaggerResponse(404, "Textbook not found")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _serviceProvider.TextBookService.GetTextBookByIdAsync(id);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "SaleStaffPolicy")]
        [SwaggerOperation(Summary = "Create a new textbook")]
        [SwaggerResponse(201, "Textbook created successfully")]
        [SwaggerResponse(400, "Invalid input")]
        public async Task<IActionResult> Create([FromBody] CreateTextBookRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceProvider.TextBookService.CreateTextBookAsync(request);
            return StatusCode(201, new { Message = "Textbook created successfully." });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "SaleStaffPolicy")]
        [SwaggerOperation(Summary = "Update textbook")]
        [SwaggerResponse(200, "Textbook updated")]
        [SwaggerResponse(404, "Textbook not found")]
        public async Task<IActionResult> Update(int id, [FromBody] TextBookDataRequest request)
        {
            if (!ModelState.IsValid || request.Id != id)
                return BadRequest(new { Message = "Invalid request." });

            try
            {
                await _serviceProvider.TextBookService.UpdateTextBookAsync(request);
                return Ok(new { Message = "Textbook updated successfully." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "SaleStaffPolicy")]
        [SwaggerOperation(Summary = "Delete textbook")]
        [SwaggerResponse(204, "Textbook deleted")]
        [SwaggerResponse(404, "Textbook not found")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _serviceProvider.TextBookService.DeleteTextBookAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
