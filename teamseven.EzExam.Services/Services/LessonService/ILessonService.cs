using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.LessonService
{
    public interface ILessonService
    {
        Task<IEnumerable<LessonDataResponse>> GetAllLessonAsync();
        Task<IEnumerable<LessonDataResponse>> GetLessonsByChapterIdAsync(int chapterId);

        Task<PagedResponse<LessonDataResponse>> GetLessonsAsync(
            int? pageNumber = null,
               int? pageSize = null,
               string? search = null,
               string? sort = null,
               int? chapterId = null,
               int isSort = 0);

        Task<LessonDataResponse> GetLessonByIdAsync(int id);
        Task CreateLessonAsync(CreateLessonRequest request);
        Task UpdateLessonAsync(LessonDataRequest request);
        Task DeleteLessonAsync(int id);

        // Optimized endpoints (labelled 'optimized') for lessons
        Task<PagedResponse<LessonFeedResponse>> GetOptimizedLessonsFeedAsync(
            int currentUserId,
            int page = 1,
            int pageSize = 20,
            string? search = null,
            int? chapterId = null,
            int isSort = 0);

        Task<LessonDetailOptimizedResponse> GetOptimizedLessonDetailsAsync(int lessonId, int currentUserId = 0);
    }
}