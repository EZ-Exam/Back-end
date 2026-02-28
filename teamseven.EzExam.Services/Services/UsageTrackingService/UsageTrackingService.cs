using AutoMapper;
using Microsoft.Extensions.Logging;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.UsageTrackingService
{
    public class UsageTrackingService : IUsageTrackingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UsageTrackingService> _logger;
        private readonly IMapper _mapper;

        public UsageTrackingService(IUnitOfWork unitOfWork, ILogger<UsageTrackingService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<bool> CanUserPerformActionAsync(int userId, string actionType)
        {
            try
            {
                var userSubscription = await GetActiveUserSubscriptionAsync(userId);
                if (userSubscription == null)
                {
                    _logger.LogWarning("No active subscription found for user {UserId}", userId);
                    return false;
                }

                var subscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(userSubscription.SubscriptionTypeId);
                if (subscriptionType == null)
                {
                    _logger.LogWarning("Subscription type not found for user {UserId}", userId);
                    return false;
                }

                if (userSubscription.EndDate.HasValue && userSubscription.EndDate.Value < DateTime.UtcNow)
                {
                    _logger.LogWarning("Subscription expired for user {UserId}", userId);
                    return false;
                }

                switch (actionType.ToUpper())
                {
                    case "AI_REQUEST":
                        if (!subscriptionType.IsAIEnabled)
                        {
                            _logger.LogInformation("AI not enabled for user {UserId} subscription", userId);
                            return false;
                        }

                        var aiUsage = await GetCurrentUsageAsync(userId, subscriptionType.Id, "AI_REQUEST");
                        if (subscriptionType.MaxAIRequests != -1 && aiUsage >= subscriptionType.MaxAIRequests)
                        {
                            _logger.LogInformation("AI request limit exceeded for user {UserId}", userId);
                            return false;
                        }
                        break;

                    case "SOLUTION_VIEW":
                        var solutionUsage = await GetCurrentUsageAsync(userId, subscriptionType.Id, "SOLUTION_VIEW");
                        if (subscriptionType.MaxSolutionViews != -1 && solutionUsage >= subscriptionType.MaxSolutionViews)
                        {
                            _logger.LogInformation("Solution view limit exceeded for user {UserId}", userId);
                            return false;
                        }
                        break;

                    default:
                        _logger.LogWarning("Unknown action type: {ActionType}", actionType);
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {UserId} can perform action {ActionType}: {Message}", userId, actionType, ex.Message);
                return false;
            }
        }

        public async Task<bool> IncrementUsageAsync(UsageTrackingRequest request)
        {
            try
            {
                var userSubscription = await GetActiveUserSubscriptionAsync(request.UserId);
                if (userSubscription == null)
                {
                    _logger.LogWarning("No active subscription found for user {UserId}", request.UserId);
                    return false;
                }

                var resetDate = GetResetDate();
                var usageTracking = await _unitOfWork.UserUsageTrackingRepository.GetUserUsageTrackingAsync(
                    request.UserId, userSubscription.SubscriptionTypeId, request.UsageType, resetDate);

                if (usageTracking == null)
                {
                    usageTracking = _mapper.Map<UserUsageTracking>(request);
                    usageTracking.SubscriptionTypeId = userSubscription.SubscriptionTypeId;
                    usageTracking.UsageCount = 1;
                    usageTracking.ResetDate = resetDate;
                    usageTracking.CreatedAt = DateTime.UtcNow;
                    usageTracking.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.UserUsageTrackingRepository.AddAsync(usageTracking);
                }
                else
                {
                    usageTracking.UsageCount++;
                    usageTracking.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.UserUsageTrackingRepository.UpdateAsync(usageTracking);
                }

                var usageHistory = _mapper.Map<UserUsageHistory>(request);
                usageHistory.CreatedAt = DateTime.UtcNow;
                await _unitOfWork.UserUsageHistoryRepository.AddAsync(usageHistory);

                await _unitOfWork.SaveChangesWithTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing usage for user {UserId}: {Message}", request.UserId, ex.Message);
                return false;
            }
        }

        public async Task<UserSubscriptionStatusResponse> GetUserSubscriptionStatusAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new NotFoundException($"User with ID {userId} not found.");
                }

                var userSubscription = await GetActiveUserSubscriptionAsync(userId);
                if (userSubscription == null)
                {
                    return new UserSubscriptionStatusResponse
                    {
                        UserId = userId,
                        UserEmail = user.Email,
                        UserName = user.FullName,
                        SubscriptionCode = "FREE",
                        SubscriptionName = "Free Plan",
                        MaxSolutionViews = 0,
                        MaxAIRequests = 0,
                        IsAIEnabled = false,
                        SubscriptionActive = false,
                        CurrentSolutionViews = 0,
                        CurrentAIRequests = 0,
                        RemainingSolutionViews = "0",
                        RemainingAIRequests = "0",
                        CanViewSolution = false,
                        CanUseAI = false
                    };
                }

                var subscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(userSubscription.SubscriptionTypeId);
                if (subscriptionType == null)
                {
                    throw new NotFoundException($"Subscription type not found for user {userId}.");
                }

                var isActive = !userSubscription.EndDate.HasValue || userSubscription.EndDate.Value > DateTime.UtcNow;
                var currentSolutionViews = await GetCurrentUsageAsync(userId, subscriptionType.Id, "SOLUTION_VIEW");
                var currentAIRequests = await GetCurrentUsageAsync(userId, subscriptionType.Id, "AI_REQUEST");

                var remainingSolutionViews = subscriptionType.MaxSolutionViews == -1 ? "Unlimited" : 
                    Math.Max(0, subscriptionType.MaxSolutionViews - currentSolutionViews).ToString();
                var remainingAIRequests = subscriptionType.MaxAIRequests == -1 ? "Unlimited" : 
                    Math.Max(0, subscriptionType.MaxAIRequests - currentAIRequests).ToString();

                return new UserSubscriptionStatusResponse
                {
                    UserId = userId,
                    UserEmail = user.Email,
                    UserName = user.FullName,
                    SubscriptionCode = subscriptionType.SubscriptionCode,
                    SubscriptionName = subscriptionType.SubscriptionName,
                    MaxSolutionViews = subscriptionType.MaxSolutionViews,
                    MaxAIRequests = subscriptionType.MaxAIRequests,
                    IsAIEnabled = subscriptionType.IsAIEnabled,
                    SubscriptionActive = isActive,
                    SubscriptionEndDate = userSubscription.EndDate,
                    CurrentSolutionViews = currentSolutionViews,
                    CurrentAIRequests = currentAIRequests,
                    RemainingSolutionViews = remainingSolutionViews,
                    RemainingAIRequests = remainingAIRequests,
                    CanViewSolution = isActive && (subscriptionType.MaxSolutionViews == -1 || currentSolutionViews < subscriptionType.MaxSolutionViews),
                    CanUseAI = isActive && subscriptionType.IsAIEnabled && (subscriptionType.MaxAIRequests == -1 || currentAIRequests < subscriptionType.MaxAIRequests)
                };
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription status for user {UserId}: {Message}", userId, ex.Message);
                throw new ApplicationException("An error occurred while retrieving subscription status.", ex);
            }
        }

        public async Task<IEnumerable<UsageTrackingResponse>> GetUserUsageTrackingAsync(int userId)
        {
            try
            {
                var userSubscription = await GetActiveUserSubscriptionAsync(userId);
                if (userSubscription == null)
                {
                    return new List<UsageTrackingResponse>();
                }

                var resetDate = GetResetDate();
                var usageTrackings = await _unitOfWork.UserUsageTrackingRepository.GetAllAsync();
                var userUsageTrackings = usageTrackings.Where(ut => 
                    ut.UserId == userId && 
                    ut.SubscriptionTypeId == userSubscription.SubscriptionTypeId &&
                    ut.ResetDate == resetDate);

                return userUsageTrackings.Select(ut => _mapper.Map<UsageTrackingResponse>(ut));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting usage tracking for user {UserId}: {Message}", userId, ex.Message);
                throw new ApplicationException("An error occurred while retrieving usage tracking.", ex);
            }
        }

        public async Task<IEnumerable<UsageHistoryResponse>> GetUserUsageHistoryAsync(int userId, int? limit = null)
        {
            try
            {
                var usageHistories = await _unitOfWork.UserUsageHistoryRepository.GetUserUsageHistoryAsync(userId, null, limit);
                return usageHistories.Select(h => _mapper.Map<UsageHistoryResponse>(h));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting usage history for user {UserId}: {Message}", userId, ex.Message);
                throw new ApplicationException("An error occurred while retrieving usage history.", ex);
            }
        }

        public async Task ResetUserUsageAsync(int userId, string usageType)
        {
            var userSubscription = await GetActiveUserSubscriptionAsync(userId)
                ?? throw new NotFoundException($"No active subscription found for user {userId}.");

            var resetDate = GetResetDate();
            var usageTracking = await _unitOfWork.UserUsageTrackingRepository.GetUserUsageTrackingAsync(
                userId, userSubscription.SubscriptionTypeId, usageType, resetDate);

            if (usageTracking != null)
            {
                usageTracking.UsageCount = 0;
                usageTracking.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.UserUsageTrackingRepository.UpdateAsync(usageTracking);
                await _unitOfWork.SaveChangesWithTransactionAsync();
            }

            _logger.LogInformation("Usage reset for user {UserId}, type {UsageType}", userId, usageType);
        }

        public async Task<bool> CheckAndIncrementSolutionViewAsync(int userId, int solutionId)
        {
            try
            {
                var canView = await CanUserPerformActionAsync(userId, "SOLUTION_VIEW");
                if (!canView)
                {
                    return false;
                }

                var request = new UsageTrackingRequest
                {
                    UserId = userId,
                    UsageType = "SOLUTION_VIEW",
                    ResourceId = solutionId,
                    ResourceType = "SOLUTION",
                    Description = $"Viewed solution {solutionId}"
                };

                return await IncrementUsageAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking and incrementing solution view for user {UserId}, solution {SolutionId}: {Message}", userId, solutionId, ex.Message);
                return false;
            }
        }

        public async Task<bool> CheckAndIncrementAIRequestAsync(int userId, string description = null)
        {
            try
            {
                var canRequest = await CanUserPerformActionAsync(userId, "AI_REQUEST");
                if (!canRequest)
                {
                    return false;
                }

                var request = new UsageTrackingRequest
                {
                    UserId = userId,
                    UsageType = "AI_REQUEST",
                    ResourceType = "AI_CHAT",
                    Description = description ?? "AI request"
                };

                return await IncrementUsageAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking and incrementing AI request for user {UserId}: {Message}", userId, ex.Message);
                return false;
            }
        }

        private async Task<UserSubscription?> GetActiveUserSubscriptionAsync(int userId)
        {
            try
            {
                var activeSubscriptions = await _unitOfWork.UserSubscriptionRepository.GetActiveSubscriptionsAsync(userId);
                if (activeSubscriptions == null || !activeSubscriptions.Any())
                {
                    return null;
                }

                return activeSubscriptions
                    .Where(s => s.IsActive && (!s.EndDate.HasValue || s.EndDate.Value > DateTime.UtcNow))
                    .OrderByDescending(s => s.StartDate)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active subscription for user {UserId}: {Message}", userId, ex.Message);
                return null;
            }
        }

        private async Task<int> GetCurrentUsageAsync(int userId, int subscriptionTypeId, string usageType)
        {
            try
            {
                var resetDate = GetResetDate();
                var usageTracking = await _unitOfWork.UserUsageTrackingRepository.GetUserUsageTrackingAsync(
                    userId, subscriptionTypeId, usageType, resetDate);
                return usageTracking?.UsageCount ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current usage for user {UserId}, type {UsageType}: {Message}", userId, usageType, ex.Message);
                return 0;
            }
        }

        private static DateTime GetResetDate()
        {
            var now = DateTime.UtcNow;
            return new DateTime(now.Year, now.Month, 1);
        }

    }
}