using System.Collections.Generic;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.LessonEnhancedService
{
    public interface ILessonEnhancedService
    {
        Task<LessonEnhancedResponse> CreateAsync(LessonEnhancedUpsertRequest req);
        Task<LessonEnhancedResponse> GetByIdAsync(int id);
        Task<List<LessonEnhancedResponse>> GetByQuestionIdAsync(int questionId);
        Task<List<LessonEnhancedResponse>> GetAllAsync(string? subjectId = null, bool includeQuestions = false);

    }
}
