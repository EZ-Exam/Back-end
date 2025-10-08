using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Repository
{
    public class ExamRepository : GenericRepository<Exam>
    {
        private readonly teamsevenezexamdbContext _context;

        public ExamRepository(teamsevenezexamdbContext context)
        {
            _context = context;
        }

        public async Task<List<Exam>?> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<Exam?> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<List<Exam>?> GetByLessonIdAsync(int lessonId)
        {
            return await _context.Exams
                .Where(e => e.LessonId == lessonId && !e.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Exam>?> GetByCreatorAsync(long userId)
        {
            return await _context.Exams
                .Where(e => e.CreatedByUserId == userId)
                .ToListAsync();
        }

        public async Task<int> AddAsync(Exam exam)
        {
            return await CreateAsync(exam);
        }

        public async Task<int> UpdateAsync(Exam exam)
        {
            return await base.UpdateAsync(exam);
        }

        public async Task<bool> DeleteAsync(Exam exam)
        {
            return await RemoveAsync(exam);
        }

        // Repository/ExamRepository.cs
        public async Task<(List<Exam> Items, int Total)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search = null,
            string? sort = null,         // name|createdAt|updatedAt|totalQuestions|timeLimit : asc|desc
            int? subjectId = null,
            int? lessonId = null,
            int? examTypeId = null,
            int? createdByUserId = null,
            int isSort = 0
        )
        {
            var q = _context.Exams.AsNoTracking().AsQueryable();

            // ------- filters -------
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                q = q.Where(x =>
                    (x.Name != null && x.Name.ToLower().Contains(s)) ||
                    (x.Description != null && x.Description.ToLower().Contains(s)));
            }

            if (subjectId.HasValue) q = q.Where(x => x.SubjectId == subjectId.Value);
            if (lessonId.HasValue) q = q.Where(x => x.LessonId == lessonId.Value);
            if (examTypeId.HasValue) q = q.Where(x => x.ExamTypeId == examTypeId.Value);
            if (createdByUserId.HasValue) q = q.Where(x => x.CreatedByUserId == createdByUserId.Value);

            // ------- sort -------
            if (isSort == 1 && !string.IsNullOrWhiteSpace(sort))
            {
                switch (sort.ToLower())
                {
                    case "name:asc": q = q.OrderBy(x => x.Name); break;
                    case "name:desc": q = q.OrderByDescending(x => x.Name); break;
                    case "createdat:asc": q = q.OrderBy(x => x.CreatedAt); break;
                    case "createdat:desc": q = q.OrderByDescending(x => x.CreatedAt); break;
                    case "updatedat:asc": q = q.OrderBy(x => x.UpdatedAt); break;
                    case "updatedat:desc": q = q.OrderByDescending(x => x.UpdatedAt); break;
                    case "totalquestions:asc": q = q.OrderBy(x => x.TotalQuestions); break;
                    case "totalquestions:desc": q = q.OrderByDescending(x => x.TotalQuestions); break;
                    case "timelimit:asc": q = q.OrderBy(x => x.TimeLimit); break;
                    case "timelimit:desc": q = q.OrderByDescending(x => x.TimeLimit); break;
                    default: q = q.OrderBy(x => x.Id); break;
                }
            }
            else
            {
                q = q.OrderBy(x => x.Id);
            }

            // ------- paging -------
            var total = await q.CountAsync();
            var items = await q.Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

            return (items, total);
        }

    }
}
