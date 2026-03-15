using AutoMapper;
using Microsoft.Extensions.Logging;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Interfaces;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.SolutionService
{
    public class SolutionService : ISolutionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SolutionService> _logger;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        /// <summary>Solutions are "lazy cached" — loaded on demand when a student opens an exam.</summary>
        private static readonly TimeSpan SolutionTtl = TimeSpan.FromMinutes(30);

        public SolutionService(
            IUnitOfWork unitOfWork,
            ILogger<SolutionService> logger,
            IMapper mapper,
            ICacheService? cache = null)
        {
            _unitOfWork = unitOfWork;
            _logger     = logger;
            _mapper     = mapper;
            _cache      = cache ?? NullCacheService.Instance;
        }

        // ── Read ───────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<SolutionDataResponse>> GetAllSolutionsAsync()
        {
            // Bulk admin view — skip cache (rarely used, data too large/volatile)
            var solutions = await _unitOfWork.SolutionRepository.GetAllAsync();
            return solutions.Select(MapResponse);
        }

        public async Task<SolutionDataResponse> GetSolutionByIdAsync(int id)
        {
            // Lazy cache: populated the first time a student requests this solution
            var cacheKey = $"solutions:{id}";
            var cached = await _cache.GetAsync<SolutionDataResponse>(cacheKey);
            if (cached is not null) return cached;

            var s = await _unitOfWork.SolutionRepository.GetByIdAsync(id);
            if (s == null) throw new NotFoundException($"Solution with ID {id} not found.");

            var result = MapResponse(s);
            await _cache.SetAsync(cacheKey, result, SolutionTtl);
            return result;
        }

        // ── Write ──────────────────────────────────────────────────────────────────

        public async Task CreateSolutionAsync(CreateSolutionRequest request)
        {
            var solution = new Solution
            {
                QuestionId      = request.QuestionId,
                Content         = request.Content,
                Explanation     = request.Explanation,
                CreatedByUserId = request.CreatedByUserId,
                CreatedAt       = DateTime.UtcNow
            };

            await _unitOfWork.SolutionRepository.CreateAsync(solution);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task UpdateSolutionAsync(SolutionDataRequest request)
        {
            var solution = await _unitOfWork.SolutionRepository.GetByIdAsync(request.Id);
            if (solution == null) throw new NotFoundException("Solution not found.");

            solution.Content    = request.Content;
            solution.Explanation = request.Explanation;
            solution.UpdatedAt  = DateTime.UtcNow;

            await _unitOfWork.SolutionRepository.UpdateAsync(solution);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            // Invalidate the cached version
            await _cache.RemoveAsync($"solutions:{request.Id}");
        }

        public async Task DeleteSolutionAsync(int id)
        {
            var solution = await _unitOfWork.SolutionRepository.GetByIdAsync(id);
            if (solution == null) throw new NotFoundException("Solution not found.");

            await _unitOfWork.SolutionRepository.RemoveAsync(solution);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            await _cache.RemoveAsync($"solutions:{id}");
        }

        public async Task AddSolutionWithVideoAsync(SolutionWithVideoRequest request)
        {
            byte[]? videoBytes  = null;
            string? contentType = null;

            if (request.VideoFile != null)
            {
                using var ms = new MemoryStream();
                await request.VideoFile.CopyToAsync(ms);
                videoBytes  = ms.ToArray();
                contentType = request.VideoFile.ContentType;
            }

            var solution = new Solution
            {
                QuestionId      = request.QuestionId ?? -1,
                Content         = request.Content,
                CreatedByUserId = 1,
                CreatedAt       = DateTime.UtcNow,
                VideoData        = videoBytes,
                VideoContentType = contentType
            };

            await _unitOfWork.SolutionRepository.AddAsync(solution);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        // ── Helpers ────────────────────────────────────────────────────────────────

        private static SolutionDataResponse MapResponse(Solution s) => new()
        {
            Id              = s.Id,
            QuestionId      = s.QuestionId,
            Content         = s.Content,
            Explanation     = s.Explanation,
            CreatedByUserId = s.CreatedByUserId,
            IsApproved      = s.IsApproved,
            CreatedAt       = s.CreatedAt,
            UpdatedAt       = s.UpdatedAt,
            VideoData        = s.VideoData,
            VideoContentType = s.VideoContentType
        };
    }
}
