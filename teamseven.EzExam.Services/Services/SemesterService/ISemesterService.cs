using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.SemesterService
{
    public interface ISemesterService
    {
        Task<IEnumerable<SemesterDataResponse>> GetAllSemesterAsync();
        Task<SemesterDataResponse> GetSemesterByIdAsync(int id);
        Task<IEnumerable<SemesterDataResponse>> GetSemesterByGradeIdAsync(int gradeId);
        Task CreateSemesterAsync(CreateSemesterRequest request);
        Task UpdateSemesterAsync(SemesterDataRequest request);
        Task DeleteSemesterAsync(int id);
    }
}
