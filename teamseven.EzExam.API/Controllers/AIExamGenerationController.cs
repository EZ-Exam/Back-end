using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Interfaces;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/ai-exam")]
    [Produces("application/json")]
    [Authorize] // Yêu cầu authentication
    public class AIExamGenerationController : ControllerBase
    {
        private readonly IAIExamGenerationService _aiExamGenerationService;
        private readonly ILogger<AIExamGenerationController> _logger;

        public AIExamGenerationController(
            IAIExamGenerationService aiExamGenerationService,
            ILogger<AIExamGenerationController> logger)
        {
            _aiExamGenerationService = aiExamGenerationService ?? throw new ArgumentNullException(nameof(aiExamGenerationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("generate")]
        [SwaggerOperation(
            Summary = "Generate AI-powered exam",
            Description = "Tạo đề thi tự động bằng AI dựa trên lịch sử học tập và năng lực của học sinh"
        )]
        [SwaggerResponse(200, "Exam generated successfully", typeof(GenerateAIExamResponse))]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GenerateExam([FromBody] GenerateAIExamRequest request)
        {
            try
            {
                _logger.LogInformation("AI exam generation request received for UserId: {UserId}, Mode: {Mode}, QuestionCount: {QuestionCount}", 
                    request.UserId, request.Mode, request.QuestionCount);

                // Validate request
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid request parameters",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                // Validate mode
                if (request.Mode != "review" && request.Mode != "advanced")
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Mode must be either 'review' or 'advanced'"
                    });
                }

                // Validate question count
                if (request.QuestionCount < 1 || request.QuestionCount > 50)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "QuestionCount must be between 1 and 50"
                    });
                }

                // Generate exam
                var result = await _aiExamGenerationService.GenerateExamAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("AI exam generated successfully for UserId: {UserId}. Generated {Count} questions", 
                        request.UserId, result.Questions.Count);

                    return Ok(new
                    {
                        Success = true,
                        Data = result,
                        Message = result.Message
                    });
                }
                else
                {
                    _logger.LogWarning("AI exam generation failed for UserId: {UserId}. Reason: {Message}", 
                        request.UserId, result.Message);

                    return BadRequest(new
                    {
                        Success = false,
                        Message = result.Message,
                        Data = result
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating AI exam for UserId: {UserId}", request.UserId);
                
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi tạo đề thi. Vui lòng thử lại sau.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("user/{userId}/history")]
        [SwaggerOperation(
            Summary = "Get user exam history",
            Description = "Lấy lịch sử làm bài gần nhất của user để phục vụ AI analysis"
        )]
        [SwaggerResponse(200, "History retrieved successfully", typeof(List<ExamHistoryResponse>))]
        [SwaggerResponse(400, "Invalid user ID")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetUserExamHistory(
            [FromRoute] int userId,
            [FromQuery] int count = 5)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                if (count < 1 || count > 10)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Count must be between 1 and 10"
                    });
                }

                var history = await _aiExamGenerationService.GetUserExamHistoryAsync(userId, count);

                return Ok(new
                {
                    Success = true,
                    Data = history,
                    Total = history.Count,
                    Message = "Exam history retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam history for UserId: {UserId}", userId);
                
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy lịch sử làm bài.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("questions/available")]
        [SwaggerOperation(
            Summary = "Get available questions for AI selection",
            Description = "Lấy danh sách câu hỏi có sẵn để AI có thể chọn lựa"
        )]
        [SwaggerResponse(200, "Questions retrieved successfully", typeof(List<QuestionSimpleResponse>))]
        [SwaggerResponse(400, "Invalid parameters")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetAvailableQuestions(
            [FromQuery] int? gradeId = null,
            [FromQuery] int? lessonId = null,
            [FromQuery] string? difficultyLevel = null,
            [FromQuery] string? subject = null)
        {
            try
            {
                var request = new GenerateAIExamRequest
                {
                    UserId = 0, // Not used for this endpoint
                    QuestionCount = 0, // Not used for this endpoint
                    Mode = "review", // Not used for this endpoint
                    GradeId = gradeId,
                    LessonId = lessonId,
                    DifficultyLevel = difficultyLevel,
                    Subject = subject
                };

                var questions = await _aiExamGenerationService.GetAvailableQuestionsAsync(request);

                return Ok(new
                {
                    Success = true,
                    Data = questions,
                    Total = questions.Count,
                    Message = "Available questions retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available questions");
                
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy danh sách câu hỏi.",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("validate-request")]
        [SwaggerOperation(
            Summary = "Validate AI exam generation request",
            Description = "Kiểm tra tính hợp lệ của request trước khi gọi AI"
        )]
        [SwaggerResponse(200, "Request is valid")]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Unauthorized")]
        public IActionResult ValidateRequest([FromBody] GenerateAIExamRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid request parameters",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                // Additional validation
                var validationErrors = new List<string>();

                if (request.Mode != "review" && request.Mode != "advanced")
                {
                    validationErrors.Add("Mode must be either 'review' or 'advanced'");
                }

                if (request.QuestionCount < 1 || request.QuestionCount > 50)
                {
                    validationErrors.Add("QuestionCount must be between 1 and 50");
                }

                if (request.UserId <= 0)
                {
                    validationErrors.Add("UserId must be a positive integer");
                }

                if (request.HistoryCount < 1 || request.HistoryCount > 5)
                {
                    validationErrors.Add("HistoryCount must be between 1 and 5");
                }

                if (validationErrors.Any())
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = validationErrors
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Request is valid",
                    Data = new
                    {
                        UserId = request.UserId,
                        QuestionCount = request.QuestionCount,
                        Mode = request.Mode,
                        HistoryCount = request.HistoryCount,
                        Filters = new
                        {
                            GradeId = request.GradeId,
                            LessonId = request.LessonId,
                            DifficultyLevel = request.DifficultyLevel,
                            Subject = request.Subject
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating request");
                
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi kiểm tra request.",
                    Error = ex.Message
                });
            }
        }
    }
}
