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
        private readonly teamsevenezexamdbContext _context;

        public LessonEnhancedRepository(teamsevenezexamdbContext context)
        {
            _context = context;
        }

        // ===== CRUD cơ bản (giống mẫu) =====
        public async Task<List<LessonEnhanced>?> GetAllAsync()
        {
            return await base.GetAllAsync();
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
            await _context.SaveChangesAsync();   // EF sẽ gán Id tự tăng vào entity.Id
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

        // ===== Paged/Search/Sort theo Title & SubjectId =====
        public async Task<(List<LessonEnhanced>, int)> GetPagedAsync(
             int pageNumber,
             int pageSize,
             string? search = null,
             string? sort = null,
             int? subjectId = null,
             int isSort = 0) // 0: mặc định theo Id; 1: áp dụng sort tham số
        {
            var query = _context.LessonsEnhanced.AsQueryable();

            // Search (accent-insensitive) theo Title
            if (!string.IsNullOrEmpty(search))
            {
                var s = search.RemoveDiacritics().ToLower();
                query = query.Where(l => (l.Title ?? string.Empty).RemoveDiacritics().ToLower().Contains(s));
            }

            // Filter theo SubjectId
            if (subjectId.HasValue)
            {
                query = query.Where(l => l.SubjectId == subjectId.Value);
            }

            // Sort
            if (isSort == 1)
            {
                if (!string.IsNullOrEmpty(sort))
                {
                    switch (sort.ToLower())
                    {
                        case "title:asc": query = query.OrderBy(l => l.Title); break;
                        case "title:desc": query = query.OrderByDescending(l => l.Title); break;
                        case "createdat:asc": query = query.OrderBy(l => l.CreatedAt); break;
                        case "createdat:desc": query = query.OrderByDescending(l => l.CreatedAt); break;
                        case "updatedat:asc": query = query.OrderBy(l => l.UpdatedAt); break;
                        case "updatedat:desc": query = query.OrderByDescending(l => l.UpdatedAt); break;
                        default: query = query.OrderByDescending(l => l.CreatedAt); break;
                    }
                }
                else
                {
                    query = query.OrderByDescending(l => l.CreatedAt);
                }
            }
            else
            {
                query = query.OrderBy(l => l.Id);
            }

            // Pagination
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        // ===== Bảng nối: helpers =====

        /// <summary>Lấy list QuestionId theo đúng thứ tự Position</summary>
        public async Task<List<int>> GetQuestionIdsAsync(int lessonId)
        {
            return await _context.LessonsEnhancedQuestions
                .Where(x => x.LessonId == lessonId)
                .OrderBy(x => x.Position)
                .Select(x => x.QuestionId)
                .ToListAsync();
        }
        /// (B1) Lọc chỉ giữ các QuestionId có tồn tại trong bảng questions
        public async Task<List<int>> FilterExistingQuestionIdsAsync(IEnumerable<int> ids)
        {
            var set = ids?.Distinct().ToList() ?? new List<int>();
            if (set.Count == 0) return new List<int>();

            return await _context.Questions
                .Where(q => set.Contains(q.Id))
                .Select(q => q.Id)
                .ToListAsync();
        }

        /// (B2) Chỉ thêm mới (append) các QuestionId chưa có, gán Position tiếp theo
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
        /// <summary>Thay toàn bộ danh sách question theo thứ tự (Position = index + 1)</summary>
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

        /// <summary>Trả về các lesson có chứa questionId</summary>
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
