using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository.Context;
namespace teamseven.EzExam.Repository.Repository
{
    public class QuestionRepository : GenericRepository<Question>
    {
        private readonly teamsevenezexamdbContext _context;

        public QuestionRepository(teamsevenezexamdbContext context)
        {
            _context = context;
        }

        public async Task<List<Question>?> GetAllAsync()
        {
            return await _context.Questions
                .Include(q => q.Lesson)
                .ToListAsync();
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<List<Question>?> GetByLessonIdAsync(int lessonId)
        {
            return await _context.Questions
                .Where(q => q.LessonId == lessonId)
                .ToListAsync();
        }

        public async Task<List<Question>?> GetByCreatorAsync(long userId)
        {
            return await _context.Questions
                .Where(q => q.CreatedByUserId == userId)
                .ToListAsync();
        }

        public async Task<List<Question>?> GetBySubjectIdAsync(int subjectId)
        {
            return await _context.Questions
                .Where(q => q.SubjectId == subjectId)
                .ToListAsync();
        }

        public async Task<int> AddAsync(Question question)
        {
            return await CreateAsync(question);
        }

        public async Task<int> UpdateAsync(Question question)
        {
            return await base.UpdateAsync(question);
        }

        public async Task<bool> DeleteAsync(Question question)
        {
            return await RemoveAsync(question);
        }

        public async Task<(List<Question>, int)> GetPagedAsync(
    int pageNumber,
    int pageSize,
    string? search = null,
    string? sort = null,
    int? lessonId = null,
    string? difficultyLevel = null,
    int? chapterId = null,
    int isSort = 0,
    int createdByUserId = 0,
    int? textbookId = null) // Default = 0 => kh�ng l?c theo user
        {
            var query = _context.Questions
                .Include(q => q.Lesson)
                .AsQueryable();

            // ?? L?c theo ngu?i t?o (n?u c�)
            if (createdByUserId != 0)
            {
                query = query.Where(q => q.CreatedByUserId == createdByUserId);
            }

            // ?? Search (b? d?u)
            if (!string.IsNullOrEmpty(search))
            {
                var searchNormalized = search.RemoveDiacritics().ToLower();
                query = query.Where(q =>
                    q.Content.RemoveDiacritics().ToLower().Contains(searchNormalized) ||
                    q.QuestionSource.RemoveDiacritics().ToLower().Contains(searchNormalized));
            }

            if (lessonId.HasValue)
            {
                query = query.Where(q => q.LessonId == lessonId.Value);
            }

            if (!string.IsNullOrEmpty(difficultyLevel))
            {
                query = query.Where(q => q.DifficultyLevel.Name == difficultyLevel || q.DifficultyLevel.Code == difficultyLevel);
            }

            if (chapterId.HasValue)
            {
                query = query.Where(q => q.Lesson.ChapterId == chapterId.Value);
            }
            // NEW: filter theo textbookId
            if (textbookId.HasValue)
                query = query.Where(q => q.TextbookId == textbookId.Value);


            // ?? S?p x?p
            if (isSort == 1)
            {
                if (!string.IsNullOrEmpty(sort))
                {
                    switch (sort.ToLower())
                    {
                        case "content:asc":
                            query = query.OrderBy(q => q.Content);
                            break;
                        case "content:desc":
                            query = query.OrderByDescending(q => q.Content);
                            break;
                        case "difficultylevel:asc":
                            query = query.OrderBy(q => q.DifficultyLevel);
                            break;
                        case "difficultylevel:desc":
                            query = query.OrderByDescending(q => q.DifficultyLevel);
                            break;
                        case "createdat:asc":
                            query = query.OrderBy(q => q.CreatedAt);
                            break;
                        case "createdat:desc":
                            query = query.OrderByDescending(q => q.CreatedAt);
                            break;
                        case "updatedat:asc":
                            query = query.OrderBy(q => q.UpdatedAt);
                            break;
                        case "updatedat:desc":
                            query = query.OrderByDescending(q => q.UpdatedAt);
                            break;
                        default:
                            query = query.OrderByDescending(q => q.CreatedAt);
                            break;
                    }
                }
                else
                {
                    query = query.OrderByDescending(q => q.CreatedAt);
                }
            }
            else
            {
                query = query.OrderBy(q => q.Id);
            }

            // ?? Pagination
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }
    }
    }
