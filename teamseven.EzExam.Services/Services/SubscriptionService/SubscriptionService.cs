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
            try
            {
                // Use repository with hardcoded data - no database queries needed
                // Validate user exists
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for subscription", userId);
                    throw new NotFoundException($"User with ID {userId} not found.");
                }

                // Validate subscription type exists - USE REPOSITORY WITH HARDCODED DATA
                var subscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(request.SubscriptionTypeId);
                if (subscriptionType == null)
                {
                    _logger.LogWarning("Subscription type with ID {SubscriptionTypeId} not found", request.SubscriptionTypeId);
                    throw new KeyNotFoundException($"Subscription type with ID {request.SubscriptionTypeId} not found.");
                }

                // Kiểm tra số dư nếu là subscription trả phí (không phải FREE)
                if (request.SubscriptionTypeId != 1 && subscriptionType.SubscriptionPrice > 0)
                {
                    if (user.Balance == null || user.Balance < subscriptionType.SubscriptionPrice)
                    {
                        _logger.LogWarning("User {UserId} has insufficient balance. Required: {Required}, Available: {Available}", 
                            userId, subscriptionType.SubscriptionPrice, user.Balance ?? 0);
                        
                        return new SubscribeResponse
                        {
                            UserId = userId,
                            UserEmail = user.Email,
                            SubscriptionTypeId = subscriptionType.Id,
                            SubscriptionCode = subscriptionType.SubscriptionCode,
                            SubscriptionName = subscriptionType.SubscriptionName,
                            SubscriptionPrice = subscriptionType.SubscriptionPrice ?? 0,
                            PaymentStatus = "INSUFFICIENT_BALANCE",
                            IsActive = false,
                            Message = $"Insufficient balance. Required: {subscriptionType.SubscriptionPrice}, Available: {user.Balance ?? 0}"
                        };
                    }
                }

                // Check if user has active subscription - OPTIMIZED QUERY
                var existingSubscription = await _unitOfWork.UserSubscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId);
                
                // Nếu có subscription cũ, xóa tất cả subscription active để đè lên gói mới
                if (existingSubscription != null)
                {
                    var allActiveSubscriptions = await _unitOfWork.UserSubscriptionRepository.GetActiveSubscriptionsAsync((long)userId);
                    if (allActiveSubscriptions != null && allActiveSubscriptions.Any())
                    {
                        foreach (var oldSubscription in allActiveSubscriptions)
                        {
                            oldSubscription.IsActive = false;
                            oldSubscription.UpdatedAt = DateTime.UtcNow;
                            await _unitOfWork.UserSubscriptionRepository.UpdateAsync(oldSubscription);
                        }
                        _logger.LogInformation("Deactivated {Count} old subscriptions for user {UserId} to replace with new subscription", 
                            allActiveSubscriptions.Count, userId);
                    }
                }

                // Create new subscription
                var userSubscription = new UserSubscription
                {
                    UserId = userId,
                    SubscriptionTypeId = request.SubscriptionTypeId,
                    Amount = subscriptionType.SubscriptionPrice,
                    StartDate = DateTime.UtcNow,
                    EndDate = CalculateEndDate(request.SubscriptionTypeId),
                    PaymentStatus = request.SubscriptionTypeId == 1 ? "Completed" : "Pending", // FREE is auto-completed
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.UserSubscriptionRepository.AddAsync(userSubscription);
                
                // Trừ tiền nếu là subscription trả phí
                if (request.SubscriptionTypeId != 1 && subscriptionType.SubscriptionPrice > 0)
                {
                    user.Balance -= subscriptionType.SubscriptionPrice;
                    await _unitOfWork.UserRepository.UpdateAsync(user);
                }
                
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("User {UserId} subscribed to subscription type {SubscriptionTypeId}", userId, request.SubscriptionTypeId);

                return new SubscribeResponse
                {
                    UserId = userId,
                    UserEmail = user.Email,
                    SubscriptionTypeId = subscriptionType.Id,
                    SubscriptionCode = subscriptionType.SubscriptionCode,
                    SubscriptionName = subscriptionType.SubscriptionName,
                    SubscriptionPrice = subscriptionType.SubscriptionPrice ?? 0,
                    StartDate = userSubscription.StartDate,
                    EndDate = userSubscription.EndDate,
                    PaymentStatus = userSubscription.PaymentStatus,
                    IsActive = userSubscription.IsActive,
                    Message = "Subscription created successfully."
                };
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing user {UserId}: {Message}", userId, ex.Message);
                throw new ApplicationException("An error occurred while creating subscription.", ex);
            }
        }

        public async Task<bool> CheckAndExpireSubscriptionsAsync()
        {
            try
            {
                var expiredSubscriptions = await _unitOfWork.UserSubscriptionRepository.GetExpiredSubscriptionsAsync();
                var expiredCount = 0;

                foreach (var subscription in expiredSubscriptions)
                {
                    subscription.IsActive = false;
                    subscription.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.UserSubscriptionRepository.UpdateAsync(subscription);
                    expiredCount++;
                }

                if (expiredCount > 0)
                {
                    await _unitOfWork.SaveChangesWithTransactionAsync();
                    _logger.LogInformation("Expired {Count} subscriptions", expiredCount);
                }

                return expiredCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking and expiring subscriptions: {Message}", ex.Message);
                throw new ApplicationException("An error occurred while checking subscription expiration.", ex);
            }
        }

        public async Task<SubscribeResponse> GetUserCurrentSubscriptionAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    throw new NotFoundException($"User with ID {userId} not found.");
                }

                var subscription = await _unitOfWork.UserSubscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId);
                if (subscription == null)
                {
                    return new SubscribeResponse
                    {
                        UserId = userId,
                        UserEmail = user.Email,
                        Message = "No active subscription found."
                    };
                }

                var subscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(subscription.SubscriptionTypeId);

                return new SubscribeResponse
                {
                    UserId = userId,
                    UserEmail = user.Email,
                    SubscriptionTypeId = subscription.SubscriptionTypeId,
                    SubscriptionCode = subscriptionType?.SubscriptionCode ?? "Unknown",
                    SubscriptionName = subscriptionType?.SubscriptionName ?? "Unknown",
                    SubscriptionPrice = subscriptionType?.SubscriptionPrice ?? 0,
                    StartDate = subscription.StartDate,
                    EndDate = subscription.EndDate,
                    PaymentStatus = subscription.PaymentStatus,
                    IsActive = subscription.IsActive,
                    Message = "Current subscription retrieved successfully."
                };
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current subscription for user {UserId}: {Message}", userId, ex.Message);
                throw new ApplicationException("An error occurred while retrieving current subscription.", ex);
            }
        }

        public async Task<bool> CancelUserSubscriptionAsync(int userId)
        {
            try
            {
                // get all instead of oneone
                var activeSubscriptions = await _unitOfWork.UserSubscriptionRepository.GetActiveSubscriptionsAsync((long)userId);
                _logger.LogInformation("Found {Count} active subscriptions for user {UserId}", activeSubscriptions?.Count ?? 0, userId);
                
                if (activeSubscriptions == null || !activeSubscriptions.Any())
                {
                    _logger.LogWarning("No active subscription found for user {UserId}", userId);
                    return false;
                }

                // Cancel tất cả subscription active
                foreach (var subscription in activeSubscriptions)
                {
                    _logger.LogInformation("Cancelling subscription ID {SubscriptionId} for user {UserId}", subscription.Id, userId);
                    subscription.IsActive = false;
                    subscription.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.UserSubscriptionRepository.UpdateAsync(subscription);
                }

                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("Cancelled {Count} active subscriptions for user {UserId}", activeSubscriptions.Count, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription for user {UserId}: {Message}", userId, ex.Message);
                throw new ApplicationException("An error occurred while cancelling subscription.", ex);
            }
        }

        public async Task<List<SubscribeResponse>> GetUserSubscriptionHistoryAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    throw new NotFoundException($"User with ID {userId} not found.");
                }

                var subscriptions = await _unitOfWork.UserSubscriptionRepository.GetSubscriptionsByUserIdAsync(userId);
                var result = new List<SubscribeResponse>();

                foreach (var subscription in subscriptions)
                {
                    var subscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(subscription.SubscriptionTypeId);
                    
                    result.Add(new SubscribeResponse
                    {
                        UserId = userId,
                        UserEmail = user.Email,
                        SubscriptionTypeId = subscription.SubscriptionTypeId,
                        SubscriptionCode = subscriptionType?.SubscriptionCode ?? "Unknown",
                        SubscriptionName = subscriptionType?.SubscriptionName ?? "Unknown",
                        SubscriptionPrice = subscriptionType?.SubscriptionPrice ?? 0,
                        StartDate = subscription.StartDate,
                        EndDate = subscription.EndDate,
                        PaymentStatus = subscription.PaymentStatus,
                        IsActive = subscription.IsActive,
                        Message = "Subscription history retrieved successfully."
                    });
                }

                return result;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription history for user {UserId}: {Message}", userId, ex.Message);
                throw new ApplicationException("An error occurred while retrieving subscription history.", ex);
            }
        }

        /// <summary>
        /// Calculate end date based on subscription type
        /// </summary>
        /// <param name="subscriptionTypeId">Subscription type ID</param>
        /// <returns>End date</returns>
        private DateTime CalculateEndDate(int subscriptionTypeId)
        {
            return subscriptionTypeId switch
            {
                1 => DateTime.UtcNow.AddYears(10), // FREE - 10 years (effectively permanent)
                2 => DateTime.UtcNow.AddMonths(1),  // BASIC - 1 month
                3 => DateTime.UtcNow.AddMonths(1),  // PREMIUM - 1 month
                4 => DateTime.UtcNow.AddMonths(1),  // UNLIMITED - 1 month
                _ => DateTime.UtcNow.AddMonths(1)   // Default - 1 month
            };
        }
    }
}