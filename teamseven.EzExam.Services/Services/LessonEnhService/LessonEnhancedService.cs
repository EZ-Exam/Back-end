using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository.Repository;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.LessonEnhancedService
{
    public class LessonEnhancedService : ILessonEnhancedService
    {
        private readonly LessonEnhancedRepository _repo;
        private readonly ILogger<LessonEnhancedService> _logger;

        public LessonEnhancedService(LessonEnhancedRepository repo, ILogger<LessonEnhancedService> logger)
        {
            _repo = repo;
            _logger = logger;
        }
        public async Task<LessonEnhancedResponse> CreateAsync(LessonEnhancedUpsertRequest req)
        {
            if (!int.TryParse(req.subjectId, out var sid))
                throw new ArgumentException("Invalid subjectId");

            // Parse các questionId từ string -> int (bỏ qua id parse lỗi)
            var parsedQids = (req.questions ?? Enumerable.Empty<string>())
                .Select(s => int.TryParse(s, out var id) ? (int?)id : null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

            // Lọc chỉ giữ QuestionId có tồn tại trong DB
            var existingQids = await _repo.FilterExistingQuestionIdsAsync(parsedQids);

            // Tao lesson
            var entity = new LessonEnhanced
            {
                Title = req.title,
                Description = req.description,
                SubjectId = sid,
                PdfUrl = req.pdfUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var newId = await _repo.AddAsync(entity);

            await _repo.AppendLessonQuestionsAsync(newId, existingQids);
            // await _repo.ReplaceLessonQuestionsAsync(newId, existingQids);

            return await GetByIdAsync(newId);
        }

        // LessonEnhancedService.cs
        public async Task<List<LessonEnhancedResponse>> GetAllAsync(string? subjectId = null, bool includeQuestions = false)
        {
            // repo đã có GetAllAsync(string? subjectId)
            var lessons = await _repo.GetAllAsync(subjectId);

            var result = lessons.Select(l => new LessonEnhancedResponse
            {
                id = l.Id.ToString(),
                title = l.Title,
                description = l.Description,
                subjectId = l.SubjectId.ToString(),
                pdfUrl = l.PdfUrl,
                createdAt = l.CreatedAt,
                updatedAt = l.UpdatedAt,
                questions = new List<string>() // sẽ fill bên dưới nếu includeQuestions
            }).ToList();

            if (includeQuestions && result.Count > 0)
            {
                // repo trả về map<int, List<int>> theo lessonId (int)
                var map = await _repo.GetQuestionsForLessonsAsync(result.Select(r => r.id)); // IEnumerable<string>

                foreach (var r in result)
                {
                    if (int.TryParse(r.id, out var lid) && map.TryGetValue(lid, out var qids))
                    {
                        r.questions = qids.Select(q => q.ToString()).ToList();
                    }
                    else
                    {
                        r.questions = new List<string>();
                    }
                }
            }

            return result;
        }



        public async Task<LessonEnhancedResponse> GetByIdAsync(int id)
        {
            var l = await _repo.GetByIdAsync(id);
            if (l == null)
            {
                _logger.LogWarning("LessonEnhanced {Id} not found", id);
                throw new KeyNotFoundException($"Lesson {id} not found");
            }

            var q = await _repo.GetQuestionIdsAsync(id);

            return new LessonEnhancedResponse
            {
                id = l.Id.ToString(),
                title = l.Title,
                description = l.Description,
                subjectId = l.SubjectId.ToString(),
                pdfUrl = l.PdfUrl,
                questions = q.Select(x => x.ToString()).ToList(),
                createdAt = l.CreatedAt,
                updatedAt = l.UpdatedAt
            };
        }

        public async Task<List<LessonEnhancedResponse>> GetByQuestionIdAsync(int questionId)
        {
            var lessons = await _repo.GetByQuestionIdAsync(questionId);
            var result = new List<LessonEnhancedResponse>(lessons.Count);

            foreach (var l in lessons)
            {
                var q = await _repo.GetQuestionIdsAsync(l.Id);
                result.Add(new LessonEnhancedResponse
                {
                    id = l.Id.ToString(),
                    title = l.Title,
                    description = l.Description,
                    subjectId = l.SubjectId.ToString(),
                    pdfUrl = l.PdfUrl,
                    questions = q.Select(x => x.ToString()).ToList(),
                    createdAt = l.CreatedAt,
                    updatedAt = l.UpdatedAt
                });
            }
            return result;
        }
    }
}
