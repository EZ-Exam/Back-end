using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Interfaces
{
    public interface IExamHistoryService
    {
        // Create
        Task<int> CreateExamHistoryAsync(CreateExamHistoryRequest request);

        // Read
        Task<ExamHistoryResponse?> GetExamHistoryByIdAsync(int id);
        Task<IEnumerable<ExamHistoryResponse>> GetExamHistoriesByUserIdAsync(int userId);
        Task<IEnumerable<ExamHistoryResponse>> GetExamHistoriesByExamIdAsync(int examId);
        Task<IEnumerable<ExamHistoryResponse>> GetAllExamHistoriesAsync();

        // Update
        Task UpdateExamHistoryAsync(int id, CreateExamHistoryRequest request);

        // Delete
        Task DeleteExamHistoryAsync(int id);

        // Exam Questions Detail
        Task<IEnumerable<ExamQuestionDetailResponse>> GetExamQuestionsDetailAsync(int examId);
    }
}
