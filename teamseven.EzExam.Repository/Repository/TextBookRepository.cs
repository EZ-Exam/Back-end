using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository.Context;

namespace teamseven.EzExam.Repository.Repository
{
    public class TextBookRepository : GenericRepository<TextBook>
    {
        private readonly teamsevenezexamdbContext _context;

        public TextBookRepository(teamsevenezexamdbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<TextBook>> GetAllTextBooksAsync()
        {
            return await _context.TextBooks.ToListAsync();
        }

        public async Task<TextBook?> GetTextBookByIdAsync(int id)
        {
            return await _context.TextBooks.FindAsync(id);
        }

        public async Task CreateTextBookAsync(TextBook textBook)
        {
            if (textBook == null)
            {
                throw new ArgumentNullException(nameof(textBook));
            }

            _context.TextBooks.Add(textBook);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTextBookAsync(TextBook textBook)
        {
            if (textBook == null)
            {
                throw new ArgumentNullException(nameof(textBook));
            }

            _context.TextBooks.Update(textBook);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTextBookAsync(int id)
        {
            var textBook = await GetTextBookByIdAsync(id);
            if (textBook == null)
            {
                throw new KeyNotFoundException($"TextBook with ID {id} not found.");
            }

            _context.TextBooks.Remove(textBook);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TextBook>> GetAllAsync(int? gradeId = null, int? subjectId = null)
        {
            var q = _context.TextBooks.AsNoTracking().AsQueryable();
            if (gradeId.HasValue) q = q.Where(x => x.GradeId == gradeId.Value);
            if (subjectId.HasValue) q = q.Where(x => x.SubjectId == subjectId.Value);
            return await q.OrderBy(x => x.GradeId).ThenBy(x => x.Name).ToListAsync();
        }
    }
}
