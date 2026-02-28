using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class LessonRepository : GenericRepository<Lesson>
    {

        public LessonRepository(teamsevenezexamdbContext context) : base(context) { }

        public async Task<List<Lesson>?> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<Lesson?> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<List<Lesson>?> GetByChapterIdAsync(int chapterId)
        {
            return await _context.Lessons
                .Where(l => l.ChapterId == chapterId)
                .ToListAsync();
        }

        public async Task<int> AddAsync(Lesson lesson)
        {
            return await CreateAsync(lesson);
        }

        public async Task<int> UpdateAsync(Lesson lesson)
        {
            return await base.UpdateAsync(lesson);
        }

        public async Task<bool> DeleteAsync(Lesson lesson)
        {
            return await RemoveAsync(lesson);
        }

        public async Task<(List<Lesson>, int)> GetPagedAsync(
             int pageNumber,
             int pageSize,
             string? search = null,
             string? sort = null,
             int? chapterId = null,
             int isSort = 0)
        {
            var query = _context.Lessons.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var searchNormalized = search.RemoveDiacritics().ToLower();
                query = query.Where(l => l.Name.RemoveDiacritics().ToLower().Contains(searchNormalized));
            }

            if (chapterId.HasValue)
            {
                query = query.Where(l => l.ChapterId == chapterId.Value);
            }

            if (isSort == 1)
            {
                if (!string.IsNullOrEmpty(sort))
                {
                    switch (sort.ToLower())
                    {
                        case "name:asc":
                            query = query.OrderBy(l => l.Name);
                            break;
                        case "name:desc":
                            query = query.OrderByDescending(l => l.Name);
                            break;
                        case "createdat:asc":
                            query = query.OrderBy(l => l.CreatedAt);
                            break;
                        case "createdat:desc":
                            query = query.OrderByDescending(l => l.CreatedAt);
                            break;
                        case "updatedat:asc":
                            query = query.OrderBy(l => l.UpdatedAt);
                            break;
                        case "updatedat:desc":
                            query = query.OrderByDescending(l => l.UpdatedAt);
                            break;
                        default:
                            query = query.OrderByDescending(l => l.CreatedAt);
                            break;
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

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }
    }
}

