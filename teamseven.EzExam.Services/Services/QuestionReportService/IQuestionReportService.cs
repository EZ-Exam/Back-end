using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Object.Requests;

namespace teamseven.EzExam.Services.Services.QuestionReportService
{
    public interface IQuestionReportService
    {
        Task<IEnumerable<QuestionReport>> GetAllReportsAsync();
        Task<QuestionReport> GetReportByIdAsync(int id);
        Task CreateReportAsync(CreateQuestionReportRequest request);
        Task UpdateReportAsync(UpdateQuestionReportRequest request);
    }
}
