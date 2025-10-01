using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Dtos;
using teamseven.EzExam.Services;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.ServiceProvider;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/questions")]
    [Produces("application/json")]
    public class QuestionsController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;
        private readonly ILogger<QuestionsController> _logger;

        public QuestionsController(IServiceProviders serviceProvider, ILogger<QuestionsController> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(
             Summary = "Get questions",
             Description = "Retrieves a list of questions with optional search, sort, filter, and pagination. Use 'search' to filter by content or source (accent-insensitive, e.g., 'lam' matches 'l�m'), 'lessonId' to filter by lesson, 'difficultyLevel' to filter by difficulty, 'chapterId' to filter by chapter, 'isSort' (0 = no sort, 1 = sort), 'sort' (e.g., 'content:asc', 'createdAt:desc'), and 'pageNumber'/'pageSize' for pagination. If 'isSort' is 0 or not provided, questions are sorted by 'Id' (ascending). If 'isSort' is 1, 'sort' parameter is used, defaulting to 'createdAt:desc' if 'sort' is invalid or not provided."
         )]
        [SwaggerResponse(200, "Questions retrieved successfully.", typeof(PagedResponse<QuestionDataResponse>))]
        [SwaggerResponse(400, "Invalid parameters.", typeof(ProblemDetails))]
        [SwaggerResponse(500, "Internal server error.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetQuestions(
             [FromQuery] string? search = null,
             [FromQuery] string? sort = null,
             [FromQuery] int? lessonId = null,
             [FromQuery] string? difficultyLevel = null,
             [FromQuery] int? chapterId = null,
             [FromQuery] int? pageNumber = null,
             [FromQuery] int? pageSize = null,
             [FromQuery] int isSort = 0,
             [FromQuery] int? userId = null,
             [FromQuery] int? textbookId = null) 
        {
            try
            {
                if (pageNumber.HasValue && pageNumber < 1 || pageSize.HasValue && pageSize < 1)
                {
                    _logger.LogWarning("Invalid pagination parameters: pageNumber={PageNumber}, pageSize={PageSize}.", pageNumber, pageSize);
                    return BadRequest(new { Message = "pageNumber and pageSize must be greater than 0." });
                }

                if (isSort != 0 && isSort != 1)
                {
                    _logger.LogWarning("Invalid isSort parameter: {IsSort}.", isSort);
                    return BadRequest(new { Message = "isSort must be 0 or 1." });
                }

                if (isSort == 1 && !string.IsNullOrEmpty(sort) && !IsValidSortParameter(sort))
                {
                    _logger.LogWarning("Invalid sort parameter: {Sort}.", sort);
                    return BadRequest(new { Message = "Invalid sort parameter. Use format 'field:asc' or 'field:desc' with valid fields (content, difficultyLevel, createdAt, updatedAt)." });
                }

                var pagedQuestions = await _serviceProvider.QuestionsService.GetQuestionsAsync(
                    pageNumber,
                    pageSize,
                    search,
                    sort,
                    lessonId,
                    difficultyLevel,
                    chapterId,
                    isSort,
                    userId,
                    textbookId);
                _logger.LogInformation("Retrieved {Count} questions for page {PageNumber}.", pagedQuestions.Items.Count, pagedQuestions.PageNumber);
                return Ok(pagedQuestions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving questions." });
            }
        }
        private bool IsValidSortParameter(string sort)
        {
            var validFields = new[] { "content", "difficultylevel", "createdat", "updatedat" };
            var validOrders = new[] { "asc", "desc" };
            var parts = sort.ToLower().Split(':');
            return parts.Length == 2 && validFields.Contains(parts[0]) && validOrders.Contains(parts[1]);
        }



        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Create a new question", Description = "Creates a new question with the provided details.")]
        [SwaggerResponse(201, "Question created successfully.", typeof(QuestionDataResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(ProblemDetails))]
        [SwaggerResponse(404, "Lesson or user not found.", typeof(ProblemDetails))]
        [SwaggerResponse(500, "Internal server error.", typeof(ProblemDetails))]
        public async Task<IActionResult> AddQuestion([FromBody] QuestionDataRequest questionDataRequest)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid request data for creating question.");
                return BadRequest(ModelState);
            }

            try
            {
                var questionResponse = await _serviceProvider.QuestionsService.AddQuestionAsync(questionDataRequest);
                _logger.LogInformation("Question created successfully with ID {QuestionId}.", questionResponse.Id);
                var result = new
                {
                    question = new
                    {
                        id = questionResponse.Id,
                        content = questionResponse.Content,
                        questionSource = questionResponse.QuestionSource,
                        difficultyLevel = questionResponse.DifficultyLevel,
                        lessonId = questionResponse.LessonId,
                        textbookId = questionResponse.TextbookId,
                        createdByUserId = questionResponse.CreatedByUserId,
                        createdAt = questionResponse.CreatedAt,
                        updatedAt = questionResponse.UpdatedAt,
                        formula = questionResponse.Formula,
                        correctAnswer = questionResponse.CorrectAnswer,
                        explanation = questionResponse.Explanation,
                        type = questionResponse.Type,
                        options = questionResponse.Options
                    },
                    message = "Question created successfully."
                };

                return StatusCode(201, result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found: {Message}", ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while creating question." });
            }
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Update a question", Description = "Updates a question by its ID.")]
        [SwaggerResponse(200, "Question updated successfully.", typeof(QuestionDataResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(ProblemDetails))]
        [SwaggerResponse(404, "Question not found.", typeof(ProblemDetails))]
        [SwaggerResponse(500, "Internal server error.", typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] UpdateQuestionRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid request data for updating question.");
                return BadRequest(ModelState);
            }

            if (id != request.Id)
            {
                _logger.LogWarning("Mismatch between route ID and body ID.");
                return BadRequest(new { Message = "Route ID and request ID do not match." });
            }

            try
            {
                var updatedQuestion = await _serviceProvider.QuestionsService.ModifyQuestionAsync(request);
                _logger.LogInformation("Question with ID {QuestionId} updated successfully.", request.Id);
                return Ok(updatedQuestion);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found: {Message}", ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while updating the question." });
            }
        }


        [HttpDelete("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Delete a question", Description = "Deletes a question by its ID.")]
        [SwaggerResponse(204, "Question deleted successfully.")]
        [SwaggerResponse(404, "Question not found.", typeof(ProblemDetails))]
        [SwaggerResponse(500, "Internal server error.", typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            try
            {
                await _serviceProvider.QuestionsService.DeleteQuestionAsync(id);
                _logger.LogInformation("Question with ID {QuestionId} deleted successfully.", id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found: {Message}", ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting question with ID {QuestionId}: {Message}", id, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while deleting question." });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get question by ID", Description = "Retrieves a question by its ID.")]
        [SwaggerResponse(200, "Question retrieved successfully.", typeof(QuestionDataResponse))]
        [SwaggerResponse(404, "Question not found.", typeof(ProblemDetails))]
        [SwaggerResponse(500, "Internal server error.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetQuestion(int id)
        {
            try
            {
                var question = await _serviceProvider.QuestionsService.GetQuestionById(id);
                _logger.LogInformation("Question with ID {QuestionId} retrieved successfully.", id);
                return Ok(question);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found: {Message}", ex.Message);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question with ID {QuestionId}: {Message}", id, ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving question." });
            }
        }
    }
}
