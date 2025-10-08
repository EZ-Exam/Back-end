using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.ExamService
{
    public interface IExamService
    {
        Task<int> CreateExamAsync (ExamRequest examRequest);
        Task<ExamResponse> UpdateExamAsync(UpdateExamRequest updateExamRequest);

        Task<IEnumerable<ExamResponse>> GetAllExamAsync();
        Task RenameExamAsync(int examId, string newName);

        Task SoftDeleteExamAsync(int examId);
        Task<ExamResponse> GetExamAsync(int id);
        Task RecoverExamAsync(int examId);
        Task CreateExamQuestionAsync(ExamQuestionRequest examQuestionRequest);

        Task<IEnumerable<ExamResponse>> GetExamsByUserIdAsync(int userId);

        Task<IEnumerable<ExamQuestionResponse>> GetExamQuestionByIdAsync(int id);

        Task RemoveExamQuestion(ExamQuestionRequest examQuestionRequest);

        Task CreateExamHistoryAsync(ExamHistoryRequest examHistoryRequest);

        Task DeleteExamHistoryAsync(ExamHistoryRequest historyRequest);

        Task<ExamHistoryResponseDto> GetExamHistoryResponseAsync(int id);
         Task<PagedResponse<ExamResponse>> GetExamsAsync(
                int? pageNumber = null,
                int? pageSize = null,
                string? search = null,
                string? sort = null,
                int? subjectId = null,
                int? lessonId = null,
                int? examTypeId = null,
                int? createdByUserId = null,
                int isSort = 0
            );
        

    }
}
