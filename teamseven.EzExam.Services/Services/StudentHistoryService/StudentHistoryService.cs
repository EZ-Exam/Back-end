using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.StudentHistoryService
{
    public class StudentHistoryService : IStudentHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StudentHistoryService> _logger;

        public StudentHistoryService(IUnitOfWork unitOfWork, ILogger<StudentHistoryService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Student Quiz History Management

        public async Task<int> CreateQuizHistoryAsync(CreateStudentQuizHistoryRequest request)
        {
            try
            {
                var history = new StudentQuizHistory
                {
                    UserId = request.UserId,
                    ExamId = request.ExamId,
                    TestSessionId = request.TestSessionId,
                    StartedAt = request.StartedAt,
                    CompletedAt = request.CompletedAt,
                    TimeSpent = request.TimeSpent,
                    TotalQuestions = request.TotalQuestions,
                    CorrectAnswers = request.CorrectAnswers,
                    IncorrectAnswers = request.IncorrectAnswers,
                    SkippedQuestions = request.SkippedQuestions,
                    TotalScore = request.TotalScore,
                    PassingScore = request.PassingScore,
                    IsPassed = request.IsPassed,
                    QuizStatus = request.QuizStatus,
                    AverageTimePerQuestion = request.TotalQuestions > 0 ? (decimal)request.TimeSpent / request.TotalQuestions : 0,
                    DifficultyBreakdown = request.DifficultyBreakdown,
                    TopicPerformance = request.TopicPerformance,
                    WeakAreas = request.WeakAreas,
                    StrongAreas = request.StrongAreas,
                    ImprovementAreas = request.ImprovementAreas,
                    PerformanceRating = request.PerformanceRating,
                    DeviceInfo = request.DeviceInfo,
                    SessionData = request.SessionData,
                    IsCheatingDetected = request.IsCheatingDetected,
                    CheatingDetails = request.CheatingDetails,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Calculate comparison to previous attempt
                var previousHistory = await _unitOfWork.StudentQuizHistoryRepository.GetLatestHistoryByUserAsync(request.UserId);
                if (previousHistory != null)
                {
                    history.ComparedToPrevious = request.TotalScore - previousHistory.TotalScore;
                }

                await _unitOfWork.StudentQuizHistoryRepository.CreateAsync(history);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                // Create question attempts if provided
                if (request.QuestionAttempts.Any())
                {
                    await CreateQuestionAttemptsAsync(history.Id, request.QuestionAttempts);
                }

                // Update performance summary
                await UpdatePerformanceSummaryAsync(request.UserId, null);

                _logger.LogInformation($"Created quiz history {history.Id} with {request.QuestionAttempts.Count} question attempts for user {request.UserId}");
                return history.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating quiz history for user {request.UserId}");
                throw;
            }
        }

        public async Task<StudentQuizHistoryResponse?> GetQuizHistoryByIdAsync(int id)
        {
            var history = await _unitOfWork.StudentQuizHistoryRepository.GetByIdAsync(id);
            if (history == null) return null;

            return MapToResponse(history);
        }

        public async Task<IEnumerable<StudentQuizHistoryResponse>> GetQuizHistoriesByUserIdAsync(int userId)
        {
            var histories = await _unitOfWork.StudentQuizHistoryRepository.GetHistoryByUserIdAsync(userId);
            return histories.Select(MapToResponse);
        }

        public async Task<IEnumerable<StudentQuizHistoryResponse>> GetRecentQuizHistoriesByUserIdAsync(int userId, int count = 5)
        {
            var histories = await _unitOfWork.StudentQuizHistoryRepository.GetRecentHistoryByUserIdAsync(userId, count);
            return histories.Select(MapToResponse);
        }

        public async Task<IEnumerable<StudentQuizHistoryResponse>> GetQuizHistoriesByUserAndSubjectAsync(int userId, int subjectId)
        {
            var histories = await _unitOfWork.StudentQuizHistoryRepository.GetHistoryByUserAndSubjectAsync(userId, subjectId);
            return histories.Select(MapToResponse);
        }

        public async Task<IEnumerable<StudentQuizHistoryResponse>> GetQuizHistoriesByExamIdAsync(int examId)
        {
            var histories = await _unitOfWork.StudentQuizHistoryRepository.GetHistoryByExamIdAsync(examId);
            return histories.Select(MapToResponse);
        }

        public async Task UpdateQuizHistoryAsync(int id, CreateStudentQuizHistoryRequest request)
        {
            var history = await _unitOfWork.StudentQuizHistoryRepository.GetByIdAsync(id);
            if (history == null)
                throw new ArgumentException("Quiz history not found");

            // Update properties
            history.TimeSpent = request.TimeSpent;
            history.TotalQuestions = request.TotalQuestions;
            history.CorrectAnswers = request.CorrectAnswers;
            history.IncorrectAnswers = request.IncorrectAnswers;
            history.SkippedQuestions = request.SkippedQuestions;
            history.TotalScore = request.TotalScore;
            history.PassingScore = request.PassingScore;
            history.IsPassed = request.IsPassed;
            history.QuizStatus = request.QuizStatus;
            history.AverageTimePerQuestion = request.TotalQuestions > 0 ? (decimal)request.TimeSpent / request.TotalQuestions : 0;
            history.DifficultyBreakdown = request.DifficultyBreakdown;
            history.TopicPerformance = request.TopicPerformance;
            history.WeakAreas = request.WeakAreas;
            history.StrongAreas = request.StrongAreas;
            history.ImprovementAreas = request.ImprovementAreas;
            history.PerformanceRating = request.PerformanceRating;
            history.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.StudentQuizHistoryRepository.UpdateAsync(history);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            // Update performance summary
            await UpdatePerformanceSummaryAsync(history.UserId, null);
        }

        public async Task DeleteQuizHistoryAsync(int id)
        {
            var history = await _unitOfWork.StudentQuizHistoryRepository.GetByIdAsync(id);
            if (history == null)
                throw new ArgumentException("Quiz history not found");

            var userId = history.UserId;
            await _unitOfWork.StudentQuizHistoryRepository.DeleteAsync(history);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            // Update performance summary
            await UpdatePerformanceSummaryAsync(userId, null);
        }

        #endregion

        #region Student Performance Summary Management

        public async Task<StudentPerformanceSummaryResponse?> GetPerformanceSummaryAsync(int userId, int? subjectId = null)
        {
            var summary = await _unitOfWork.StudentPerformanceSummaryRepository.GetByUserIdAsync(userId, subjectId);
            if (summary == null) return null;

            var response = MapToSummaryResponse(summary);

            // Get recent quizzes
            var recentQuizzes = await GetRecentQuizHistoriesByUserIdAsync(userId, 5);
            response.RecentQuizzes = recentQuizzes.ToList();

            return response;
        }

        public async Task<IEnumerable<StudentPerformanceSummaryResponse>> GetAllPerformanceSummariesByUserAsync(int userId)
        {
            var summaries = await _unitOfWork.StudentPerformanceSummaryRepository.GetAllByUserIdAsync(userId);
            return summaries.Select(MapToSummaryResponse);
        }

        public async Task UpdatePerformanceSummaryAsync(int userId, int? subjectId = null)
        {
            try
            {
                var summary = await _unitOfWork.StudentPerformanceSummaryRepository.GetOrCreateAsync(userId, subjectId);
                if (summary == null) return;

                // Get recent quiz histories
                var recentQuizzes = subjectId.HasValue
                    ? await _unitOfWork.StudentQuizHistoryRepository.GetHistoryByUserAndSubjectAsync(userId, subjectId.Value)
                    : await _unitOfWork.StudentQuizHistoryRepository.GetRecentHistoryByUserIdAsync(userId, 5);

                var completedQuizzes = recentQuizzes.Where(q => q.CompletedAt != null).OrderByDescending(q => q.CompletedAt).Take(5).ToList();

                if (!completedQuizzes.Any()) return;

                // Update basic metrics
                summary.TotalQuizzesCompleted = await _unitOfWork.StudentQuizHistoryRepository.GetTotalQuizzesCompletedByUserAsync(userId, subjectId);
                summary.RecentQuizIds = JsonConvert.SerializeObject(completedQuizzes.Select(q => q.Id).ToList());
                summary.AverageScore = completedQuizzes.Average(q => q.TotalScore);
                summary.AverageTimePerQuiz = (decimal)completedQuizzes.Average(q => q.TimeSpent);
                summary.AverageTimePerQuestion = completedQuizzes.Average(q => q.AverageTimePerQuestion);
                summary.OverallAccuracy = completedQuizzes.Average(q => (decimal)q.CorrectAnswers / q.TotalQuestions * 100);

                // Calculate improvement trend
                if (completedQuizzes.Count >= 3)
                {
                    var firstHalf = completedQuizzes.Skip(completedQuizzes.Count / 2).Average(q => q.TotalScore);
                    var secondHalf = completedQuizzes.Take(completedQuizzes.Count / 2).Average(q => q.TotalScore);
                    var trendChange = secondHalf - firstHalf;

                    summary.TrendPercentage = trendChange;
                    summary.ImprovementTrend = trendChange > 5 ? "IMPROVING" : trendChange < -5 ? "DECLINING" : "STABLE";
                }

                // Calculate learning velocity (improvement per quiz)
                if (completedQuizzes.Count >= 2)
                {
                    var scores = completedQuizzes.OrderBy(q => q.CompletedAt).Select(q => q.TotalScore).ToList();
                    var improvements = new List<decimal>();
                    for (int i = 1; i < scores.Count; i++)
                    {
                        improvements.Add(scores[i] - scores[i - 1]);
                    }
                    summary.LearningVelocity = improvements.Average();
                }

                // Calculate consistency score (inverse of standard deviation)
                if (completedQuizzes.Count >= 3)
                {
                    var scores = completedQuizzes.Select(q => q.TotalScore).ToList();
                    var mean = scores.Average();
                    var variance = scores.Sum(s => (s - mean) * (s - mean)) / scores.Count;
                    var stdDev = (decimal)Math.Sqrt((double)variance);
                    summary.ConsistencyScore = Math.Max(0, 100 - (decimal)stdDev * 2); // Higher score = more consistent
                }

                // Set other calculated fields
                summary.LastQuizDate = completedQuizzes.First().CompletedAt;
                summary.LastAnalysisDate = DateTime.UtcNow;
                summary.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.StudentPerformanceSummaryRepository.UpdateAsync(summary);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation($"Updated performance summary for user {userId}, subject {subjectId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating performance summary for user {userId}, subject {subjectId}");
                throw;
            }
        }

        public async Task RecalculatePerformanceSummaryAsync(int userId, int? subjectId = null)
        {
            // Force recalculation by updating
            await UpdatePerformanceSummaryAsync(userId, subjectId);
        }

        #endregion

        #region Analytics and Insights

        public async Task<StudentCompetencyResponse> GetStudentCompetencyForAIAsync(int userId, int? subjectId = null)
        {
            var summary = await _unitOfWork.StudentPerformanceSummaryRepository.GetByUserIdAsync(userId, subjectId);
            var recentQuizzes = await _unitOfWork.StudentQuizHistoryRepository.GetRecentHistoryByUserIdAsync(userId, 5);

            var response = new StudentCompetencyResponse
            {
                UserId = userId,
                SubjectId = subjectId,
                OverallAverageScore = summary?.AverageScore ?? 0,
                OverallAccuracy = summary?.OverallAccuracy ?? 0,
                CurrentPerformanceLevel = DeterminePerformanceLevel(summary?.AverageScore ?? 0),
                ImprovementTrend = summary?.ImprovementTrend,
                TrendPercentage = summary?.TrendPercentage,
                RecentAverageScore = recentQuizzes.Any() ? recentQuizzes.Average(q => q.TotalScore) : 0,
                RecentAverageTime = recentQuizzes.Any() ? (decimal)recentQuizzes.Average(q => q.TimeSpent) : 0,
                ConsistencyScore = summary?.ConsistencyScore ?? 0,
                LearningVelocity = summary?.LearningVelocity ?? 0,
                ConfidenceLevel = summary?.ConfidenceLevel ?? 0,
                TimeManagementScore = summary?.TimeManagementScore ?? 0,
                PredictedNextScore = summary?.PredictedNextScore,
                LastAnalysisDate = summary?.LastAnalysisDate ?? DateTime.UtcNow,
                LastQuizDate = summary?.LastQuizDate
            };

            // Map recent quizzes
            response.RecentQuizzes = recentQuizzes.Select(q => new RecentQuizPerformance
            {
                QuizHistoryId = q.Id,
                ExamId = q.ExamId,
                ExamName = q.Exam?.Name,
                Score = q.TotalScore,
                TimeSpent = q.TimeSpent,
                TotalQuestions = q.TotalQuestions,
                CorrectAnswers = q.CorrectAnswers,
                CompletedAt = q.CompletedAt ?? q.StartedAt,
                PerformanceRating = q.PerformanceRating
            }).ToList();

            // Generate recommendations
            response.RecommendedTopics = GenerateTopicRecommendations(summary, recentQuizzes);
            response.AreasForImprovement = GenerateImprovementAreas(summary, recentQuizzes);
            response.OptimalQuestionDifficulty = DetermineOptimalDifficulty(summary?.AverageScore ?? 0);
            response.RecommendedQuestionCount = DetermineOptimalQuestionCount(summary?.AverageTimePerQuestion ?? 0);

            return response;
        }

        public async Task<IEnumerable<StudentPerformanceSummaryResponse>> GetTopPerformersAsync(int? subjectId = null, int count = 10)
        {
            var summaries = await _unitOfWork.StudentPerformanceSummaryRepository.GetTopPerformersAsync(subjectId, count);
            return summaries.Select(MapToSummaryResponse);
        }

        public async Task<IEnumerable<StudentPerformanceSummaryResponse>> GetStudentsNeedingImprovementAsync(int? subjectId = null, decimal threshold = 60.0m)
        {
            var summaries = await _unitOfWork.StudentPerformanceSummaryRepository.GetNeedingImprovementAsync(subjectId, threshold);
            return summaries.Select(MapToSummaryResponse);
        }

        #endregion

        #region Integration with Test Session

        public async Task CreateQuizHistoryFromTestSessionAsync(int testSessionId)
        {
            var testSession = await _unitOfWork.TestSessionRepository.GetByIdAsync(testSessionId);
            if (testSession == null || testSession.SessionStatus != "COMPLETED")
                return;

            // Check if history already exists
            var allHistories = await _unitOfWork.StudentQuizHistoryRepository.GetAllAsync();
            var existingHistory = allHistories.FirstOrDefault(h => h.TestSessionId == testSessionId);
            if (existingHistory != null)
                return;

            var request = new CreateStudentQuizHistoryRequest
            {
                UserId = testSession.UserId,
                ExamId = testSession.ExamId,
                TestSessionId = testSessionId,
                StartedAt = testSession.StartedAt ?? testSession.CreatedAt,
                CompletedAt = testSession.CompletedAt,
                TimeSpent = testSession.TimeSpent,
                TotalQuestions = testSession.TotalQuestions,
                CorrectAnswers = testSession.CorrectAnswers,
                IncorrectAnswers = testSession.TotalQuestions - testSession.CorrectAnswers,
                SkippedQuestions = 0, // Calculate from session data if available
                TotalScore = testSession.TotalScore ?? 0,
                PassingScore = testSession.PassingScore,
                IsPassed = testSession.IsPassed,
                QuizStatus = "COMPLETED",
                DeviceInfo = testSession.DeviceInfo,
                SessionData = testSession.SessionData,
                IsCheatingDetected = testSession.IsCheatingDetected,
                CheatingDetails = testSession.CheatingDetails
            };

            await CreateQuizHistoryAsync(request);
        }

        public async Task<bool> HasRecentQuizHistoryAsync(int userId, int examId, TimeSpan timeWindow)
        {
            var cutoffTime = DateTime.UtcNow - timeWindow;
            var histories = await _unitOfWork.StudentQuizHistoryRepository.GetHistoryByUserIdAsync(userId);
            return histories.Any(h => h.ExamId == examId && h.CreatedAt >= cutoffTime);
        }

        #endregion

        #region Batch Operations

        public async Task<int> CreateMultipleQuizHistoriesAsync(IEnumerable<CreateStudentQuizHistoryRequest> requests)
        {
            var count = 0;
            foreach (var request in requests)
            {
                await CreateQuizHistoryAsync(request);
                count++;
            }
            return count;
        }

        public async Task UpdateMultiplePerformanceSummariesAsync(IEnumerable<int> userIds, int? subjectId = null)
        {
            foreach (var userId in userIds)
            {
                await UpdatePerformanceSummaryAsync(userId, subjectId);
            }
        }

        #endregion

        #region Question Attempts Management

        public async Task CreateQuestionAttemptsAsync(int quizHistoryId, IEnumerable<StudentQuestionAttemptRequest> attempts)
        {
            try
            {
                var quizHistory = await _unitOfWork.StudentQuizHistoryRepository.GetByIdAsync(quizHistoryId);
                if (quizHistory == null)
                    throw new ArgumentException("Quiz history not found");

                var questionAttempts = attempts.Select(attempt => new StudentQuestionAttempt
                {
                    StudentQuizHistoryId = quizHistoryId,
                    QuestionId = attempt.QuestionId,
                    UserId = quizHistory.UserId,
                    SelectedAnswerId = attempt.SelectedAnswerId,
                    UserAnswer = attempt.UserAnswer,
                    IsCorrect = attempt.IsCorrect,
                    DifficultyLevel = attempt.DifficultyLevel,
                    TimeSpent = attempt.TimeSpent,
                    Topic = attempt.Topic,
                    ChapterId = attempt.ChapterId,
                    LessonId = attempt.LessonId,
                    QuestionOrder = attempt.QuestionOrder,
                    ConfidenceLevel = attempt.ConfidenceLevel,
                    IsMarkedForReview = attempt.IsMarkedForReview,
                    IsSkipped = attempt.IsSkipped,
                    AnswerChangeCount = attempt.AnswerChangeCount,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                foreach (var questionAttempt in questionAttempts)
                {
                    await _unitOfWork.StudentQuestionAttemptRepository.CreateAsync(questionAttempt);
                }

                await _unitOfWork.SaveChangesWithTransactionAsync();
                _logger.LogInformation($"Created {questionAttempts.Count} question attempts for quiz history {quizHistoryId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating question attempts for quiz history {quizHistoryId}");
                throw;
            }
        }

        public async Task<IEnumerable<StudentQuestionAttemptResponse>> GetQuestionAttemptsByQuizHistoryAsync(int quizHistoryId)
        {
            var attempts = await _unitOfWork.StudentQuestionAttemptRepository.GetByQuizHistoryIdAsync(quizHistoryId);
            return attempts.Select(MapToQuestionAttemptResponse);
        }

        public async Task<IEnumerable<StudentQuestionAttemptResponse>> GetQuestionAttemptsByUserAsync(int userId)
        {
            var attempts = await _unitOfWork.StudentQuestionAttemptRepository.GetByUserIdAsync(userId);
            return attempts.Select(MapToQuestionAttemptResponse);
        }

        public async Task<IEnumerable<StudentQuestionAttemptResponse>> GetCorrectAttemptsByUserAsync(int userId)
        {
            var attempts = await _unitOfWork.StudentQuestionAttemptRepository.GetCorrectAttemptsByUserAsync(userId);
            return attempts.Select(MapToQuestionAttemptResponse);
        }

        public async Task<IEnumerable<StudentQuestionAttemptResponse>> GetIncorrectAttemptsByUserAsync(int userId)
        {
            var attempts = await _unitOfWork.StudentQuestionAttemptRepository.GetIncorrectAttemptsByUserAsync(userId);
            return attempts.Select(MapToQuestionAttemptResponse);
        }

        public async Task<decimal> GetUserAccuracyAsync(int userId)
        {
            return await _unitOfWork.StudentQuestionAttemptRepository.GetAccuracyByUserAsync(userId);
        }

        public async Task<decimal> GetUserAccuracyByDifficultyAsync(int userId, string difficultyLevel)
        {
            return await _unitOfWork.StudentQuestionAttemptRepository.GetAccuracyByUserAndDifficultyAsync(userId, difficultyLevel);
        }

        public async Task<decimal> GetUserAccuracyByTopicAsync(int userId, string topic)
        {
            return await _unitOfWork.StudentQuestionAttemptRepository.GetAccuracyByUserAndTopicAsync(userId, topic);
        }

        #endregion

        #region Helper Methods

        private StudentQuizHistoryResponse MapToResponse(StudentQuizHistory history)
        {
            return new StudentQuizHistoryResponse
            {
                Id = history.Id,
                UserId = history.UserId,
                ExamId = history.ExamId,
                TestSessionId = history.TestSessionId,
                StartedAt = history.StartedAt,
                CompletedAt = history.CompletedAt,
                TimeSpent = history.TimeSpent,
                TotalQuestions = history.TotalQuestions,
                CorrectAnswers = history.CorrectAnswers,
                IncorrectAnswers = history.IncorrectAnswers,
                SkippedQuestions = history.SkippedQuestions,
                TotalScore = history.TotalScore,
                PassingScore = history.PassingScore,
                IsPassed = history.IsPassed,
                QuizStatus = history.QuizStatus,
                AverageTimePerQuestion = history.AverageTimePerQuestion,
                DifficultyBreakdown = history.DifficultyBreakdown,
                TopicPerformance = history.TopicPerformance,
                WeakAreas = history.WeakAreas,
                StrongAreas = history.StrongAreas,
                ImprovementAreas = history.ImprovementAreas,
                PerformanceRating = history.PerformanceRating,
                ComparedToPrevious = history.ComparedToPrevious,
                DeviceInfo = history.DeviceInfo,
                IsCheatingDetected = history.IsCheatingDetected,
                CheatingDetails = history.CheatingDetails,
                CreatedAt = history.CreatedAt,
                UpdatedAt = history.UpdatedAt,
                UserName = history.User?.Email, // User model doesn't have FirstName/LastName
                UserEmail = history.User?.Email,
                ExamName = history.Exam?.Name,
                SubjectName = history.Exam?.Subject?.Name,
                ExamTypeName = history.Exam?.ExamType?.Name
            };
        }

        private StudentPerformanceSummaryResponse MapToSummaryResponse(StudentPerformanceSummary summary)
        {
            return new StudentPerformanceSummaryResponse
            {
                Id = summary.Id,
                UserId = summary.UserId,
                SubjectId = summary.SubjectId,
                GradeId = summary.GradeId,
                TotalQuizzesCompleted = summary.TotalQuizzesCompleted,
                RecentQuizzesCount = summary.RecentQuizzesCount,
                RecentQuizIds = summary.RecentQuizIds,
                AverageScore = summary.AverageScore,
                AverageTimePerQuiz = summary.AverageTimePerQuiz,
                AverageTimePerQuestion = summary.AverageTimePerQuestion,
                OverallAccuracy = summary.OverallAccuracy,
                ImprovementTrend = summary.ImprovementTrend,
                TrendPercentage = summary.TrendPercentage,
                StrongTopics = summary.StrongTopics,
                WeakTopics = summary.WeakTopics,
                DifficultyProfile = summary.DifficultyProfile,
                RecommendedDifficulty = summary.RecommendedDifficulty,
                LearningVelocity = summary.LearningVelocity,
                ConsistencyScore = summary.ConsistencyScore,
                PredictedNextScore = summary.PredictedNextScore,
                ConfidenceLevel = summary.ConfidenceLevel,
                TimeManagementScore = summary.TimeManagementScore,
                LastQuizDate = summary.LastQuizDate,
                LastAnalysisDate = summary.LastAnalysisDate,
                CreatedAt = summary.CreatedAt,
                UpdatedAt = summary.UpdatedAt,
                UserName = summary.User?.Email, // User model doesn't have FirstName/LastName
                UserEmail = summary.User?.Email,
                SubjectName = summary.Subject?.Name,
                GradeName = summary.Grade?.Name
            };
        }

        private string DeterminePerformanceLevel(decimal averageScore)
        {
            return averageScore switch
            {
                >= 90 => "EXCELLENT",
                >= 80 => "GOOD",
                >= 70 => "AVERAGE",
                >= 60 => "NEEDS_IMPROVEMENT",
                _ => "POOR"
            };
        }

        private List<string> GenerateTopicRecommendations(StudentPerformanceSummary? summary, IEnumerable<StudentQuizHistory> recentQuizzes)
        {
            var recommendations = new List<string>();
            
            if (summary?.WeakTopics != null)
            {
                try
                {
                    var weakTopics = JsonConvert.DeserializeObject<List<string>>(summary.WeakTopics);
                    if (weakTopics != null)
                        recommendations.AddRange(weakTopics.Take(3));
                }
                catch { }
            }

            return recommendations;
        }

        private List<string> GenerateImprovementAreas(StudentPerformanceSummary? summary, IEnumerable<StudentQuizHistory> recentQuizzes)
        {
            var areas = new List<string>();

            if (summary?.AverageScore < 70)
                areas.Add("Focus on fundamental concepts");
            
            if (summary?.TimeManagementScore < 60)
                areas.Add("Improve time management skills");
            
            if (summary?.ConsistencyScore < 70)
                areas.Add("Work on maintaining consistent performance");

            return areas;
        }

        private StudentQuestionAttemptResponse MapToQuestionAttemptResponse(StudentQuestionAttempt attempt)
        {
            return new StudentQuestionAttemptResponse
            {
                Id = attempt.Id,
                StudentQuizHistoryId = attempt.StudentQuizHistoryId,
                QuestionId = attempt.QuestionId,
                UserId = attempt.UserId,
                SelectedAnswerId = attempt.SelectedAnswerId,
                UserAnswer = attempt.UserAnswer,
                IsCorrect = attempt.IsCorrect,
                DifficultyLevel = attempt.DifficultyLevel,
                TimeSpent = attempt.TimeSpent,
                Topic = attempt.Topic,
                ChapterId = attempt.ChapterId,
                LessonId = attempt.LessonId,
                QuestionOrder = attempt.QuestionOrder,
                ConfidenceLevel = attempt.ConfidenceLevel,
                IsMarkedForReview = attempt.IsMarkedForReview,
                IsSkipped = attempt.IsSkipped,
                AnswerChangeCount = attempt.AnswerChangeCount,
                CreatedAt = attempt.CreatedAt,
                QuestionContent = attempt.Question?.Content,
                QuestionType = "Multiple Choice", // Question model doesn't have Type field
                SelectedAnswerContent = attempt.SelectedAnswer?.Content,
                ChapterName = attempt.Chapter?.Name,
                LessonName = attempt.Lesson?.Name
            };
        }

        private string DetermineOptimalDifficulty(decimal averageScore)
        {
            return averageScore switch
            {
                >= 85 => "HARD",
                >= 70 => "MEDIUM",
                _ => "EASY"
            };
        }

        private int DetermineOptimalQuestionCount(decimal averageTimePerQuestion)
        {
            // Base on average time per question to suggest optimal count
            return averageTimePerQuestion switch
            {
                <= 30 => 20, // Fast answering - can handle more questions
                <= 60 => 15, // Average speed
                _ => 10      // Slower - fewer questions
            };
        }

        #endregion
    }
}
