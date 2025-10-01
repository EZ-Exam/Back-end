using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.TextBookService
{
    public interface ITextBookService
    {
        Task<IEnumerable<TextBookDataResponse>> GetAllTextBookAsync();
        Task<List<TextBookDataResponse>> GetAsync(int? gradeId = null, int? subjectId = null);
        Task<TextBookDataResponse> GetTextBookByIdAsync(int id);
        Task CreateTextBookAsync(CreateTextBookRequest request);
        Task UpdateTextBookAsync(TextBookDataRequest request);
        Task DeleteTextBookAsync(int id);
    }
}
