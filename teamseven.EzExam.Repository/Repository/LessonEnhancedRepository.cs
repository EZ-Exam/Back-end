using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class LessonEnhancedRepository : GenericRepository<LessonEnhanced>
    {

        public LessonEnhancedRepository(teamsevenezexamdbContext context) : base(context) { }

        public async Task<List<LessonEnhanced>> GetAllAsync(string? subjectId = null)
        {
            var q = _context.LessonsEnhanced.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                if (int.TryParse(subjectId, out var sid))
                {
                    q = q.Where(x => x.SubjectId == sid);
                }
                else
                {
                    return new List<LessonEnhanced>();
                }
            }

            return await q.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<Dictionary<int, List<int>>> GetQuestionsForLessonsAsync(IEnumerable<string> lessonIds)
        {
            var idList = (lessonIds ?? Enumerable.Empty<string>())
                .Select(s => int.TryParse(s, out var id) ? (int?)id : null)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToList();

            if (idList.Count == 0) return new();

            var rows = await _context.LessonsEnhancedQuestions
                .Where(x => idList.Contains(x.LessonId))
                .OrderBy(x => x.LessonId).ThenBy(x => x.Position)
                .Select(x => new { x.LessonId, x.QuestionId })
                .ToListAsync();

            return rows
                .GroupBy(r => r.LessonId)
                .ToDictionary(g => g.Key, g => g.Select(r => r.QuestionId).ToList());
        }
        public async Task<Dictionary<int, List<int>>> GetQuestionsForLessonsAsync(IReadOnlyCollection<int> lessonIds)
        {
            ArgumentNullException.ThrowIfNull(lessonIds);

            if (lessonIds.Count == 0)
                return new();

            var rows = await _context.LessonsEnhancedQuestions
                .Where(x => lessonIds.Contains(x.LessonId))
                .OrderBy(x => x.LessonId).ThenBy(x => x.Position)
                .Select(x => new { x.LessonId, x.QuestionId })
                .ToListAsync();

            return rows
                .GroupBy(r => r.LessonId)
                .ToDictionary(g => g.Key, g => g.Select(r => r.QuestionId).ToList());
        }

        public async Task<LessonEnhanced?> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<List<LessonEnhanced>?> GetBySubjectIdAsync(int subjectId)
        {
            return await _context.LessonsEnhanced
                .Where(l => l.SubjectId == subjectId)
                .ToListAsync();
        }

        public async Task<int> AddAsync(LessonEnhanced lesson)
        {
            _context.LessonsEnhanced.Add(lesson);
            await _context.SaveChangesAsync();
            return lesson.Id;
        }

        public async Task<int> UpdateAsync(LessonEnhanced lesson)
        {
            return await base.UpdateAsync(lesson);
        }

        public async Task<bool> DeleteAsync(LessonEnhanced lesson)
        {
            return await RemoveAsync(lesson);
        }

        public async Task<(List<LessonEnhanced> Items, int Total)> GetPagedAsync(
         int pageNumber,
         int pageSize,
         string? search = null,
         string? sort = null,
         int? subjectId = null,
         int? questionId = null,
         int isSort = 0)
        {
            IQueryable<LessonEnhanced> q = _context.LessonsEnhanced.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.RemoveDiacritics().ToLower();
                q = q.Where(x =>
                    (x.Title ?? "").RemoveDiacritics().ToLower().Contains(s) ||
                    (x.Description ?? "").RemoveDiacritics().ToLower().Contains(s));
            }
            if (subjectId.HasValue)
                q = q.Where(x => x.SubjectId == subjectId.Value);

            if (questionId.HasValue)
            {
                q = from le in q
                    join leq in _context.LessonsEnhancedQuestions.AsNoTracking()
                        on le.Id equals leq.LessonId
                    where leq.QuestionId == questionId.Value
                    select le;
                q = q.Distinct();
            }

            if (isSort == 1)
            {
                q = (sort ?? "").ToLower() switch
                {
                    "title:asc" => q.OrderBy(x => x.Title),
                    "title:desc" => q.OrderByDescending(x => x.Title),
                    "createdat:asc" => q.OrderBy(x => x.CreatedAt),
                    "createdat:desc" => q.OrderByDescending(x => x.CreatedAt),
                    "updatedat:asc" => q.OrderBy(x => x.UpdatedAt),
                    "updatedat:desc" => q.OrderByDescending(x => x.UpdatedAt),
                    _ => q.OrderByDescending(x => x.CreatedAt)
                };
            }
            else
            {
                q = q.OrderBy(x => x.Id);
            }

            var total = await q.CountAsync();
            var items = await q.Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

            return (items, total);
        }

        public async Task<List<int>> GetQuestionIdsAsync(int lessonId)
        {
            return await _context.LessonsEnhancedQuestions
                .Where(x => x.LessonId == lessonId)
                .OrderBy(x => x.Position)
                .Select(x => x.QuestionId)
                .ToListAsync();
        }
        public async Task<List<int>> FilterExistingQuestionIdsAsync(IEnumerable<int> ids)
        {
            var set = ids?.Distinct().ToList() ?? new List<int>();
            if (set.Count == 0) return new List<int>();

            return await _context.Questions
                .Where(q => set.Contains(q.Id))
                .Select(q => q.Id)
                .ToListAsync();
        }

        public async Task AppendLessonQuestionsAsync(int lessonId, IEnumerable<int> newQuestionIds)
        {
            var incoming = newQuestionIds?.Distinct().ToList() ?? new List<int>();
            if (incoming.Count == 0) return;

            var existing = await _context.LessonsEnhancedQuestions
                .Where(x => x.LessonId == lessonId)
                .ToListAsync();

            var existingIds = existing.Select(x => x.QuestionId).ToHashSet();
            var toAdd = incoming.Where(id => !existingIds.Contains(id)).ToList();
            if (toAdd.Count == 0) return;

            int startPos = existing.Count == 0 ? 1 : existing.Max(x => x.Position) + 1;

            var rows = toAdd.Select((qid, i) => new LessonEnhancedQuestion
            {
                LessonId = lessonId,
                QuestionId = qid,
                Position = startPos + i
            });

            await _context.LessonsEnhancedQuestions.AddRangeAsync(rows);
            await _context.SaveChangesAsync();
        }
        public async Task ReplaceLessonQuestionsAsync(int lessonId, IEnumerable<int> questionIds)
        {
            var olds = await _context.LessonsEnhancedQuestions
                                     .Where(x => x.LessonId == lessonId)
                                     .ToListAsync();
            if (olds.Count > 0)
            {
                _context.RemoveRange(olds);
                await _context.SaveChangesAsync();
            }

            int pos = 1;
            var news = questionIds.Select(q => new LessonEnhancedQuestion
            {
                LessonId = lessonId,
                QuestionId = q,
                Position = pos++
            }).ToList();

            if (news.Count > 0)
            {
                await _context.AddRangeAsync(news);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<LessonEnhanced>> GetByQuestionIdAsync(int questionId)
        {
            var lessonIds = await _context.LessonsEnhancedQuestions
                .Where(x => x.QuestionId == questionId)
                .Select(x => x.LessonId)
                .Distinct()
                .ToListAsync();

            if (lessonIds.Count == 0) return new();

            return await _context.LessonsEnhanced
                .Where(l => lessonIds.Contains(l.Id))
                .ToListAsync();
        }
    }
}

