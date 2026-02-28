using AutoMapper;
using Microsoft.Extensions.Logging;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.SubscriptionTypeService
{
    public class SubscriptionTypeService : ISubscriptionTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SubscriptionTypeService> _logger;
        private readonly IMapper _mapper;

        public SubscriptionTypeService(IUnitOfWork unitOfWork, ILogger<SubscriptionTypeService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<SubscriptionTypeResponse> GetSubscriptionTypeByIdAsync(int id)
        {
            try
            {
                var subscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(id);
                if (subscriptionType == null)
                    throw new NotFoundException($"Subscription type with ID {id} not found.");

                return _mapper.Map<SubscriptionTypeResponse>(subscriptionType);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription type by ID {Id}: {Message}", id, ex.Message);
                throw new ApplicationException("An error occurred while retrieving subscription type.", ex);
            }
        }

        public async Task<IEnumerable<SubscriptionTypeResponse>> GetAllSubscriptionTypesAsync()
        {
            try
            {
                var subscriptionTypes = await _unitOfWork.SubscriptionTypeRepository.GetAllAsync();
                return subscriptionTypes.Select(t => _mapper.Map<SubscriptionTypeResponse>(t));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subscription types: {Message}", ex.Message);
                throw new ApplicationException("An error occurred while retrieving subscription types.", ex);
            }
        }

        public async Task<SubscriptionTypeResponse> GetSubscriptionTypeByCodeAsync(string subscriptionCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subscriptionCode))
                    throw new ArgumentException("Subscription code cannot be null or empty.", nameof(subscriptionCode));

                var subscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByCodeAsync(subscriptionCode);
                if (subscriptionType == null)
                    throw new NotFoundException($"Subscription type with code '{subscriptionCode}' not found.");

                return _mapper.Map<SubscriptionTypeResponse>(subscriptionType);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription type by code {Code}: {Message}", subscriptionCode, ex.Message);
                throw new ApplicationException("An error occurred while retrieving subscription type.", ex);
            }
        }

        public async Task<SubscriptionTypeResponse> CreateSubscriptionTypeAsync(SubscriptionTypeRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("Create subscription type request is null.");
                throw new ArgumentNullException(nameof(request), "Create subscription type request cannot be null.");
            }

            try
            {
                var existingType = await _unitOfWork.SubscriptionTypeRepository.GetByCodeAsync(request.SubscriptionCode);
                var codeExists = existingType != null;
                if (codeExists)
                    throw new InvalidOperationException($"Subscription code '{request.SubscriptionCode}' already exists.");

                var subscriptionType = _mapper.Map<SubscriptionType>(request);
                subscriptionType.CreatedAt = DateTime.UtcNow;
                subscriptionType.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SubscriptionTypeRepository.AddAsync(subscriptionType);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("Subscription type {Code} created successfully", request.SubscriptionCode);

                return _mapper.Map<SubscriptionTypeResponse>(subscriptionType);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription type: {Message}", ex.Message);
                throw new ApplicationException("An error occurred while creating subscription type.", ex);
            }
        }

        public async Task<SubscriptionTypeResponse> UpdateSubscriptionTypeAsync(int id, SubscriptionTypeRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("Update subscription type request is null.");
                throw new ArgumentNullException(nameof(request), "Update subscription type request cannot be null.");
            }

            try
            {
                var existingSubscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(id);
                if (existingSubscriptionType == null)
                    throw new NotFoundException($"Subscription type with ID {id} not found.");

                var existingType = await _unitOfWork.SubscriptionTypeRepository.GetByCodeAsync(request.SubscriptionCode);
                var codeExists = existingType != null && existingType.Id != id;
                if (codeExists)
                    throw new InvalidOperationException($"Subscription code '{request.SubscriptionCode}' already exists.");

                _mapper.Map(request, existingSubscriptionType);
                existingSubscriptionType.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SubscriptionTypeRepository.UpdateAsync(existingSubscriptionType);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("Subscription type {Id} updated successfully", id);

                return _mapper.Map<SubscriptionTypeResponse>(existingSubscriptionType);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription type {Id}: {Message}", id, ex.Message);
                throw new ApplicationException("An error occurred while updating subscription type.", ex);
            }
        }

        public async Task DeleteSubscriptionTypeAsync(int id)
        {
            var subscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Subscription type with ID {id} not found.");

            await _unitOfWork.SubscriptionTypeRepository.DeleteAsync(subscriptionType);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            _logger.LogInformation("Subscription type {Id} deleted successfully", id);
        }

        public async Task<IEnumerable<SubscriptionTypeResponse>> GetActiveSubscriptionTypesAsync()
        {
            try
            {
                var activeSubscriptionTypes = await _unitOfWork.SubscriptionTypeRepository.GetActiveSubscriptionTypesAsync();
                return activeSubscriptionTypes.Select(t => _mapper.Map<SubscriptionTypeResponse>(t));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active subscription types: {Message}", ex.Message);
                throw new ApplicationException("An error occurred while retrieving active subscription types.", ex);
            }
        }

        public async Task<bool> IsSubscriptionCodeExistsAsync(string subscriptionCode, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subscriptionCode))
                    throw new ArgumentException("Subscription code cannot be null or empty.", nameof(subscriptionCode));

                var existingType = await _unitOfWork.SubscriptionTypeRepository.GetByCodeAsync(subscriptionCode);
                if (existingType == null) return false;
                if (excludeId.HasValue && existingType.Id == excludeId.Value) return false;
                return true;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if subscription code exists {Code}: {Message}", subscriptionCode, ex.Message);
                throw new ApplicationException("An error occurred while checking subscription code existence.", ex);
            }
        }

        public async Task ActivateSubscriptionTypeAsync(int id)
        {
            var subscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Subscription type with ID {id} not found.");

            subscriptionType.IsActive = true;
            subscriptionType.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SubscriptionTypeRepository.UpdateAsync(subscriptionType);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            _logger.LogInformation("Subscription type {Id} activated successfully", id);
        }

        public async Task DeactivateSubscriptionTypeAsync(int id)
        {
            var subscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Subscription type with ID {id} not found.");

            subscriptionType.IsActive = false;
            subscriptionType.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SubscriptionTypeRepository.UpdateAsync(subscriptionType);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            _logger.LogInformation("Subscription type {Id} deactivated successfully", id);
        }

    }
}