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
        Task<IEnumerable<ExamHistoryResponse>> GetExamHistoriesByUserIdAsync(string userId);
        Task<IEnumerable<ExamHistoryResponse>> GetExamHistoriesByExamIdAsync(string examId);
        Task<IEnumerable<ExamHistoryResponse>> GetAllExamHistoriesAsync();

        // Update
        Task UpdateExamHistoryAsync(int id, CreateExamHistoryRequest request);

        // Delete
        Task DeleteExamHistoryAsync(int id);
    }
}
