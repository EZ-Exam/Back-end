using teamseven.EzExam.Repository.Dtos;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.GradeService
{
    public interface IGradeService
    {
        Task<IEnumerable<GradeDataResponse>> GetAllGradesAsync();
        Task<GradeDataResponse> GetGradeByIdAsync(int id);
        Task CreateGradeAsync(CreateGradeRequest request);
        Task UpdateGradeAsync(GradeDataRequest request);
        Task DeleteGradeAsync(string encodedId);
    }
}
