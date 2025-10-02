using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Interfaces
{
    public interface IAIExamGenerationService
    {
        Task<GenerateAIExamResponse> GenerateExamAsync(GenerateAIExamRequest request);
        Task<List<ExamHistoryResponse>> GetUserExamHistoryAsync(int userId, int count = 5);
        Task<List<QuestionSimpleResponse>> GetAvailableQuestionsAsync(GenerateAIExamRequest request);
        Task<string> BuildPromptAsync(GenerateAIExamRequest request, List<ExamHistoryResponse> history, List<QuestionSimpleResponse> questions);
        Task<GenerateAIExamResponse> ParseAIResponseAsync(string aiResponse, GenerateAIExamRequest request);
    }
}
