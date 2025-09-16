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

        public SubscriptionTypeService(IUnitOfWork unitOfWork, ILogger<SubscriptionTypeService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<SubscriptionTypeResponse>> GetAllSubscriptionTypesAsync()
        {
            try
            {
                var subscriptionTypes = await _unitOfWork.SubscriptionTypeRepository.GetAllAsync();
                return subscriptionTypes.Select(MapToResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all subscription types: {Message}", ex.Message);
                throw new ApplicationException("An error occurred while retrieving subscription types.", ex);
            }
        }

        public async Task<IEnumerable<SubscriptionTypeResponse>> GetActiveSubscriptionTypesAsync()
        {
            try
            {
                var subscriptionTypes = await _unitOfWork.SubscriptionTypeRepository.GetActiveSubscriptionTypesAsync();
                return subscriptionTypes.Select(MapToResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription types: {Message}", ex.Message);
                throw new ApplicationException("An error occurred while retrieving active subscription types.", ex);
            }
        }

        public async Task<SubscriptionTypeResponse> GetSubscriptionTypeByIdAsync(int id)
        {
            try
            {
                var subscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(id);
                if (subscriptionType == null)
                    throw new NotFoundException($"Subscription type with ID {id} not found.");

                return MapToResponse(subscriptionType);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription type with ID {Id}: {Message}", id, ex.Message);
                throw new ApplicationException("An error occurred while retrieving the subscription type.", ex);
            }
        }

        public async Task<SubscriptionTypeResponse> GetSubscriptionTypeByCodeAsync(string subscriptionCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subscriptionCode))
                    throw new ArgumentException("Subscription code cannot be null or empty.", nameof(subscriptionCode));

                var subscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetBySubscriptionCodeAsync(subscriptionCode);
                if (subscriptionType == null)
                    throw new NotFoundException($"Subscription type with code '{subscriptionCode}' not found.");

                return MapToResponse(subscriptionType);
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
                _logger.LogError(ex, "Error retrieving subscription type with code '{Code}': {Message}", subscriptionCode, ex.Message);
                throw new ApplicationException("An error occurred while retrieving the subscription type.", ex);
            }
        }

        public async Task<SubscriptionTypeResponse> CreateSubscriptionTypeAsync(SubscriptionTypeRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("Create subscription type request is null.");
                throw new ArgumentNullException(nameof(request), "Subscription type creation request cannot be null.");
            }

            try
            {
                // Check if subscription code already exists
                var codeExists = await _unitOfWork.SubscriptionTypeRepository.IsSubscriptionCodeExistsAsync(request.SubscriptionCode);
                if (codeExists)
                    throw new InvalidOperationException($"Subscription code '{request.SubscriptionCode}' already exists.");

                var subscriptionType = new SubscriptionType
                {
                    SubscriptionCode = request.SubscriptionCode,
                    SubscriptionName = request.SubscriptionName,
                    SubscriptionPrice = request.SubscriptionPrice,
                    Description = request.Description,
                    MaxSolutionViews = request.MaxSolutionViews,
                    MaxAIRequests = request.MaxAIRequests,
                    IsAIEnabled = request.IsAIEnabled,
                    Features = request.Features,
                    IsActive = request.IsActive,
                    UpdatedBy = request.UpdatedBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.SubscriptionTypeRepository.CreateAsync(subscriptionType);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("Created subscription type with ID {Id} and code '{Code}'.", 
                    subscriptionType.Id, subscriptionType.SubscriptionCode);

                return MapToResponse(subscriptionType);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription type: {Message}", ex.Message);
                throw new ApplicationException("An error occurred while creating the subscription type.", ex);
            }
        }

        public async Task<SubscriptionTypeResponse> UpdateSubscriptionTypeAsync(int id, SubscriptionTypeRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("Update subscription type request is null.");
                throw new ArgumentNullException(nameof(request), "Subscription type update request cannot be null.");
            }

            try
            {
                var existingSubscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(id);
                if (existingSubscriptionType == null)
                    throw new NotFoundException($"Subscription type with ID {id} not found.");

                // Check if subscription code already exists (excluding current record)
                var codeExists = await _unitOfWork.SubscriptionTypeRepository.IsSubscriptionCodeExistsAsync(request.SubscriptionCode, id);
                if (codeExists)
                    throw new InvalidOperationException($"Subscription code '{request.SubscriptionCode}' already exists.");

                // Update properties
                existingSubscriptionType.SubscriptionCode = request.SubscriptionCode;
                existingSubscriptionType.SubscriptionName = request.SubscriptionName;
                existingSubscriptionType.SubscriptionPrice = request.SubscriptionPrice;
                existingSubscriptionType.Description = request.Description;
                existingSubscriptionType.MaxSolutionViews = request.MaxSolutionViews;
                existingSubscriptionType.MaxAIRequests = request.MaxAIRequests;
                existingSubscriptionType.IsAIEnabled = request.IsAIEnabled;
                existingSubscriptionType.Features = request.Features;
                existingSubscriptionType.IsActive = request.IsActive;
                existingSubscriptionType.UpdatedBy = request.UpdatedBy;
                existingSubscriptionType.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SubscriptionTypeRepository.UpdateAsync(existingSubscriptionType);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("Updated subscription type with ID {Id}.", id);

                return MapToResponse(existingSubscriptionType);
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
                _logger.LogError(ex, "Error updating subscription type with ID {Id}: {Message}", id, ex.Message);
                throw new ApplicationException("An error occurred while updating the subscription type.", ex);
            }
        }

        public async Task<bool> DeleteSubscriptionTypeAsync(int id)
        {
            try
            {
                var subscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(id);
                if (subscriptionType == null)
                    throw new NotFoundException($"Subscription type with ID {id} not found.");

                // Check if subscription type is being used
                var hasActiveSubscriptions = await _unitOfWork.UserSubscriptionRepository
                    .GetAllAsync(us => us.SubscriptionTypeId == id && us.IsActive);

                if (hasActiveSubscriptions.Any())
                    throw new InvalidOperationException("Cannot delete subscription type that has active user subscriptions.");

                await _unitOfWork.SubscriptionTypeRepository.DeleteAsync(subscriptionType);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("Deleted subscription type with ID {Id}.", id);
                return true;
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
                _logger.LogError(ex, "Error deleting subscription type with ID {Id}: {Message}", id, ex.Message);
                throw new ApplicationException("An error occurred while deleting the subscription type.", ex);
            }
        }

        public async Task<bool> ActivateSubscriptionTypeAsync(int id)
        {
            try
            {
                var subscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(id);
                if (subscriptionType == null)
                    throw new NotFoundException($"Subscription type with ID {id} not found.");

                subscriptionType.IsActive = true;
                subscriptionType.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SubscriptionTypeRepository.UpdateAsync(subscriptionType);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("Activated subscription type with ID {Id}.", id);
                return true;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating subscription type with ID {Id}: {Message}", id, ex.Message);
                throw new ApplicationException("An error occurred while activating the subscription type.", ex);
            }
        }

        public async Task<bool> DeactivateSubscriptionTypeAsync(int id)
        {
            try
            {
                var subscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(id);
                if (subscriptionType == null)
                    throw new NotFoundException($"Subscription type with ID {id} not found.");

                subscriptionType.IsActive = false;
                subscriptionType.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SubscriptionTypeRepository.UpdateAsync(subscriptionType);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("Deactivated subscription type with ID {Id}.", id);
                return true;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating subscription type with ID {Id}: {Message}", id, ex.Message);
                throw new ApplicationException("An error occurred while deactivating the subscription type.", ex);
            }
        }

        public async Task<bool> IsSubscriptionCodeExistsAsync(string subscriptionCode, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subscriptionCode))
                    throw new ArgumentException("Subscription code cannot be null or empty.", nameof(subscriptionCode));

                return await _unitOfWork.SubscriptionTypeRepository.IsSubscriptionCodeExistsAsync(subscriptionCode, excludeId);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking subscription code existence: {Message}", ex.Message);
                throw new ApplicationException("An error occurred while checking subscription code existence.", ex);
            }
        }

        private static SubscriptionTypeResponse MapToResponse(SubscriptionType subscriptionType)
        {
            return new SubscriptionTypeResponse
            {
                Id = subscriptionType.Id,
                SubscriptionCode = subscriptionType.SubscriptionCode,
                SubscriptionName = subscriptionType.SubscriptionName,
                SubscriptionPrice = subscriptionType.SubscriptionPrice,
                Description = subscriptionType.Description,
                MaxSolutionViews = subscriptionType.MaxSolutionViews,
                MaxAIRequests = subscriptionType.MaxAIRequests,
                IsAIEnabled = subscriptionType.IsAIEnabled,
                Features = subscriptionType.Features,
                IsActive = subscriptionType.IsActive,
                CreatedAt = subscriptionType.CreatedAt,
                UpdatedAt = subscriptionType.UpdatedAt,
                UpdatedBy = subscriptionType.UpdatedBy,
                UpdatedByName = subscriptionType.UpdatedByNavigation?.FullName
            };
        }
    }
}