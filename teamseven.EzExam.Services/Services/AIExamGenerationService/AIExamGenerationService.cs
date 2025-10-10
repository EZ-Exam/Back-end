using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Services.Interfaces;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.QuestionsService;

namespace teamseven.EzExam.Services.Services.AIExamGenerationService
{
    public class AIExamGenerationService : IAIExamGenerationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQuestionsService _questionsService;
        private readonly IExamHistoryService _examHistoryService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIExamGenerationService> _logger;
        private readonly HttpClient _httpClient;

        public AIExamGenerationService(
            IUnitOfWork unitOfWork,
            IQuestionsService questionsService,
            IExamHistoryService examHistoryService,
            IConfiguration configuration,
            ILogger<AIExamGenerationService> logger,
            HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _questionsService = questionsService;
            _examHistoryService = examHistoryService;
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
                _logger.LogInformation("Starting AI exam generation for UserId: {UserId}, Mode: {Mode}, QuestionCount: {QuestionCount}", 
                    request.UserId, request.Mode, request.QuestionCount);

                // 1. Lấy lịch sử làm bài của user
                var examHistory = await GetUserExamHistoryAsync(request.UserId, request.HistoryCount);
                _logger.LogInformation("Retrieved {Count} exam history records for user {UserId}", examHistory.Count, request.UserId);

                // 2. Lấy danh sách câu hỏi có sẵn
                var availableQuestions = await GetAvailableQuestionsAsync(request);
                _logger.LogInformation("Retrieved {Count} available questions", availableQuestions.Count);

                if (availableQuestions.Count == 0)
                {
                    return new GenerateAIExamResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy câu hỏi phù hợp với tiêu chí đã chọn.",
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

                // 3. Xây dựng prompt
                var prompt = await BuildPromptAsync(request, examHistory, availableQuestions);

                // 4. Gọi OpenAI API
                var aiResponse = await CallOpenAIAsync(prompt);

                // 5. Parse response từ AI
                var result = await ParseAIResponseAsync(aiResponse, request);

                stopwatch.Stop();

                // 6. Cập nhật metadata
                result.Metadata.ProcessingTimeSeconds = stopwatch.Elapsed.TotalSeconds;
                result.Metadata.GeneratedAt = DateTime.UtcNow;
                result.Metadata.TotalQuestions = result.Questions.Count;

                _logger.LogInformation("AI exam generation completed successfully for UserId: {UserId}. Generated {Count} questions in {Time}ms", 
                    request.UserId, result.Questions.Count, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error generating AI exam for UserId: {UserId}", request.UserId);
                
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

        public async Task<List<QuestionSimpleResponse>> GetAvailableQuestionsAsync(GenerateAIExamRequest request)
        {
            try
            {
                var searchRequest = new QuestionSearchRequest
                {
                    GradeId = request.GradeId,
                    LessonId = request.LessonId, // null means get questions from all lessons in the grade
                    DifficultyLevel = request.DifficultyLevel
                };

                var questions = await _questionsService.GetAllQuestionsSimpleAsync(searchRequest);
                
                // Filter by subject if specified
                if (!string.IsNullOrEmpty(request.Subject))
                {
                    // Note: Subject filtering would need to be implemented in QuestionService
                    // For now, we'll return all questions
                }

                _logger.LogInformation("Retrieved {Count} questions for GradeId: {GradeId}, LessonId: {LessonId} (null = all lessons)", 
                    questions.Count, request.GradeId, request.LessonId);

                return questions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available questions");
                return new List<QuestionSimpleResponse>();
            }
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
            if (request.LessonId == null)
            {
                promptBuilder.AppendLine("4. CHÚ Ý: LessonId không được chỉ định - hãy chọn câu hỏi từ TẤT CẢ các bài học trong khối lớp để tạo đề thi tổng hợp (phù hợp cho thi học kỳ)");
            }
            else
            {
                promptBuilder.AppendLine("4. CHÚ Ý: LessonId được chỉ định - hãy tập trung chọn câu hỏi từ bài học cụ thể này");
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
                var apiKey = _configuration["OpenAI:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new InvalidOperationException("OpenAI API key is not configured");
                }

                var requestBody = new
                {
                    model = "gpt-4",
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 4000,
                    temperature = 0.7
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var openAIResponse = JsonConvert.DeserializeObject<OpenAIResponse>(responseContent);

                return openAIResponse?.Choices?.FirstOrDefault()?.Message?.Content ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling OpenAI API");
                throw;
            }
        }

        public async Task<GenerateAIExamResponse> ParseAIResponseAsync(string aiResponse, GenerateAIExamRequest request)
        {
            try
            {
                // Extract JSON from AI response
                var jsonStart = aiResponse.IndexOf("{");
                var jsonEnd = aiResponse.LastIndexOf("}") + 1;
                
                if (jsonStart == -1 || jsonEnd == 0)
                {
                    throw new InvalidOperationException("Invalid AI response format");
                }

                var jsonContent = aiResponse.Substring(jsonStart, jsonEnd - jsonStart);
                var aiResult = JsonConvert.DeserializeObject<AIResponse>(jsonContent);

                if (aiResult?.SelectedQuestions == null || !aiResult.SelectedQuestions.Any())
                {
                    throw new InvalidOperationException("No questions selected by AI");
                }

                // Get full question details
                var selectedQuestionIds = aiResult.SelectedQuestions.Select(q => q.QuestionId).ToList();
                var questions = new List<AIQuestionResponse>();

                foreach (var selectedQuestion in aiResult.SelectedQuestions)
                {
                    try
                    {
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
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error retrieving question details for ID: {QuestionId}", selectedQuestion.QuestionId);
                    }
                }

                // Calculate metadata
                var difficultyDistribution = questions
                    .GroupBy(q => q.DifficultyLevel)
                    .ToDictionary(g => g.Key, g => g.Count());

                var subjectDistribution = questions
                    .GroupBy(q => q.LessonName)
                    .ToDictionary(g => g.Key, g => g.Count());

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
                        AIModel = "gpt-4",
                        TokensUsed = 0, // Would need to parse from OpenAI response
                        ProcessingTimeSeconds = 0,
                        DifficultyDistribution = difficultyDistribution,
                        SubjectDistribution = subjectDistribution
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing AI response");
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
    }
}
