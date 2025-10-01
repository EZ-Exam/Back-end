using System.Collections.Generic;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Dtos;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.QuestionsService
{
    public interface IQuestionsService
    {
        Task<QuestionDataResponse> AddQuestionAsync(QuestionDataRequest questionDataRequest);
        Task DeleteQuestionAsync(int id);
        Task<QuestionDataResponse> GetQuestionById(int id);
        Task<QuestionDataResponse> ModifyQuestionAsync(UpdateQuestionRequest questionDataRequest);
        Task<List<QuestionDataResponse>> GetQuestionBySubjectIdAsync(int subjectId);
        Task<PagedResponse<QuestionDataResponse>> GetQuestionsAsync(
            int? pageNumber = null,
            int? pageSize = null,
            string? search = null,
            string? sort = null,
            int? lessonId = null,
            string? difficultyLevel = null,
            int? chapterId = null,
            int isSort = 0,
            int? createdByUserId = null);
    }
}
