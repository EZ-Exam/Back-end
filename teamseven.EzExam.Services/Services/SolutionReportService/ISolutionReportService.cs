using System.Collections.Generic;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Dtos;

namespace teamseven.EzExam.Services.Services.SolutionReportService
{
    public interface ISolutionReportService
    {
        Task AddAsync(SolutionReportRequest request);
        Task<SolutionReportResponse> GetByIdAsync(int id);
        Task<List<SolutionReportResponse>> GetBySolutionIdAsync(int solutionId);
        Task UpdateStatusAsync(int id, string newStatus);
        Task DeleteAsync(int id);
    }
}
