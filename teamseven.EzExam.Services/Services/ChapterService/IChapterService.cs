using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.ChapterService
{
    public interface IChapterService
    {
        Task<IEnumerable<ChapterDataResponse>> GetAllChaptersAsync();
        Task<ChapterDataResponse> GetChapterByIdAsync(int id);
        Task UpdateChapterAsync(ChapterDataRequest request);
        Task CreateChapterAsync(CreateChapterRequest request);
        Task DeleteChapterAsync(int id);
    }
}
