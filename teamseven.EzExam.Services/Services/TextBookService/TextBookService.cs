using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Interfaces;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.TextBookService
{
    public class TextBookService : ITextBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TextBookService> _logger;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        private const string CacheKeyAll = "textbooks:all";
        private static readonly TimeSpan Ttl = TimeSpan.FromHours(24);

        public TextBookService(
            IUnitOfWork unitOfWork,
            ILogger<TextBookService> logger,
            IMapper mapper,
            ICacheService? cache = null)
        {
            _unitOfWork = unitOfWork;
            _logger     = logger;
            _mapper     = mapper;
            _cache      = cache ?? NullCacheService.Instance;
        }

        // ──────────────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<TextBookDataResponse>> GetAllTextBookAsync()
        {
            var cached = await _cache.GetAsync<List<TextBookDataResponse>>(CacheKeyAll);
            if (cached is not null) return cached;

            // ProjectTo: EF Core generates SELECT only the columns needed by the DTO
            var result = await _unitOfWork.Context.TextBooks
                .AsNoTracking()
                .OrderBy(x => x.GradeId).ThenBy(x => x.Name)
                .ProjectTo<TextBookDataResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            await _cache.SetAsync(CacheKeyAll, result, Ttl);
            return result;
        }

        public async Task<List<TextBookDataResponse>> GetAsync(int? gradeId = null, int? subjectId = null)
        {
            var key = $"textbooks:g{gradeId}:s{subjectId}";
            var cached = await _cache.GetAsync<List<TextBookDataResponse>>(key);
            if (cached is not null) return cached;

            var query = _unitOfWork.Context.TextBooks.AsNoTracking().AsQueryable();
            if (gradeId.HasValue)   query = query.Where(x => x.GradeId == gradeId.Value);
            if (subjectId.HasValue) query = query.Where(x => x.SubjectId == subjectId.Value);

            var result = await query
                .OrderBy(x => x.GradeId).ThenBy(x => x.Name)
                .ProjectTo<TextBookDataResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            await _cache.SetAsync(key, result, Ttl);
            return result;
        }

        public async Task<TextBookDataResponse> GetTextBookByIdAsync(int id)
        {
            var key = $"textbooks:{id}";
            var cached = await _cache.GetAsync<TextBookDataResponse>(key);
            if (cached is not null) return cached;

            var tb = await _unitOfWork.TextBookRepository.GetByIdAsync(id);
            if (tb == null) throw new NotFoundException($"Textbook with ID {id} not found.");

            var result = _mapper.Map<TextBookDataResponse>(tb);
            await _cache.SetAsync(key, result, Ttl);
            return result;
        }

        public async Task CreateTextBookAsync(CreateTextBookRequest request)
        {
            var textbook = _mapper.Map<TextBook>(request);
            textbook.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.TextBookRepository.CreateAsync(textbook);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            await InvalidateCacheAsync();
        }

        public async Task UpdateTextBookAsync(TextBookDataRequest request)
        {
            var existing = await _unitOfWork.TextBookRepository.GetByIdAsync(request.Id);
            if (existing == null)
                throw new NotFoundException($"Textbook with ID {request.Id} not found.");

            existing.Name    = request.Name;
            existing.GradeId = request.GradeId;
            existing.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.TextBookRepository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            await InvalidateCacheAsync();
            await _cache.RemoveAsync($"textbooks:{request.Id}");
        }

        public async Task DeleteTextBookAsync(int id)
        {
            var textbook = await _unitOfWork.TextBookRepository.GetByIdAsync(id);
            if (textbook == null) throw new NotFoundException($"Textbook with ID {id} not found.");

            await _unitOfWork.TextBookRepository.RemoveAsync(textbook);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            await InvalidateCacheAsync();
            await _cache.RemoveAsync($"textbooks:{id}");
        }

        // ── helpers ────────────────────────────────────────────────────────────────

        private async Task InvalidateCacheAsync()
        {
            await _cache.RemoveAsync(CacheKeyAll);
            await _cache.RemoveByPrefixAsync("textbooks:");
        }
    }
}
