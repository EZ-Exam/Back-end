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

            var qids = new List<int>();
            foreach (var s in req.questions ?? Enumerable.Empty<string>())
            {
                if (!int.TryParse(s, out var qid))
                    throw new ArgumentException($"Invalid questionId: {s}");
                qids.Add(qid);
            }

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

            await _repo.ReplaceLessonQuestionsAsync(newId, qids);

            return await GetByIdAsync(newId);
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
