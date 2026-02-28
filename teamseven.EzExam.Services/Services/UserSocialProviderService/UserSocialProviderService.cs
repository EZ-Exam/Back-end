using Microsoft.Extensions.Logging;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

using AutoMapper;

namespace teamseven.EzExam.Services.Services.UserSocialProviderService
{
    public class UserSocialProviderService : IUserSocialProviderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserSocialProviderService> _logger;
        private readonly IMapper _mapper;

        public UserSocialProviderService(IUnitOfWork unitOfWork, ILogger<UserSocialProviderService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserSocialProviderDataResponse>> GetAllAsync()
        {
            var items = await _unitOfWork.UserSocialProviderRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserSocialProviderDataResponse>>(items);
        }

        public async Task<UserSocialProviderDataResponse> GetByIdAsync(int id)
        {
            var item = await _unitOfWork.UserSocialProviderRepository.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException($"UserSocialProvider with ID {id} not found.");

            return _mapper.Map<UserSocialProviderDataResponse>(item);
        }

        public async Task CreateAsync(CreateUserSocialProviderRequest request)
        {
            var entity = _mapper.Map<UserSocialProvider>(request);
            entity.CreatedAt = DateTime.UtcNow;

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
