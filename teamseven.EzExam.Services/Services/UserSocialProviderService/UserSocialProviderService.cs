using Microsoft.Extensions.Logging;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.UserSocialProviderService
{
    public class UserSocialProviderService : IUserSocialProviderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserSocialProviderService> _logger;

        public UserSocialProviderService(IUnitOfWork unitOfWork, ILogger<UserSocialProviderService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<UserSocialProviderDataResponse>> GetAllAsync()
        {
            var items = await _unitOfWork.UserSocialProviderRepository.GetAllAsync();
            return items.Select(x => new UserSocialProviderDataResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                ProviderName = x.ProviderName,
                ProviderId = x.ProviderId,
                Email = x.Email,
                ProfileUrl = x.ProfileUrl,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            });
        }

        public async Task<UserSocialProviderDataResponse> GetByIdAsync(int id)
        {
            var item = await _unitOfWork.UserSocialProviderRepository.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException($"UserSocialProvider with ID {id} not found.");

            return new UserSocialProviderDataResponse
            {
                Id = item.Id,
                UserId = item.UserId,
                ProviderName = item.ProviderName,
                ProviderId = item.ProviderId,
                Email = item.Email,
                ProfileUrl = item.ProfileUrl,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };
        }

        public async Task CreateAsync(CreateUserSocialProviderRequest request)
        {
            var entity = new UserSocialProvider
            {
                UserId = request.UserId,
                ProviderName = request.ProviderName,
                ProviderId = request.ProviderId,
                Email = request.Email,
                ProfileUrl = request.ProfileUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserSocialProviderRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task UpdateAsync(UserSocialProviderDataRequest request)
        {
            var entity = await _unitOfWork.UserSocialProviderRepository.GetByIdAsync(request.Id);
            if (entity == null)
                throw new NotFoundException("UserSocialProvider not found.");

            entity.Email = request.Email;
            entity.ProfileUrl = request.ProfileUrl;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.UserSocialProviderRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.UserSocialProviderRepository.GetByIdAsync(id);
            if (entity == null)
                throw new NotFoundException("UserSocialProvider not found.");

            await _unitOfWork.UserSocialProviderRepository.RemoveAsync(entity);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }
    }
}
