using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Interfaces
{
    public interface IExamHistoryService
    {
        Task<int> CreateExamHistoryAsync(CreateExamHistoryRequest request);

        Task<ExamHistoryResponse?> GetExamHistoryByIdAsync(int id);
        Task<IEnumerable<ExamHistoryResponse>> GetExamHistoriesByUserIdAsync(int userId);
        Task<IEnumerable<ExamHistoryMinimalResponse>> GetExamHistoriesMinimalByUserIdAsync(int userId);
        Task<IEnumerable<ExamHistoryResponse>> GetExamHistoriesByExamIdAsync(int examId);
        Task<IEnumerable<ExamHistoryResponse>> GetAllExamHistoriesAsync();

        Task UpdateExamHistoryAsync(int id, CreateExamHistoryRequest request);

        Task DeleteExamHistoryAsync(int id);

        Task<IEnumerable<ExamQuestionDetailResponse>> GetExamQuestionsDetailAsync(int examId);
    }
}
