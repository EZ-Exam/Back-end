using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.SubscriptionService
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(IUnitOfWork unitOfWork, ILogger<SubscriptionService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<SubscribeResponse> SubscribeUserAsync(int userId, SubscribeRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("Subscribe request is null.");
                throw new ArgumentNullException(nameof(request), "Subscribe request cannot be null.");
            }

            try
            {
                // Get user
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    throw new NotFoundException($"User with ID {userId} not found.");
                }

                // Get subscription type
                var subscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(request.SubscriptionTypeId);
                if (subscriptionType == null)
                {
                    _logger.LogWarning("Subscription type with ID {SubscriptionTypeId} not found", request.SubscriptionTypeId);
                    throw new NotFoundException($"Subscription type with ID {request.SubscriptionTypeId} not found.");
                }

                if (!subscriptionType.IsActive)
                {
                    _logger.LogWarning("Subscription type {SubscriptionCode} is not active", subscriptionType.SubscriptionCode);
                    throw new InvalidOperationException($"Subscription type {subscriptionType.SubscriptionCode} is not active.");
                }

                // Deactivate current active subscription
                var currentSubscriptions = await _unitOfWork.UserSubscriptionRepository
                    .GetAllAsync(us => us.UserId == userId && us.IsActive)
                    .ToListAsync();

                foreach (var currentSub in currentSubscriptions)
                {
                    currentSub.IsActive = false;
                    currentSub.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.UserSubscriptionRepository.UpdateAsync(currentSub);
                }

                // Calculate end date (1 month from now, except for UNLIMITED)
                DateTime? endDate = null;
                if (subscriptionType.SubscriptionCode != "UNLIMITED")
                {
                    endDate = DateTime.UtcNow.AddMonths(1);
                }

                // Create new subscription
                var newSubscription = new UserSubscription
                {
                    UserId = userId,
                    SubscriptionTypeId = request.SubscriptionTypeId,
                    Amount = subscriptionType.SubscriptionPrice,
                    PaymentStatus = "Completed", // Auto-completed for direct subscription
                    PaymentGatewayTransactionId = $"DIRECT_SUB_{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}",
                    StartDate = DateTime.UtcNow,
                    EndDate = endDate,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.UserSubscriptionRepository.AddAsync(newSubscription);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("User {UserId} subscribed to {SubscriptionCode} successfully", 
                    userId, subscriptionType.SubscriptionCode);

                return new SubscribeResponse
                {
                    UserId = userId,
                    UserEmail = user.Email,
                    SubscriptionTypeId = subscriptionType.Id,
                    SubscriptionCode = subscriptionType.SubscriptionCode,
                    SubscriptionName = subscriptionType.SubscriptionName,
                    SubscriptionPrice = subscriptionType.SubscriptionPrice ?? 0,
                    StartDate = newSubscription.StartDate,
                    EndDate = newSubscription.EndDate,
                    PaymentStatus = newSubscription.PaymentStatus,
                    IsActive = newSubscription.IsActive,
                    Message = $"Successfully subscribed to {subscriptionType.SubscriptionName}"
                };
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
                _logger.LogError(ex, "Error subscribing user {UserId} to subscription {SubscriptionTypeId}: {Message}", 
                    userId, request.SubscriptionTypeId, ex.Message);
                throw new ApplicationException("An error occurred while subscribing user.", ex);
            }
        }

        public async Task<bool> CheckAndExpireSubscriptionsAsync()
        {
            try
            {
                var currentTime = DateTime.UtcNow;
                var expiredSubscriptions = await _unitOfWork.UserSubscriptionRepository
                    .GetAllAsync(us => us.IsActive && 
                                     us.EndDate.HasValue && 
                                     us.EndDate.Value <= currentTime &&
                                     us.PaymentStatus == "Completed")
                    .ToListAsync();

                if (!expiredSubscriptions.Any())
                {
                    _logger.LogInformation("No expired subscriptions found");
                    return true;
                }

                // Get FREE subscription type
                var freeSubscriptionType = await _unitOfWork.SubscriptionTypeRepository
                    .GetAllAsync(st => st.SubscriptionCode == "FREE" && st.IsActive)
                    .FirstOrDefaultAsync();

                if (freeSubscriptionType == null)
                {
                    _logger.LogError("FREE subscription type not found");
                    return false;
                }

                var processedCount = 0;
                foreach (var expiredSub in expiredSubscriptions)
                {
                    try
                    {
                        // Deactivate expired subscription
                        expiredSub.IsActive = false;
                        expiredSub.UpdatedAt = currentTime;
                        await _unitOfWork.UserSubscriptionRepository.UpdateAsync(expiredSub);

                        // Create FREE subscription for user
                        var freeSubscription = new UserSubscription
                        {
                            UserId = expiredSub.UserId,
                            SubscriptionTypeId = freeSubscriptionType.Id,
                            Amount = 0,
                            PaymentStatus = "Completed",
                            PaymentGatewayTransactionId = $"AUTO_FREE_{expiredSub.UserId}_{currentTime:yyyyMMddHHmmss}",
                            StartDate = currentTime,
                            EndDate = null, // FREE subscription has no end date
                            IsActive = true,
                            CreatedAt = currentTime,
                            UpdatedAt = currentTime
                        };

                        await _unitOfWork.UserSubscriptionRepository.AddAsync(freeSubscription);
                        processedCount++;

                        _logger.LogInformation("Expired subscription for user {UserId} converted to FREE subscription", 
                            expiredSub.UserId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing expired subscription for user {UserId}: {Message}", 
                            expiredSub.UserId, ex.Message);
                    }
                }

                if (processedCount > 0)
                {
                    await _unitOfWork.SaveChangesWithTransactionAsync();
                    _logger.LogInformation("Successfully processed {Count} expired subscriptions", processedCount);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking and expiring subscriptions: {Message}", ex.Message);
                return false;
            }
        }

        public async Task<SubscribeResponse> GetUserCurrentSubscriptionAsync(int userId)
        {
            try
            {
                var activeSubscription = await _unitOfWork.UserSubscriptionRepository
                    .GetAllAsync(us => us.UserId == userId && us.IsActive && us.PaymentStatus == "Completed")
                    .Include(us => us.SubscriptionType)
                    .Include(us => us.User)
                    .FirstOrDefaultAsync();

                if (activeSubscription == null)
                {
                    _logger.LogWarning("No active subscription found for user {UserId}", userId);
                    throw new NotFoundException($"No active subscription found for user {userId}");
                }

                return new SubscribeResponse
                {
                    UserId = userId,
                    UserEmail = activeSubscription.User.Email,
                    SubscriptionTypeId = activeSubscription.SubscriptionTypeId,
                    SubscriptionCode = activeSubscription.SubscriptionType.SubscriptionCode,
                    SubscriptionName = activeSubscription.SubscriptionType.SubscriptionName,
                    SubscriptionPrice = activeSubscription.Amount ?? 0,
                    StartDate = activeSubscription.StartDate,
                    EndDate = activeSubscription.EndDate,
                    PaymentStatus = activeSubscription.PaymentStatus,
                    IsActive = activeSubscription.IsActive,
                    Message = "Current subscription retrieved successfully"
                };
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current subscription for user {UserId}: {Message}", userId, ex.Message);
                throw new ApplicationException("An error occurred while retrieving subscription.", ex);
            }
        }

        public async Task<bool> CancelUserSubscriptionAsync(int userId)
        {
            try
            {
                var activeSubscriptions = await _unitOfWork.UserSubscriptionRepository
                    .GetAllAsync(us => us.UserId == userId && us.IsActive)
                    .ToListAsync();

                if (!activeSubscriptions.Any())
                {
                    _logger.LogWarning("No active subscriptions found for user {UserId}", userId);
                    return false;
                }

                foreach (var subscription in activeSubscriptions)
                {
                    subscription.IsActive = false;
                    subscription.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.UserSubscriptionRepository.UpdateAsync(subscription);
                }

                await _unitOfWork.SaveChangesWithTransactionAsync();
                _logger.LogInformation("Successfully cancelled all subscriptions for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription for user {UserId}: {Message}", userId, ex.Message);
                return false;
            }
        }

        public async Task<List<SubscribeResponse>> GetUserSubscriptionHistoryAsync(int userId)
        {
            try
            {
                var subscriptions = await _unitOfWork.UserSubscriptionRepository
                    .GetAllAsync(us => us.UserId == userId)
                    .Include(us => us.SubscriptionType)
                    .Include(us => us.User)
                    .OrderByDescending(us => us.CreatedAt)
                    .ToListAsync();

                return subscriptions.Select(sub => new SubscribeResponse
                {
                    UserId = userId,
                    UserEmail = sub.User.Email,
                    SubscriptionTypeId = sub.SubscriptionTypeId,
                    SubscriptionCode = sub.SubscriptionType.SubscriptionCode,
                    SubscriptionName = sub.SubscriptionType.SubscriptionName,
                    SubscriptionPrice = sub.Amount ?? 0,
                    StartDate = sub.StartDate,
                    EndDate = sub.EndDate,
                    PaymentStatus = sub.PaymentStatus,
                    IsActive = sub.IsActive,
                    Message = sub.IsActive ? "Active subscription" : "Inactive subscription"
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription history for user {UserId}: {Message}", userId, ex.Message);
                throw new ApplicationException("An error occurred while retrieving subscription history.", ex);
            }
        }
    }
}
