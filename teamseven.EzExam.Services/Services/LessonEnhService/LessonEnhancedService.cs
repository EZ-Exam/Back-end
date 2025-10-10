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

        public async Task<PagedResponse<LessonEnhancedResponse>> GetPagedAsync(
           int? pageNumber = null,
           int? pageSize = null,
           string? search = null,
           string? sort = null,
           string? subjectId = null,
           string? questionId = null,
           int isSort = 0,
           bool includeQuestions = false)
        {
            int? sid = null, qid = null;
            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                if (!int.TryParse(subjectId, out var s))
                    throw new ArgumentException("subjectId must be integer");
                sid = s;
            }
            if (!string.IsNullOrWhiteSpace(questionId))
            {
                if (!int.TryParse(questionId, out var q))
                    throw new ArgumentException("questionId must be integer");
                qid = q;
            }

            List<LessonEnhanced> items; int total;
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                (items, total) = await _repo.GetPagedAsync(pageNumber.Value, pageSize.Value, search, sort, sid, qid, isSort);
            }
            else
            {
                (items, total) = await _repo.GetPagedAsync(1, int.MaxValue, search, sort, sid, qid, isSort);
                pageNumber ??= 1; pageSize ??= total;
            }

            var responses = items.Select(l => new LessonEnhancedResponse
            {
                id = l.Id.ToString(),
                title = l.Title,
                description = l.Description,
                subjectId = l.SubjectId.ToString(),
                pdfUrl = l.PdfUrl,
                createdAt = l.CreatedAt,
                updatedAt = l.UpdatedAt,
                questions = new List<string>() // fill sau nếu includeQuestions
            }).ToList();

            if (includeQuestions && responses.Count > 0)
            {
                // Thu thập ID hợp lệ vào 1 List<int> chắc chắn non-null
                var lessonIds = new List<int>();
                foreach (var r in responses)
                {
                    if (int.TryParse(r.id, out var v))
                        lessonIds.Add(v);
                }

                if (lessonIds.Count > 0)
                {
                    var map = await _repo.GetQuestionsForLessonsAsync(lessonIds); // <- giờ không còn cảnh báo
                    foreach (var r in responses)
                    {
                        if (int.TryParse(r.id, out var lid) && map.TryGetValue(lid, out var qlist))
                            r.questions = qlist.Select(x => x.ToString()).ToList();
                    }
                }
            }


            return new PagedResponse<LessonEnhancedResponse>(responses, pageNumber!.Value, pageSize!.Value, total);
        }
    }
    }

