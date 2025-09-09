using Microsoft.Extensions.Logging;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Extensions;

namespace teamseven.EzExam.Services.Services.SolutionService
{
    public class SolutionService : ISolutionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SolutionService> _logger;

        public SolutionService(IUnitOfWork unitOfWork, ILogger<SolutionService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<SolutionDataResponse>> GetAllSolutionsAsync()
        {
            var solutions = await _unitOfWork.SolutionRepository.GetAllAsync();
            return solutions.Select(s => new SolutionDataResponse
            {
                Id = s.Id,
                QuestionId = s.QuestionId,
                Content = s.Content,
                Explanation = s.Explanation,
                CreatedByUserId = s.CreatedByUserId,
                IsApproved = s.IsApproved,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            });
        }

        public async Task<SolutionDataResponse> GetSolutionByIdAsync(int id)
        {
            var solution = await _unitOfWork.SolutionRepository.GetByIdAsync(id);
            if (solution == null)
                throw new NotFoundException($"Solution with ID {id} not found.");

            return new SolutionDataResponse
            {
                Id = solution.Id,
                QuestionId = solution.QuestionId,
                Content = solution.Content,
                Explanation = solution.Explanation,
                CreatedByUserId = solution.CreatedByUserId,
                IsApproved = solution.IsApproved,
                CreatedAt = solution.CreatedAt,
                UpdatedAt = solution.UpdatedAt,
                VideoData = solution.VideoData,
                VideoContentType = solution.VideoContentType
            };
        }

        public async Task CreateSolutionAsync(CreateSolutionRequest request)
        {
            var solution = new Solution
            {
                QuestionId = request.QuestionId,
                Content = request.Content,
                Explanation = request.Explanation,
                CreatedByUserId = request.CreatedByUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.SolutionRepository.CreateAsync(solution);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task UpdateSolutionAsync(SolutionDataRequest request)
        {
            var solution = await _unitOfWork.SolutionRepository.GetByIdAsync(request.Id);
            if (solution == null)
                throw new NotFoundException("Solution not found.");

            solution.Content = request.Content;
            solution.Explanation = request.Explanation;
            solution.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SolutionRepository.UpdateAsync(solution);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task DeleteSolutionAsync(int id)
        {
            var solution = await _unitOfWork.SolutionRepository.GetByIdAsync(id);
            if (solution == null)
                throw new NotFoundException("Solution not found.");

            await _unitOfWork.SolutionRepository.RemoveAsync(solution);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }
        public async Task AddSolutionWithVideoAsync(SolutionWithVideoRequest request)
        {
            byte[] videoBytes = null;
            string contentType = null;

            if (request.VideoFile != null)
            {
                using var ms = new MemoryStream();
                await request.VideoFile.CopyToAsync(ms);
                videoBytes = ms.ToArray();
                contentType = request.VideoFile.ContentType; // ví d?: "video/mp4"
            }

            var solution = new Solution
            {
                QuestionId = request.QuestionId ?? -1,
                Content = request.Content,
                CreatedByUserId = 1, 
                CreatedAt = DateTime.UtcNow,
                VideoData = videoBytes,
                VideoContentType = contentType
            };

            await _unitOfWork.SolutionRepository.AddAsync(solution);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

    }
}
