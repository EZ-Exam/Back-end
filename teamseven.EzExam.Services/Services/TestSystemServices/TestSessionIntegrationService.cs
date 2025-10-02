using Microsoft.Extensions.Logging;
using System.Linq;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Services.StudentHistoryService;

namespace teamseven.EzExam.Services.Services.TestSystemServices
{
    public interface ITestSessionIntegrationService
    {
        Task HandleTestSessionCompletedAsync(int testSessionId);
        Task CreateQuizHistoryFromCompletedSessionAsync(int testSessionId);
    }

    public class TestSessionIntegrationService : ITestSessionIntegrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStudentHistoryService _studentHistoryService;
        private readonly ILogger<TestSessionIntegrationService> _logger;

        public TestSessionIntegrationService(
            IUnitOfWork unitOfWork,
            IStudentHistoryService studentHistoryService,
            ILogger<TestSessionIntegrationService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _studentHistoryService = studentHistoryService ?? throw new ArgumentNullException(nameof(studentHistoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleTestSessionCompletedAsync(int testSessionId)
        {
            try
            {
                _logger.LogInformation($"Handling completed test session {testSessionId}");
                
                // Create quiz history from test session
                await CreateQuizHistoryFromCompletedSessionAsync(testSessionId);
                
                _logger.LogInformation($"Successfully handled completed test session {testSessionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling completed test session {testSessionId}");
                throw;
            }
        }

        public async Task CreateQuizHistoryFromCompletedSessionAsync(int testSessionId)
        {
            try
            {
                var testSession = await _unitOfWork.TestSessionRepository.GetByIdAsync(testSessionId);
                if (testSession == null)
                {
                    _logger.LogWarning($"Test session {testSessionId} not found");
                    return;
                }

                if (testSession.SessionStatus != "COMPLETED")
                {
                    _logger.LogWarning($"Test session {testSessionId} is not completed (status: {testSession.SessionStatus})");
                    return;
                }

                // Check if quiz history already exists for this session
                var allHistories = await _unitOfWork.StudentQuizHistoryRepository.GetAllAsync();
                var existingHistory = allHistories.FirstOrDefault(h => h.TestSessionId == testSessionId);
                if (existingHistory != null)
                {
                    _logger.LogInformation($"Quiz history already exists for test session {testSessionId}");
                    return;
                }

                // Get session answers for detailed analysis
                var sessionAnswers = await _unitOfWork.TestSessionAnswerRepository.GetBySessionIdAsync(testSessionId);
                
                // Calculate performance metrics
                var totalQuestions = testSession.TotalQuestions;
                var correctAnswers = testSession.CorrectAnswers;
                var incorrectAnswers = totalQuestions - correctAnswers;
                var skippedQuestions = totalQuestions - sessionAnswers.Count();
                
                // Determine performance rating
                var score = testSession.TotalScore ?? 0;
                var performanceRating = DeterminePerformanceRating(score);

                // Create quiz history request
                var request = new CreateStudentQuizHistoryRequest
                {
                    UserId = testSession.UserId,
                    ExamId = testSession.ExamId,
                    TestSessionId = testSessionId,
                    StartedAt = testSession.StartedAt ?? testSession.CreatedAt,
                    CompletedAt = testSession.CompletedAt,
                    TimeSpent = testSession.TimeSpent,
                    TotalQuestions = totalQuestions,
                    CorrectAnswers = correctAnswers,
                    IncorrectAnswers = incorrectAnswers,
                    SkippedQuestions = skippedQuestions,
                    TotalScore = score,
                    PassingScore = testSession.PassingScore,
                    IsPassed = testSession.IsPassed,
                    QuizStatus = "COMPLETED",
                    PerformanceRating = performanceRating,
                    DeviceInfo = testSession.DeviceInfo,
                    SessionData = testSession.SessionData,
                    IsCheatingDetected = testSession.IsCheatingDetected,
                    CheatingDetails = testSession.CheatingDetails
                };

                // Analyze session answers for detailed insights
                if (sessionAnswers.Any())
                {
                    await AnalyzeSessionAnswersAsync(request, sessionAnswers, testSession.ExamId);
                }

                // Create the quiz history
                var historyId = await _studentHistoryService.CreateQuizHistoryAsync(request);
                
                _logger.LogInformation($"Created quiz history {historyId} from test session {testSessionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating quiz history from test session {testSessionId}");
                throw;
            }
        }

        private async Task AnalyzeSessionAnswersAsync(CreateStudentQuizHistoryRequest request, IEnumerable<TestSessionAnswer> sessionAnswers, int examId)
        {
            try
            {
                // Get exam questions with their topics and difficulty levels
                var examQuestions = await _unitOfWork.ExamQuestionRepository.GetByExamIdAsync(examId);
                var questionIds = examQuestions.Select(eq => eq.QuestionId).ToList();
                var allQuestions = await _unitOfWork.QuestionRepository.GetAllAsync();
                var questions = allQuestions.Where(q => questionIds.Contains(q.Id)).ToList();

                // Analyze difficulty performance
                var difficultyPerformance = new Dictionary<string, (int correct, int total)>();
                var topicPerformance = new Dictionary<string, (int correct, int total, int timeSpent)>();
                var weakAreas = new List<string>();
                var strongAreas = new List<string>();

                foreach (var answer in sessionAnswers)
                {
                    var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                    if (question == null) continue;

                    var difficulty = question.DifficultyLevel?.Name ?? "MEDIUM";
                    var topic = "General"; // Question model doesn't have Topic field

                    // Track difficulty performance
                    if (!difficultyPerformance.ContainsKey(difficulty))
                        difficultyPerformance[difficulty] = (0, 0);
                    
                    var (correct, total) = difficultyPerformance[difficulty];
                    difficultyPerformance[difficulty] = (
                        correct + (answer.IsCorrect == true ? 1 : 0),
                        total + 1
                    );

                    // Track topic performance
                    if (!topicPerformance.ContainsKey(topic))
                        topicPerformance[topic] = (0, 0, 0);
                    
                    var (topicCorrect, topicTotal, topicTime) = topicPerformance[topic];
                    topicPerformance[topic] = (
                        topicCorrect + (answer.IsCorrect == true ? 1 : 0),
                        topicTotal + 1,
                        topicTime + answer.TimeSpent
                    );
                }

                // Identify weak and strong areas
                foreach (var (topic, (correct, total, timeSpent)) in topicPerformance)
                {
                    var accuracy = total > 0 ? (decimal)correct / total * 100 : 0;
                    if (accuracy < 60)
                        weakAreas.Add(topic);
                    else if (accuracy >= 80)
                        strongAreas.Add(topic);
                }

                // Set analysis results
                request.DifficultyBreakdown = System.Text.Json.JsonSerializer.Serialize(difficultyPerformance);
                request.TopicPerformance = System.Text.Json.JsonSerializer.Serialize(topicPerformance);
                request.WeakAreas = System.Text.Json.JsonSerializer.Serialize(weakAreas);
                request.StrongAreas = System.Text.Json.JsonSerializer.Serialize(strongAreas);

                // Generate improvement suggestions
                var improvementAreas = new List<string>();
                if (weakAreas.Any())
                    improvementAreas.Add($"Focus on improving: {string.Join(", ", weakAreas.Take(3))}");
                
                var avgTimePerQuestion = sessionAnswers.Any() ? sessionAnswers.Average(a => a.TimeSpent) : 0;
                if (avgTimePerQuestion > 120) // More than 2 minutes per question
                    improvementAreas.Add("Work on time management skills");

                request.ImprovementAreas = System.Text.Json.JsonSerializer.Serialize(improvementAreas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing session answers");
                // Don't throw - this is supplementary analysis
            }
        }

        private string DeterminePerformanceRating(decimal score)
        {
            return score switch
            {
                >= 90 => "EXCELLENT",
                >= 80 => "GOOD",
                >= 70 => "AVERAGE",
                >= 60 => "NEEDS_IMPROVEMENT",
                _ => "POOR"
            };
        }
    }
}
