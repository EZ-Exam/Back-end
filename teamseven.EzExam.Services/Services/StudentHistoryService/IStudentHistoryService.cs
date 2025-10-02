using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.StudentHistoryService
{
    public interface IStudentHistoryService
    {
        // Student Quiz History Management
        Task<int> CreateQuizHistoryAsync(CreateStudentQuizHistoryRequest request);
        Task<StudentQuizHistoryResponse?> GetQuizHistoryByIdAsync(int id);
        Task<IEnumerable<StudentQuizHistoryResponse>> GetQuizHistoriesByUserIdAsync(int userId);
        Task<IEnumerable<StudentQuizHistoryResponse>> GetRecentQuizHistoriesByUserIdAsync(int userId, int count = 5);
        Task<IEnumerable<StudentQuizHistoryResponse>> GetQuizHistoriesByUserAndSubjectAsync(int userId, int subjectId);
        Task<IEnumerable<StudentQuizHistoryResponse>> GetQuizHistoriesByExamIdAsync(int examId);
        Task UpdateQuizHistoryAsync(int id, CreateStudentQuizHistoryRequest request);
        Task DeleteQuizHistoryAsync(int id);

        // Student Performance Summary Management
        Task<StudentPerformanceSummaryResponse?> GetPerformanceSummaryAsync(int userId, int? subjectId = null);
        Task<IEnumerable<StudentPerformanceSummaryResponse>> GetAllPerformanceSummariesByUserAsync(int userId);
        Task UpdatePerformanceSummaryAsync(int userId, int? subjectId = null);
        Task RecalculatePerformanceSummaryAsync(int userId, int? subjectId = null);

        // Analytics and Insights
        Task<StudentCompetencyResponse> GetStudentCompetencyForAIAsync(int userId, int? subjectId = null);
        Task<IEnumerable<StudentPerformanceSummaryResponse>> GetTopPerformersAsync(int? subjectId = null, int count = 10);
        Task<IEnumerable<StudentPerformanceSummaryResponse>> GetStudentsNeedingImprovementAsync(int? subjectId = null, decimal threshold = 60.0m);

        // Integration with Test Session
        Task CreateQuizHistoryFromTestSessionAsync(int testSessionId);
        Task<bool> HasRecentQuizHistoryAsync(int userId, int examId, TimeSpan timeWindow);

        // Batch Operations
        Task<int> CreateMultipleQuizHistoriesAsync(IEnumerable<CreateStudentQuizHistoryRequest> requests);
        Task UpdateMultiplePerformanceSummariesAsync(IEnumerable<int> userIds, int? subjectId = null);

        // Question Attempts Management
        Task CreateQuestionAttemptsAsync(int quizHistoryId, IEnumerable<StudentQuestionAttemptRequest> attempts);
        Task<IEnumerable<StudentQuestionAttemptResponse>> GetQuestionAttemptsByQuizHistoryAsync(int quizHistoryId);
        Task<IEnumerable<StudentQuestionAttemptResponse>> GetQuestionAttemptsByUserAsync(int userId);
        Task<IEnumerable<StudentQuestionAttemptResponse>> GetCorrectAttemptsByUserAsync(int userId);
        Task<IEnumerable<StudentQuestionAttemptResponse>> GetIncorrectAttemptsByUserAsync(int userId);
        Task<decimal> GetUserAccuracyAsync(int userId);
        Task<decimal> GetUserAccuracyByDifficultyAsync(int userId, string difficultyLevel);
        Task<decimal> GetUserAccuracyByTopicAsync(int userId, string topic);
    }
}
