using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Services.Interfaces;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.QuestionsService;
using teamseven.EzExam.Services.Services.ExamService;

namespace teamseven.EzExam.Services.Services.AIExamGenerationService
{
    public class AIExamGenerationService : IAIExamGenerationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQuestionsService _questionsService;
        private readonly IExamHistoryService _examHistoryService;
        private readonly IExamService _examService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIExamGenerationService> _logger;
        private readonly HttpClient _httpClient;

        public AIExamGenerationService(
            IUnitOfWork unitOfWork,
            IQuestionsService questionsService,
            IExamHistoryService examHistoryService,
            IExamService examService,
            IConfiguration configuration,
            ILogger<AIExamGenerationService> logger,
            HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _questionsService = questionsService;
            _examHistoryService = examHistoryService;
            _examService = examService;
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<GenerateAIExamResponse> GenerateExamAsync(GenerateAIExamRequest request)
        {
            var startTime = DateTime.UtcNow;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var examHistory = await GetUserExamHistoryMinimalAsync(request.UserId, request.HistoryCount);

                if (examHistory == null)
                {
                    examHistory = new List<ExamHistoryMinimalResponse>();
                }

                var availableQuestions = await GetAvailableQuestionsMinimalAsync(request);

                if (availableQuestions == null)
                {
                    availableQuestions = new List<QuestionMinimalResponse>();
                }

                if (availableQuestions.Count == 0)
                {
                    _logger.LogError("No available questions found after all retry attempts");
                    
                    return new GenerateAIExamResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy câu hỏi phù hợp với tiêu chí đã chọn. Vui lòng thử lại với bộ lọc khác hoặc liên hệ admin.",
                        Questions = new List<AIQuestionResponse>(),
                        Metadata = new AIExamMetadata
                        {
                            TotalQuestions = 0,
                            Mode = request.Mode,
                            UserId = request.UserId,
                            GeneratedAt = DateTime.UtcNow,
                            AIModel = "gpt-4",
                            TokensUsed = 0,
                            ProcessingTimeSeconds = stopwatch.Elapsed.TotalSeconds
                        }
                    };
                }

                // Log chi tiết câu hỏi có sẵn
                var difficultyGroups = availableQuestions.GroupBy(q => q.DifficultyLevel);
                foreach (var group in difficultyGroups)
                {
                    // summary suppressed in production
                }

                var gradeGroups = availableQuestions.GroupBy(q => q.GradeName);
                foreach (var group in gradeGroups)
                {
                    // summary suppressed in production
                }

                // Lọc lịch sử theo yêu cầu của user để AI hiểu đúng context
                var filteredHistory = FilterExamHistoryByRequest(examHistory, request);
                // Filtered history summary suppressed in production

                // 3. Xây dựng prompt với data tối thiểu
                // building prompt (log suppressed)
                var prompt = await BuildPromptMinimalAsync(request, filteredHistory, availableQuestions);
                // prompt built (length suppressed)

                // Log prompt preview (first 500 chars)
                var promptPreview = prompt.Length > 500 ? prompt.Substring(0, 500) + "..." : prompt;
                // prompt preview suppressed

                // 4. Gọi OpenAI API
                // calling OpenAI (log suppressed)
                var aiResponse = await CallOpenAIAsync(prompt);
                // OpenAI response received (size suppressed)

                // Log AI response preview
                var responsePreview = aiResponse.Length > 300 ? aiResponse.Substring(0, 300) + "..." : aiResponse;
                // AI response preview suppressed

                // 5. Parse response từ AI
                // parsing AI response (log suppressed)
                var result = await ParseAIResponseAsync(aiResponse, request);
                // AI parsed successfully (summary suppressed)

                stopwatch.Stop();

                // 6. Cập nhật metadata
                result.Metadata.ProcessingTimeSeconds = stopwatch.Elapsed.TotalSeconds;
                result.Metadata.GeneratedAt = DateTime.UtcNow;
                result.Metadata.TotalQuestions = result.Questions.Count;

                // 7. Lưu exam vào database
                // saving exam to database (log suppressed)
                var examId = await SaveAIExamToDatabaseAsync(request, result);
                if (examId > 0)
                {
                    result.Metadata.ExamId = examId;
                    result.Message = $"Tạo đề thi thành công với {result.Questions.Count} câu hỏi. Exam ID: {examId}";
                    // exam saved successfully (log suppressed)
                }
                else
                {
                    _logger.LogWarning("⚠️ Failed to save exam to database, but questions were generated");
                }

                // AI exam generation completed (summary suppressed)

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "❌ Error generating AI exam for UserId: {UserId}. Error: {ErrorMessage}", request.UserId, ex.Message);
                
                return new GenerateAIExamResponse
                {
                    Success = false,
                    Message = $"Lỗi khi tạo đề thi: {ex.Message}",
                    Questions = new List<AIQuestionResponse>(),
                    Metadata = new AIExamMetadata
                    {
                        TotalQuestions = 0,
                        Mode = request.Mode,
                        UserId = request.UserId,
                        GeneratedAt = DateTime.UtcNow,
                        AIModel = "gpt-4",
                        TokensUsed = 0,
                        ProcessingTimeSeconds = stopwatch.Elapsed.TotalSeconds
                    }
                };
            }
        }

        public async Task<List<ExamHistoryMinimalResponse>> GetUserExamHistoryMinimalAsync(int userId, int count = 5)
        {
            try
            {
                var histories = await _examHistoryService.GetExamHistoriesMinimalByUserIdAsync(userId);
                
                        // Convert to minimal response - only necessary fields
                var minimalHistories = histories
                    .OrderByDescending(h => h.SubmittedAt)
                    .Take(count)
                    .ToList();

                        _logger.LogInformation("Retrieved {Count} minimal exam history records for user {UserId}", minimalHistories.Count, userId);
                return minimalHistories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam history for UserId: {UserId}", userId);
                return new List<ExamHistoryMinimalResponse>();
            }
        }

        public async Task<List<ExamHistoryResponse>> GetUserExamHistoryAsync(int userId, int count = 5)
        {
            try
            {
                var histories = await _examHistoryService.GetExamHistoriesByUserIdAsync(userId);
                return histories
                    .OrderByDescending(h => h.SubmittedAt)
                    .Take(count)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam history for UserId: {UserId}", userId);
                return new List<ExamHistoryResponse>();
            }
        }

        public async Task<List<QuestionMinimalResponse>> GetAvailableQuestionsMinimalAsync(GenerateAIExamRequest request)
        {
            try
            {
                        _logger.LogInformation("🔍 Starting question retrieval for UserId: {UserId}", request.UserId);
                        _logger.LogInformation("📝 Initial request - SubjectIds: [{SubjectIds}], GradeIds: [{GradeIds}], ChapterIds: [{ChapterIds}], LessonIds: [{LessonIds}]", 
                            request.SubjectIds != null && request.SubjectIds.Any() ? string.Join(", ", request.SubjectIds) : "null",
                            request.GradeIds != null && request.GradeIds.Any() ? string.Join(", ", request.GradeIds) : "null",
                            request.ChapterIds != null && request.ChapterIds.Any() ? string.Join(", ", request.ChapterIds) : "null",
                            request.LessonIds != null && request.LessonIds.Any() ? string.Join(", ", request.LessonIds) : "null");
                
                // Lấy lịch sử để phân tích (luôn lấy để có context cho AI)
                var examHistory = await GetUserExamHistoryMinimalAsync(request.UserId, request.HistoryCount);
                
                // Xác định PHẠM VI gen câu hỏi (SubjectIds, GradeIds, LessonIds)
                // 2 nguồn BỔ TRỢ nhau:
                // 1. Request của user → Xác định phạm vi gen đề
                // 2. Lịch sử làm bài → AI biết học sinh làm sai chỗ nào, cần cải thiện gì
                List<int>? finalSubjectIds = request.SubjectIds;
                List<int>? finalGradeIds = request.GradeIds;
                List<int>? finalLessonIds = request.LessonIds;
                
                if (examHistory.Any())
                {
                    _logger.LogInformation("📈 Found {Count} exam history records for analysis", examHistory.Count);
                    
                    // Phân tích lịch sử để hiểu năng lực học sinh
                    var mostCommonSubject = examHistory.GroupBy(h => h.SubjectId)
                        .OrderByDescending(g => g.Count())
                        .First().Key;
                    var mostCommonGrade = examHistory.Where(h => h.GradeId.HasValue)
                        .GroupBy(h => h.GradeId)
                        .OrderByDescending(g => g.Count())
                        .FirstOrDefault()?.Key;

                    var mostRecentHistory = examHistory.First();
                            _logger.LogInformation("🎯 History shows student mainly studies: Subject {SubjectId} ({SubjectName}), Grade {GradeId} ({GradeName})", 
                        mostCommonSubject, mostRecentHistory.SubjectName, mostCommonGrade, mostRecentHistory.GradeName);

                    // Xác định PHẠM VI gen câu hỏi
                    // Nếu user chỉ định → Dùng phạm vi của user
                    // Nếu không → Auto-detect từ lịch sử
                    if (finalSubjectIds == null || !finalSubjectIds.Any())
                    {
                        finalSubjectIds = new List<int> { mostCommonSubject };
                                _logger.LogInformation("✅ Scope: Auto-detected SubjectIds [{SubjectIds}] from history", 
                            string.Join(", ", finalSubjectIds));
                    }
                    else
                    {
                                _logger.LogInformation("👤 Scope: User selected SubjectIds [{SubjectIds}]", 
                            string.Join(", ", finalSubjectIds));
                    }
                    
                    if ((finalGradeIds == null || !finalGradeIds.Any()) && mostCommonGrade.HasValue)
                    {
                        finalGradeIds = new List<int> { mostCommonGrade.Value };
                                _logger.LogInformation("✅ Scope: Auto-detected GradeIds [{GradeIds}] from history", 
                            string.Join(", ", finalGradeIds));
                    }
                    else if (finalGradeIds != null && finalGradeIds.Any())
                    {
                                _logger.LogInformation("👤 Scope: User selected GradeIds [{GradeIds}]", 
                            string.Join(", ", finalGradeIds));
                    }

                    if (finalLessonIds != null && finalLessonIds.Any())
                    {
                                _logger.LogInformation("👤 Scope: User selected LessonIds [{LessonIds}]", 
                            string.Join(", ", finalLessonIds));
                    }
                }
                else
                {
                    _logger.LogWarning("⚠️ No exam history found for user {UserId}", request.UserId);
                    // Nếu không có lịch sử nhưng user có yêu cầu → Sử dụng yêu cầu của user
                    if ((finalSubjectIds == null || !finalSubjectIds.Any()) || (finalGradeIds == null || !finalGradeIds.Any()))
                    {
                        _logger.LogWarning("⚠️ No history and no user-provided SubjectIds/GradeIds. Cannot proceed.");
                    }
                    else
                    {
                                _logger.LogInformation("✅ No history, but using user-provided filters: SubjectIds [{SubjectIds}], GradeIds [{GradeIds}]",
                            string.Join(", ", finalSubjectIds ?? new List<int>()),
                            string.Join(", ", finalGradeIds ?? new List<int>()));
                    }
                }

                        _logger.LogInformation("🔍 [ATTEMPT 1] Final search criteria - SubjectIds: [{SubjectIds}], GradeIds: [{GradeIds}], ChapterIds: [{ChapterIds}], LessonIds: [{LessonIds}], DifficultyLevelId: {DifficultyLevelId}", 
                    finalSubjectIds != null ? string.Join(", ", finalSubjectIds) : "null",
                    finalGradeIds != null ? string.Join(", ", finalGradeIds) : "null",
                    request.ChapterIds != null && request.ChapterIds.Any() ? string.Join(", ", request.ChapterIds) : "null",
                    finalLessonIds != null ? string.Join(", ", finalLessonIds) : "null",
                    request.DifficultyLevelId?.ToString() ?? "null");

                var searchRequest = new QuestionSearchRequest
                {
                    SubjectIds = finalSubjectIds,
                    GradeIds = finalGradeIds,
                    ChapterIds = request.ChapterIds,
                    LessonIds = finalLessonIds,
                    DifficultyLevelId = request.DifficultyLevelId
                };

                        _logger.LogInformation("📡 [ATTEMPT 1] Calling QuestionService with auto-detect from history...");
                var questions = await _questionsService.GetAllQuestionsSimpleAsync(searchRequest);
                        _logger.LogInformation("📥 [ATTEMPT 1] Received {Count} questions from QuestionService", questions.Count);
                
                // ⚠️ RETRY LOGIC: Nếu không tìm thấy câu hỏi nào → Thử lại CHỈ dựa vào request của user
                if (questions.Count == 0)
                {
                    _logger.LogWarning("⚠️ [ATTEMPT 1] No questions found with auto-detect. Retrying with ONLY user request (ignore history)...");
                    
                    // Kiểm tra xem user có cung cấp filter không
                    bool hasUserFilters = (request.SubjectIds != null && request.SubjectIds.Any()) ||
                                         (request.GradeIds != null && request.GradeIds.Any()) ||
                                         (request.ChapterIds != null && request.ChapterIds.Any()) ||
                                         (request.LessonIds != null && request.LessonIds.Any()) ||
                                         request.DifficultyLevelId.HasValue;

                    QuestionSearchRequest? retrySearchRequest = null;

                    if (hasUserFilters)
                    {
                        // Tạo search request CHỈ từ yêu cầu của user, BỎ QUA auto-detect
                        retrySearchRequest = new QuestionSearchRequest
                        {
                            SubjectIds = request.SubjectIds,
                            GradeIds = request.GradeIds,
                            ChapterIds = request.ChapterIds,
                            LessonIds = request.LessonIds,
                            DifficultyLevelId = request.DifficultyLevelId
                        };

                                _logger.LogInformation("🔍 [ATTEMPT 2] Retry with ONLY user filters - SubjectIds: [{SubjectIds}], GradeIds: [{GradeIds}], ChapterIds: [{ChapterIds}], LessonIds: [{LessonIds}], DifficultyLevelId: {DifficultyLevelId}",
                            request.SubjectIds != null && request.SubjectIds.Any() ? string.Join(", ", request.SubjectIds) : "null",
                            request.GradeIds != null && request.GradeIds.Any() ? string.Join(", ", request.GradeIds) : "null",
                            request.ChapterIds != null && request.ChapterIds.Any() ? string.Join(", ", request.ChapterIds) : "null",
                            request.LessonIds != null && request.LessonIds.Any() ? string.Join(", ", request.LessonIds) : "null",
                            request.DifficultyLevelId?.ToString() ?? "null");
                    }
                    else
                    {
                        // User không có filter gì → Lấy TẤT CẢ câu hỏi (null = no filter)
                        _logger.LogWarning("⚠️ [ATTEMPT 2] User has NO filters. Fetching ALL questions from database...");
                        retrySearchRequest = null; // null = lấy tất cả
                    }

                            _logger.LogInformation("📡 [ATTEMPT 2] Calling QuestionService...");
                    questions = await _questionsService.GetAllQuestionsSimpleAsync(retrySearchRequest);
                            _logger.LogInformation("📥 [ATTEMPT 2] Received {Count} questions from QuestionService", questions.Count);

                    if (questions.Count == 0)
                    {
                        _logger.LogError("❌ [ATTEMPT 2] Still no questions found. Both attempts failed.");
                    }
                    else
                    {
                        _logger.LogInformation("✅ [ATTEMPT 2] Successfully found {Count} questions without history auto-detect!", questions.Count);
                    }
                }
                
                // Convert to minimal response - chỉ lấy các trường cần thiết
                var minimalQuestions = questions.Select(q => new QuestionMinimalResponse
                {
                    Id = q.Id,
                    Content = q.Content,
                    DifficultyLevel = q.DifficultyLevel ?? "MEDIUM",
                    GradeName = q.GradeName ?? "Unknown",
                    LessonName = q.LessonName ?? "Unknown"
                }).ToList();

                        _logger.LogInformation("✅ Converted to {Count} minimal questions", minimalQuestions.Count);
                _logger.LogInformation("📊 Question Distribution:");
                
                var difficultyGroups = minimalQuestions.GroupBy(q => q.DifficultyLevel);
                foreach (var group in difficultyGroups)
                {
                    _logger.LogInformation("  🎯 Difficulty {Difficulty}: {Count} questions", group.Key, group.Count());
                }

                var gradeGroups = minimalQuestions.GroupBy(q => q.GradeName);
                foreach (var group in gradeGroups)
                {
                    _logger.LogInformation("  🏫 Grade {Grade}: {Count} questions", group.Key, group.Count());
                }

                var lessonGroups = minimalQuestions.GroupBy(q => q.LessonName);
                foreach (var group in lessonGroups.Take(5)) // Limit to first 5 lessons
                {
                    _logger.LogInformation("  📖 Lesson {Lesson}: {Count} questions", group.Key, group.Count());
                }

                _logger.LogInformation("✅ Successfully retrieved {Count} minimal questions", minimalQuestions.Count);

                return minimalQuestions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving available questions for UserId: {UserId}. Error: {ErrorMessage}", request.UserId, ex.Message);
                return new List<QuestionMinimalResponse>();
            }
        }

        public async Task<List<QuestionSimpleResponse>> GetAvailableQuestionsAsync(GenerateAIExamRequest request)
        {
            try
            {
                var searchRequest = new QuestionSearchRequest
                {
                    SubjectIds = request.SubjectIds,
                    GradeIds = request.GradeIds,
                    ChapterIds = request.ChapterIds,
                    LessonIds = request.LessonIds,
                    DifficultyLevelId = request.DifficultyLevelId
                };

                var questions = await _questionsService.GetAllQuestionsSimpleAsync(searchRequest);

                _logger.LogInformation("Retrieved {Count} questions for SubjectIds: [{SubjectIds}], GradeIds: [{GradeIds}], ChapterIds: [{ChapterIds}], LessonIds: [{LessonIds}]", 
                    questions.Count, 
                    request.SubjectIds != null && request.SubjectIds.Any() ? string.Join(", ", request.SubjectIds) : "null",
                    request.GradeIds != null && request.GradeIds.Any() ? string.Join(", ", request.GradeIds) : "null",
                    request.ChapterIds != null && request.ChapterIds.Any() ? string.Join(", ", request.ChapterIds) : "null",
                    request.LessonIds != null && request.LessonIds.Any() ? string.Join(", ", request.LessonIds) : "null");

                return questions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available questions");
                return new List<QuestionSimpleResponse>();
            }
        }

        public async Task<string> BuildPromptMinimalAsync(GenerateAIExamRequest request, List<ExamHistoryMinimalResponse> history, List<QuestionMinimalResponse> questions)
        {
            var promptBuilder = new StringBuilder();

            // System prompt - simplified and direct
            promptBuilder.AppendLine($"🎯 MỤC TIÊU: Chọn ĐÚNG {request.QuestionCount} câu hỏi để gen đề thi");
            promptBuilder.AppendLine($"📚 MODE: {(request.Mode == "review" ? "Ôn tập (ưu tiên dễ-trung bình)" : "Nâng cao (ưu tiên trung bình-khó)")}");
            promptBuilder.AppendLine();

            // User's exam history (minimal) - Để AI hiểu học sinh làm sai chỗ nào
            promptBuilder.AppendLine("LỊCH SỬ LÀM BÀI (trong phạm vi đã chọn) - Phân tích để biết học sinh làm SAI CHỖ NÀO:");
            if (history.Any())
            {
                foreach (var h in history)
                {
                    promptBuilder.AppendLine($"- Điểm: {h.Score}/100, Đúng: {h.CorrectCount}/{h.TotalQuestions}, Thời gian: {h.TimeTaken}s, Ngày: {h.SubmittedAt:dd/MM/yyyy}");
                    promptBuilder.AppendLine($"  Môn: {h.SubjectName}, Lớp: {h.GradeName}, Chương: {h.ChapterName}, Bài: {h.LessonName}");
                }
            }
            else
            {
                promptBuilder.AppendLine("- Chưa có lịch sử làm bài");
            }
            promptBuilder.AppendLine();

            // Available questions (minimal) - đã được lọc theo phạm vi user chọn
            promptBuilder.AppendLine("DANH SÁCH CÂU HỎI (đã lọc theo phạm vi user chọn):");
            
            // Gửi ít nhất 2x số câu cần chọn, tối đa 100 câu để tiết kiệm token
            // Ví dụ: Cần 10 câu → Gửi 20-30 câu; Cần 50 câu → Gửi 100 câu
            var minQuestions = Math.Min(request.QuestionCount * 2, 100);
            var maxQuestions = Math.Min(request.QuestionCount * 3, 150);
            var questionLimit = Math.Min(Math.Max(minQuestions, questions.Count), maxQuestions);
            var limitedQuestions = questions.Take(questionLimit).ToList();
            
            // Rút ngắn content dựa vào số câu hỏi
            // Nếu nhiều câu (>30) → Rút ngắn 60 ký tự để tiết kiệm token
            // Nếu ít câu (<=30) → Hiển thị đầy đủ hơn (100 ký tự)
            var maxContentLength = limitedQuestions.Count > 30 ? 60 : 100;
            
            foreach (var q in limitedQuestions)
            {
                var shortContent = q.Content.Length > maxContentLength 
                    ? q.Content.Substring(0, maxContentLength) + "..." 
                    : q.Content;
                promptBuilder.AppendLine($"ID: {q.Id} | {shortContent} | Độ khó: {q.DifficultyLevel}");
            }
            promptBuilder.AppendLine($"(Tổng {questions.Count} câu, hiển thị {limitedQuestions.Count} câu)");
            promptBuilder.AppendLine();

            // Instructions - Kết hợp lịch sử và yêu cầu
            promptBuilder.AppendLine("YÊU CẦU:");
            promptBuilder.AppendLine($"1. Phân tích LỊCH SỬ → Hiểu điểm yếu của học sinh");
            promptBuilder.AppendLine($"2. Chọn từ DANH SÁCH → {request.QuestionCount} câu giúp cải thiện năng lực");
            promptBuilder.AppendLine($"3. Tuân thủ MODE → {(request.Mode == "review" ? "Câu dễ-trung bình" : "Câu trung bình-khó")}");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("OUTPUT (JSON):");
            promptBuilder.AppendLine("{");
            promptBuilder.AppendLine("  \"selectedQuestions\": [");
            promptBuilder.AppendLine("    {\"questionId\": 123, \"reasoning\": \"Lý do chọn\"},");
            promptBuilder.AppendLine($"    ... ({request.QuestionCount} câu)");
            promptBuilder.AppendLine("  ],");
            promptBuilder.AppendLine("  \"analysis\": \"Phân tích ngắn gọn về năng lực học sinh\"");
            promptBuilder.AppendLine("}");

            return promptBuilder.ToString();
        }

        public async Task<string> BuildPromptAsync(GenerateAIExamRequest request, List<ExamHistoryResponse> history, List<QuestionSimpleResponse> questions)
        {
            var promptBuilder = new StringBuilder();

            // System prompt
            promptBuilder.AppendLine("Bạn là một AI chuyên gia giáo dục, có nhiệm vụ tạo đề thi tự động dựa trên lịch sử học tập và năng lực của học sinh.");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("NHIỆM VỤ:");
            promptBuilder.AppendLine($"- Tạo đề thi gồm {request.QuestionCount} câu hỏi");
            promptBuilder.AppendLine($"- Mode: {(request.Mode == "review" ? "Ôn lại kiến thức" : "Nâng cao kỹ năng")}");
            promptBuilder.AppendLine("- Phân tích lịch sử làm bài để hiểu điểm mạnh/yếu của học sinh");
            promptBuilder.AppendLine("- Chọn câu hỏi phù hợp để cải thiện năng lực học tập");
            promptBuilder.AppendLine();

            // User's exam history
            promptBuilder.AppendLine("LỊCH SỬ LÀM BÀI GẦN NHẤT:");
            if (history.Any())
            {
                foreach (var h in history)
                {
                    promptBuilder.AppendLine($"- Điểm: {h.Score}/100, Đúng: {h.CorrectCount}/{h.TotalQuestions}, Thời gian: {h.TimeTaken}s, Ngày: {h.SubmittedAt:dd/MM/yyyy}");
                }
            }
            else
            {
                promptBuilder.AppendLine("- Chưa có lịch sử làm bài");
            }
            promptBuilder.AppendLine();

            // Available questions
            promptBuilder.AppendLine("DANH SÁCH CÂU HỎI CÓ SẴN:");
            foreach (var q in questions.Take(100)) // Limit to first 100 questions for prompt size
            {
                promptBuilder.AppendLine($"ID: {q.Id} | Nội dung: {q.Content} | Độ khó: {q.DifficultyLevel} | Lớp: {q.GradeName} | Bài: {q.LessonName}");
            }
            promptBuilder.AppendLine();

            // Instructions
            promptBuilder.AppendLine("YÊU CẦU:");
            promptBuilder.AppendLine("1. Phân tích lịch sử làm bài để xác định điểm yếu của học sinh");
            promptBuilder.AppendLine("2. Chọn câu hỏi phù hợp với mode đã chọn:");
            if (request.Mode == "review")
            {
                promptBuilder.AppendLine("   - Ưu tiên câu hỏi ở mức độ dễ-trung bình");
                promptBuilder.AppendLine("   - Tập trung vào kiến thức cơ bản");
                promptBuilder.AppendLine("   - Giúp học sinh củng cố kiến thức đã học");
            }
            else
            {
                promptBuilder.AppendLine("   - Ưu tiên câu hỏi ở mức độ trung bình-khó");
                promptBuilder.AppendLine("   - Thách thức học sinh với kiến thức nâng cao");
                promptBuilder.AppendLine("   - Phát triển tư duy phân tích và giải quyết vấn đề");
            }
            promptBuilder.AppendLine("3. Đảm bảo đa dạng về độ khó và chủ đề");
            if (request.LessonIds == null || !request.LessonIds.Any())
            {
                promptBuilder.AppendLine("4. CHÚ Ý: LessonIds không được chỉ định - hãy chọn câu hỏi từ TẤT CẢ các bài học trong khối lớp để tạo đề thi tổng hợp (phù hợp cho thi học kỳ)");
            }
            else
            {
                promptBuilder.AppendLine($"4. CHÚ Ý: LessonIds được chỉ định [{string.Join(", ", request.LessonIds)}] - hãy tập trung chọn câu hỏi từ các bài học cụ thể này");
            }
            promptBuilder.AppendLine("5. Trả về KẾT QUẢ DƯỚI DẠNG JSON với format sau:");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("```json");
            promptBuilder.AppendLine("{");
            promptBuilder.AppendLine("  \"selectedQuestions\": [");
            promptBuilder.AppendLine("    {");
            promptBuilder.AppendLine("      \"questionId\": 123,");
            promptBuilder.AppendLine("      \"reasoning\": \"Lý do chọn câu hỏi này dựa trên phân tích lịch sử học tập\"");
            promptBuilder.AppendLine("    }");
            promptBuilder.AppendLine("  ],");
            promptBuilder.AppendLine("  \"analysis\": \"Phân tích tổng quan về năng lực học sinh và lý do chọn đề thi này\"");
            promptBuilder.AppendLine("}");
            promptBuilder.AppendLine("```");

            return promptBuilder.ToString();
        }

        public async Task<string> CallOpenAIAsync(string prompt)
        {
            try
            {
                _logger.LogInformation("🤖 Starting OpenAI API call...");
                
                var apiKey = _configuration["OpenAI:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogError("❌ OpenAI API key is not configured");
                    throw new InvalidOperationException("OpenAI API key is not configured");
                }

                _logger.LogInformation("🔑 OpenAI API key found, length: {Length}", apiKey.Length);

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 1500,
                    temperature = 0.3
                };

                var json = JsonConvert.SerializeObject(requestBody);
                _logger.LogInformation("📤 Request body size: {Size} characters", json.Length);
                _logger.LogInformation("🎯 Model: gpt-3.5-turbo, Max tokens: 1500, Temperature: 0.3");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                _logger.LogInformation("📡 Sending request to OpenAI API...");
                var startTime = DateTime.UtcNow;
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                var endTime = DateTime.UtcNow;
                var duration = (endTime - startTime).TotalSeconds;

                _logger.LogInformation("📥 Received response from OpenAI API. Status: {StatusCode}, Duration: {Duration}s", 
                    response.StatusCode, duration);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("❌ OpenAI API error response: {ErrorContent}", errorContent);
                    throw new HttpRequestException($"OpenAI API returned {response.StatusCode}. Error: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("✅ OpenAI API call successful. Response size: {Size} characters", responseContent.Length);

                var openAIResponse = JsonConvert.DeserializeObject<OpenAIResponse>(responseContent);
                var aiResponse = openAIResponse?.Choices?.FirstOrDefault()?.Message?.Content ?? string.Empty;

                if (string.IsNullOrEmpty(aiResponse))
                {
                    _logger.LogError("❌ Empty response from OpenAI API");
                    throw new InvalidOperationException("Empty response from OpenAI API");
                }

                _logger.LogInformation("🎉 Successfully extracted AI response. Length: {Length} characters", aiResponse.Length);
                return aiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error calling OpenAI API. Error: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<GenerateAIExamResponse> ParseAIResponseAsync(string aiResponse, GenerateAIExamRequest request)
        {
            try
            {
                _logger.LogInformation("🔍 Starting AI response parsing...");
                _logger.LogInformation("📄 Input response length: {Length} characters", aiResponse.Length);

                // Extract JSON from AI response
                var jsonStart = aiResponse.IndexOf("{");
                var jsonEnd = aiResponse.LastIndexOf("}") + 1;
                
                _logger.LogInformation("🔍 JSON extraction - Start: {Start}, End: {End}", jsonStart, jsonEnd);
                
                if (jsonStart == -1 || jsonEnd == 0)
                {
                    _logger.LogError("❌ Invalid AI response format - no JSON found");
                    throw new InvalidOperationException("Invalid AI response format");
                }

                var jsonContent = aiResponse.Substring(jsonStart, jsonEnd - jsonStart);
                _logger.LogInformation("📝 Extracted JSON content length: {Length} characters", jsonContent.Length);
                
                var aiResult = JsonConvert.DeserializeObject<AIResponse>(jsonContent);
                _logger.LogInformation("✅ Successfully deserialized AI response");

                if (aiResult?.SelectedQuestions == null || !aiResult.SelectedQuestions.Any())
                {
                    _logger.LogError("❌ No questions selected by AI");
                    throw new InvalidOperationException("No questions selected by AI");
                }

                _logger.LogInformation("🎯 AI selected {Count} questions", aiResult.SelectedQuestions.Count);
                
                // Log AI analysis
                if (!string.IsNullOrEmpty(aiResult.Analysis))
                {
                    _logger.LogInformation("🧠 AI Analysis: {Analysis}", aiResult.Analysis);
                }

                // Get full question details
                var selectedQuestionIds = aiResult.SelectedQuestions.Select(q => q.QuestionId).ToList();
                _logger.LogInformation("📋 Selected Question IDs: {QuestionIds}", string.Join(", ", selectedQuestionIds));
                
                var questions = new List<AIQuestionResponse>();
                var successCount = 0;
                var errorCount = 0;

                foreach (var selectedQuestion in aiResult.SelectedQuestions)
                {
                    try
                    {
                        _logger.LogInformation("🔍 Processing question ID {QuestionId}...", selectedQuestion.QuestionId);
                        var questionDetail = await _questionsService.GetQuestionById(selectedQuestion.QuestionId);
                        
                        if (questionDetail != null)
                        {
                            var aiQuestion = new AIQuestionResponse
                            {
                                QuestionId = questionDetail.Id,
                                Content = questionDetail.Content,
                                DifficultyLevel = questionDetail.DifficultyLevelId.ToString(),
                                QuestionType = questionDetail.Type ?? "MULTIPLE_CHOICE",
                                Options = questionDetail.Options ?? new List<string>(),
                                CorrectAnswer = questionDetail.CorrectAnswer ?? "",
                                Explanation = questionDetail.Explanation ?? "",
                                Formula = questionDetail.Formula,
                                QuestionSource = questionDetail.QuestionSource,
                                GradeId = questionDetail.GradeId ?? 0,
                                GradeName = "", // Will be populated from Grade service if needed
                                LessonId = questionDetail.LessonId ?? 0,
                                LessonName = questionDetail.LessonName ?? "",
                                AIReasoning = selectedQuestion.Reasoning
                            };
                            questions.Add(aiQuestion);
                            successCount++;
                            _logger.LogInformation("✅ Successfully processed question ID {QuestionId}", selectedQuestion.QuestionId);
                        }
                        else
                        {
                            errorCount++;
                            _logger.LogWarning("⚠️ Question ID {QuestionId} not found in database", selectedQuestion.QuestionId);
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        _logger.LogError(ex, "❌ Error processing question ID {QuestionId}. Error: {ErrorMessage}", 
                            selectedQuestion.QuestionId, ex.Message);
                    }
                }

                _logger.LogInformation("📊 Question processing summary - Success: {SuccessCount}, Errors: {ErrorCount}", 
                    successCount, errorCount);

                // Calculate metadata
                var difficultyDistribution = questions
                    .GroupBy(q => q.DifficultyLevel)
                    .ToDictionary(g => g.Key, g => g.Count());

                var subjectDistribution = questions
                    .GroupBy(q => q.LessonName)
                    .ToDictionary(g => g.Key, g => g.Count());

                _logger.LogInformation("🎉 AI response parsing completed successfully. Generated {Count} questions", questions.Count);
                
                return new GenerateAIExamResponse
                {
                    Success = true,
                    Message = "Đề thi được tạo thành công bởi AI",
                    Questions = questions,
                    Metadata = new AIExamMetadata
                    {
                        TotalQuestions = questions.Count,
                        Mode = request.Mode,
                        UserId = request.UserId,
                        GeneratedAt = DateTime.UtcNow,
                        AIModel = "gpt-3.5-turbo",
                        TokensUsed = 0, // Would need to parse from OpenAI response
                        ProcessingTimeSeconds = 0,
                        Analysis = aiResult?.Analysis ?? string.Empty,
                        DifficultyDistribution = difficultyDistribution,
                        SubjectDistribution = subjectDistribution
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error parsing AI response. Error: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        // Helper classes for OpenAI API
        private class OpenAIResponse
        {
            public List<Choice> Choices { get; set; } = new List<Choice>();
        }

        private class Choice
        {
            public Message Message { get; set; } = new Message();
        }

        private class Message
        {
            public string Content { get; set; } = string.Empty;
        }

        private class AIResponse
        {
            public List<SelectedQuestion> SelectedQuestions { get; set; } = new List<SelectedQuestion>();
            public string Analysis { get; set; } = string.Empty;
        }

        private class SelectedQuestion
        {
            public int QuestionId { get; set; }
            public string Reasoning { get; set; } = string.Empty;
        }

        // Save AI generated exam to database
        private async Task<int> SaveAIExamToDatabaseAsync(GenerateAIExamRequest request, GenerateAIExamResponse aiResult)
        {
            try
            {
                _logger.LogInformation("💾 Saving AI generated exam to database for UserId: {UserId}", request.UserId);

                // Create exam
                var examRequest = new ExamRequest
                {
                    Name = $"AI Exam - {request.Mode} - {DateTime.UtcNow:dd/MM/yyyy HH:mm}",
                    Description = $"AI generated exam. Mode: {request.Mode}",
                    SubjectId = request.SubjectIds?.FirstOrDefault() ?? 1, // Use first subject or default
                    GradeId = request.GradeIds?.FirstOrDefault(),
                    LessonId = request.LessonIds?.FirstOrDefault(),
                    ExamTypeId = 1, // Default exam type
                    CreatedByUserId = request.UserId,
                    Duration = request.QuestionCount * 3 // 3 minutes per question
                };

                var examId = await _examService.CreateExamAsync(examRequest);
                _logger.LogInformation("✅ Created exam with ID: {ExamId}", examId);

                // Add questions to exam
                foreach (var question in aiResult.Questions)
                {
                    await _unitOfWork.ExamQuestionRepository.AddAsync(new Repository.Models.ExamQuestion
                    {
                        ExamId = examId,
                        QuestionId = question.QuestionId,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _unitOfWork.SaveChangesWithTransactionAsync();
                _logger.LogInformation("✅ Added {Count} questions to exam {ExamId}", aiResult.Questions.Count, examId);

                return examId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error saving AI exam to database. Error: {ErrorMessage}", ex.Message);
                return 0; // Return 0 if failed
            }
        }

        /// <summary>
        /// Lọc lịch sử thi theo yêu cầu của user để AI chỉ nhận context liên quan
        /// </summary>
        private List<ExamHistoryMinimalResponse> FilterExamHistoryByRequest(
            List<ExamHistoryMinimalResponse> history, 
            GenerateAIExamRequest request)
        {
            if (history == null || !history.Any())
                return new List<ExamHistoryMinimalResponse>();

            var filtered = history.AsEnumerable();

            // Lọc theo SubjectIds (nếu user chỉ định)
            if (request.SubjectIds != null && request.SubjectIds.Any())
            {
                filtered = filtered.Where(h => request.SubjectIds.Contains(h.SubjectId));
                _logger.LogInformation("🔍 Filtered history by SubjectIds [{SubjectIds}]: {Count} records remain", 
                    string.Join(", ", request.SubjectIds), filtered.Count());
            }

            // Lọc theo GradeIds (nếu user chỉ định)
            if (request.GradeIds != null && request.GradeIds.Any())
            {
                filtered = filtered.Where(h => h.GradeId.HasValue && request.GradeIds.Contains(h.GradeId.Value));
                _logger.LogInformation("🔍 Filtered history by GradeIds [{GradeIds}]: {Count} records remain", 
                    string.Join(", ", request.GradeIds), filtered.Count());
            }

            // Lọc theo ChapterIds (nếu user chỉ định)
            if (request.ChapterIds != null && request.ChapterIds.Any())
            {
                filtered = filtered.Where(h => h.ChapterId.HasValue && request.ChapterIds.Contains(h.ChapterId.Value));
                _logger.LogInformation("🔍 Filtered history by ChapterIds [{ChapterIds}]: {Count} records remain", 
                    string.Join(", ", request.ChapterIds), filtered.Count());
            }

            // Lọc theo LessonIds (nếu user chỉ định)
            if (request.LessonIds != null && request.LessonIds.Any())
            {
                filtered = filtered.Where(h => h.LessonId.HasValue && request.LessonIds.Contains(h.LessonId.Value));
                _logger.LogInformation("🔍 Filtered history by LessonIds [{LessonIds}]: {Count} records remain", 
                    string.Join(", ", request.LessonIds), filtered.Count());
            }

            return filtered.ToList();
        }
    }
}
