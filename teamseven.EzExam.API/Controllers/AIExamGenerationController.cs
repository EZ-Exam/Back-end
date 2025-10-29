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
            Summary = "🎓 Generate AI-powered exam and save to database",
            Description = @"
**Tạo đề thi tự động bằng AI** dựa trên lịch sử học tập của học sinh. Đề thi sẽ được **tự động lưu vào database**.

---

### 📋 **Request Body:**

```json
{
  ""userId"": 8,
  ""questionCount"": 10,
  ""mode"": ""review"",
  ""historyCount"": 5,
  ""subjectIds"": [1, 2],      // Optional - Chọn nhiều môn (Toán, Vật lý)
  ""gradeIds"": [10, 11],      // Optional - Chọn nhiều khối (10, 11)
  ""chapterIds"": [1, 2, 3],   // Optional - Chọn nhiều chương cụ thể
  ""lessonIds"": [1, 5, 10],   // Optional - Chọn nhiều bài cụ thể
  ""difficultyLevelId"": 2     // Optional - 1=EASY, 2=MEDIUM, 3=HARD
}
```

---

### 🎯 **QUY TẮC:**

#### **1. SubjectIds (Môn học) - Array:**
- **User chỉ định** → AI **BẮT BUỘC tuân theo**, KHÔNG auto-detect
- **User không chỉ định** → Tự động detect từ lịch sử thi
- **Ví dụ:** `[1, 2, 3]` = Gen đề từ Toán, Vật lý, Hóa học

#### **2. GradeIds (Khối lớp) - Array:**
- **User chỉ định** → AI **BẮT BUỘC tuân theo**, KHÔNG auto-detect
- **User không chỉ định** → Tự động detect từ lịch sử thi
- **Ví dụ:** `[10, 11, 12]` = Gen đề từ lớp 10, 11, 12

#### **3. ChapterIds (Chương) - Array:**
- **User chỉ định** → AI **BẮT BUỘC chỉ lấy từ các chương đó**
- **User không chỉ định** → Lấy từ tất cả chương trong môn
- **Ví dụ:** `[1, 2, 3]` = Chỉ gen đề từ chương 1, 2, 3
- **Hierarchy:** Grade → Subject → Chapter → Lesson

#### **4. LessonIds (Bài học) - Array:**
- **User chỉ định** → AI **BẮT BUỘC chỉ lấy từ các bài đó**
- **User không chỉ định** → Lấy từ tất cả bài trong chương/môn
- **Ví dụ:** `[1, 5, 10]` = Chỉ gen đề từ bài 1, 5, 10

#### **5. DifficultyLevelId (Độ khó):**
- **1** = EASY (Dễ)
- **2** = MEDIUM (Trung bình)
- **3** = HARD (Khó)
- **null** = Tất cả độ khó

#### **6. Mode (Chế độ):**
- **review** = Ôn tập (ưu tiên câu dễ-trung bình)
- **advanced** = Nâng cao (ưu tiên câu trung bình-khó)

---

### 📤 **Response:**

```json
{
  ""success"": true,
  ""examId"": 123,  // ID đề thi đã lưu trong DB
  ""data"": {
    ""questions"": [...],
    ""metadata"": {
      ""examId"": 123,
      ""totalQuestions"": 10,
      ""processingTimeSeconds"": 5.2,
      ""analysis"": ""AI analysis...""
    }
  }
}
```

---

### ✨ **Tính năng:**
✅ Tự động lưu đề thi vào database  
✅ Hỗ trợ chọn **nhiều môn, nhiều lớp, nhiều bài** (combobox)  
✅ AI phân tích lịch sử để đề xuất câu hỏi phù hợp  
✅ **Nếu user chỉ định → AI BẮT BUỘC tuân theo**  
✅ **Nếu user không chỉ định → Auto-detect thông minh**
"
        )]
        [SwaggerResponse(200, "Exam generated and saved successfully", typeof(GenerateAIExamResponse))]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GenerateExam([FromBody] GenerateAIExamRequest request)
        {
            try
            {
                _logger.LogInformation("🚀 AI exam generation request received for UserId: {UserId}, Mode: {Mode}, QuestionCount: {QuestionCount}", 
                    request.UserId, request.Mode, request.QuestionCount);
                _logger.LogInformation("📋 Request details - SubjectIds: [{SubjectIds}], GradeIds: [{GradeIds}], ChapterIds: [{ChapterIds}], LessonIds: [{LessonIds}], DifficultyLevelId: {DifficultyLevelId}, HistoryCount: {HistoryCount}", 
                    request.SubjectIds != null && request.SubjectIds.Any() ? string.Join(", ", request.SubjectIds) : "Auto-detect",
                    request.GradeIds != null && request.GradeIds.Any() ? string.Join(", ", request.GradeIds) : "Auto-detect",
                    request.ChapterIds != null && request.ChapterIds.Any() ? string.Join(", ", request.ChapterIds) : "All chapters",
                    request.LessonIds != null && request.LessonIds.Any() ? string.Join(", ", request.LessonIds) : "All lessons",
                    request.DifficultyLevelId?.ToString() ?? "All",
                    request.HistoryCount);

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

                // Generate exam and save to database
                var result = await _aiExamGenerationService.GenerateExamAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("🎉 AI exam generated and saved successfully for UserId: {UserId}. ExamId: {ExamId}, Questions: {Count}", 
                        request.UserId, result.Metadata.ExamId, result.Questions.Count);
                    _logger.LogInformation("📊 Final result - Processing time: {Time}s, Mode: {Mode}, Total questions: {TotalQuestions}", 
                        result.Metadata.ProcessingTimeSeconds, result.Metadata.Mode, result.Metadata.TotalQuestions);

                    return Ok(new
                    {
                        Success = true,
                        Data = result,
                        Message = result.Message,
                        ExamId = result.Metadata.ExamId,
                        Note = "Exam has been saved to database and ready to use"
                    });
                }
                else
                {
                    _logger.LogWarning("❌ AI exam generation failed for UserId: {UserId}. Reason: {Message}", 
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
                _logger.LogError(ex, "❌ Error generating AI exam for UserId: {UserId}. Error: {ErrorMessage}", request.UserId, ex.Message);
                
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
            Summary = "Get user exam history (minimal data)",
            Description = "Lấy lịch sử làm bài gần nhất với data tối thiểu (bao gồm Subject, Grade, Lesson). Mặc định lấy 5 lần, có thể tùy chỉnh từ 1-10 lần."
        )]
        [SwaggerResponse(200, "History retrieved successfully", typeof(List<ExamHistoryMinimalResponse>))]
        [SwaggerResponse(400, "Invalid user ID")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetUserExamHistory(
            [FromRoute] int userId,
            [FromQuery] int count = 5)
        {
            try
            {
                _logger.LogInformation("📋 Retrieving exam history for UserId: {UserId}, Count: {Count}", userId, count);

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

                // Get minimal history data
                var history = await _aiExamGenerationService.GetUserExamHistoryMinimalAsync(userId, count);

                _logger.LogInformation("✅ Retrieved {Count} exam history records for UserId: {UserId}", history.Count, userId);

                return Ok(new
                {
                    Success = true,
                    Data = history,
                    Total = history.Count,
                    Message = "Exam history retrieved successfully",
                    Note = "Data includes Subject, Grade, and Lesson information for AI analysis"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving exam history for UserId: {UserId}. Error: {ErrorMessage}", userId, ex.Message);
                
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
            Summary = "Get available questions (minimal data)",
            Description = "Lấy danh sách câu hỏi có sẵn với data tối thiểu. GradeId và Subject sẽ tự động phát hiện từ lịch sử nếu không cung cấp."
        )]
        [SwaggerResponse(200, "Questions retrieved successfully", typeof(List<QuestionMinimalResponse>))]
        [SwaggerResponse(400, "Invalid parameters")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetAvailableQuestions(
            [FromQuery] List<int>? subjectIds = null,
            [FromQuery] List<int>? gradeIds = null,
            [FromQuery] List<int>? chapterIds = null,
            [FromQuery] List<int>? lessonIds = null,
            [FromQuery] int? difficultyLevelId = null,
            [FromQuery] int? userId = null)
        {
            try
            {
                _logger.LogInformation("🔍 Retrieving available questions - SubjectIds: [{SubjectIds}], GradeIds: [{GradeIds}], ChapterIds: [{ChapterIds}], LessonIds: [{LessonIds}], DifficultyLevelId: {DifficultyLevelId}, UserId: {UserId}", 
                    subjectIds != null && subjectIds.Any() ? string.Join(", ", subjectIds) : "null",
                    gradeIds != null && gradeIds.Any() ? string.Join(", ", gradeIds) : "null",
                    chapterIds != null && chapterIds.Any() ? string.Join(", ", chapterIds) : "null",
                    lessonIds != null && lessonIds.Any() ? string.Join(", ", lessonIds) : "null",
                    difficultyLevelId?.ToString() ?? "null",
                    userId);

                var request = new GenerateAIExamRequest
                {
                    UserId = userId ?? 0,
                    QuestionCount = 100, // Get more questions for selection
                    Mode = "review",
                    SubjectIds = subjectIds,
                    GradeIds = gradeIds,
                    ChapterIds = chapterIds,
                    LessonIds = lessonIds,
                    DifficultyLevelId = difficultyLevelId
                };

                // Get minimal question data  
                var questions = await _aiExamGenerationService.GetAvailableQuestionsAsync(request);

                _logger.LogInformation("✅ Retrieved {Count} available questions", questions.Count);

                return Ok(new
                {
                    Success = true,
                    Data = questions,
                    Total = questions.Count,
                    Message = "Available questions retrieved successfully",
                    Note = "Minimal data for optimal performance"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving available questions. Error: {ErrorMessage}", ex.Message);
                
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
            Description = "Kiểm tra tính hợp lệ của request trước khi gọi AI. SubjectId/GradeId: Tùy chọn (auto-detect nếu không có). LessonId/DifficultyLevel: Tùy chọn."
        )]
        [SwaggerResponse(200, "Request is valid")]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Unauthorized")]
        public IActionResult ValidateRequest([FromBody] GenerateAIExamRequest request)
        {
            try
            {
                _logger.LogInformation("🔍 Validating AI exam request for UserId: {UserId}", request.UserId);

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
                    _logger.LogWarning("❌ Validation failed for UserId: {UserId}. Errors: {Errors}", 
                        request.UserId, string.Join(", ", validationErrors));

                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = validationErrors
                    });
                }

                _logger.LogInformation("✅ Request validation successful for UserId: {UserId}", request.UserId);

                return Ok(new
                {
                    Success = true,
                    Message = "Request is valid and ready for AI generation",
                    Data = new
                    {
                        UserId = request.UserId,
                        QuestionCount = request.QuestionCount,
                        Mode = request.Mode,
                        HistoryCount = request.HistoryCount,
                        Filters = new
                        {
                            SubjectIds = request.SubjectIds != null && request.SubjectIds.Any() 
                                ? $"[{string.Join(", ", request.SubjectIds)}]" 
                                : "Auto-detect from history",
                            GradeIds = request.GradeIds != null && request.GradeIds.Any() 
                                ? $"[{string.Join(", ", request.GradeIds)}]" 
                                : "Auto-detect from history",
                            ChapterIds = request.ChapterIds != null && request.ChapterIds.Any() 
                                ? $"[{string.Join(", ", request.ChapterIds)}]" 
                                : "All chapters in subject",
                            LessonIds = request.LessonIds != null && request.LessonIds.Any() 
                                ? $"[{string.Join(", ", request.LessonIds)}]" 
                                : "All lessons in chapter",
                            DifficultyLevelId = request.DifficultyLevelId.HasValue 
                                ? $"{request.DifficultyLevelId.Value} ({(request.DifficultyLevelId.Value == 1 ? "EASY" : request.DifficultyLevelId.Value == 2 ? "MEDIUM" : "HARD")})" 
                                : "All levels"
                        },
                        Notes = new[]
                        {
                            "✅ Hierarchy: Grade → Subject → Chapter → Lesson",
                            "✅ SubjectIds, GradeIds, ChapterIds, LessonIds: Có thể chọn NHIỀU (combobox)",
                            "✅ Nếu user chỉ định → AI BẮT BUỘC tuân theo",
                            "✅ Nếu user không chỉ định → Auto-detect từ lịch sử hoặc lấy tất cả",
                            "✅ Đề thi tự động lưu vào DB và trả về ExamId"
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error validating request. Error: {ErrorMessage}", ex.Message);
                
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
