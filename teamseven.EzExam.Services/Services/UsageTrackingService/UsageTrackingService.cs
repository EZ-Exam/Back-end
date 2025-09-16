using Microsoft.EntityFrameworkCore;
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

        public UsageTrackingService(IUnitOfWork unitOfWork, ILogger<UsageTrackingService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> CanUserPerformActionAsync(int userId, string actionType)
        {
            try
            {
                // Get user's active subscription
                var activeSubscription = await _unitOfWork.UserSubscriptionRepository
                    .GetAllAsync(us => us.UserId == userId && us.IsActive && us.PaymentStatus == "Completed")
                    .FirstOrDefaultAsync();

                if (activeSubscription == null)
                {
                    _logger.LogWarning("User {UserId} has no active subscription", userId);
                    return false;
                }

                // Get subscription type details
                var subscriptionType = await _unitOfWork.SubscriptionTypeRepository
                    .GetByIdAsync(activeSubscription.SubscriptionTypeId);

                if (subscriptionType == null)
                {
                    _logger.LogWarning("Subscription type not found for user {UserId}", userId);
                    return false;
                }

                // Check AI access
                if (actionType == "AI_REQUEST" && !subscriptionType.IsAIEnabled)
                {
                    _logger.LogInformation("User {UserId} does not have AI access", userId);
                    return false;
                }

                // Get current usage for today
                var today = DateTime.UtcNow.Date;
                var currentUsage = await _unitOfWork.UserUsageTrackingRepository
                    .GetAllAsync(ut => ut.UserId == userId && 
                                      ut.UsageType == actionType && 
                                      ut.SubscriptionTypeId == subscriptionType.Id &&
                                      ut.ResetDate == today)
                    .FirstOrDefaultAsync();

                var currentCount = currentUsage?.UsageCount ?? 0;

                // Check limits
                int maxAllowed = actionType switch
                {
                    "SOLUTION_VIEW" => subscriptionType.MaxSolutionViews,
                    "AI_REQUEST" => subscriptionType.MaxAIRequests,
                    _ => 0
                };

                // -1 means unlimited
                if (maxAllowed == -1)
                {
                    return true;
                }

                return currentCount < maxAllowed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {UserId} can perform action {ActionType}: {Message}", 
                    userId, actionType, ex.Message);
                return false;
            }
        }

        public async Task<bool> IncrementUsageAsync(UsageTrackingRequest request)
        {
            try
            {
                // Check if user can perform action
                var canPerform = await CanUserPerformActionAsync(request.UserId, request.UsageType);
                if (!canPerform)
                {
                    _logger.LogWarning("User {UserId} cannot perform action {ActionType} - limit exceeded", 
                        request.UserId, request.UsageType);
                    return false;
                }

                // Get user's active subscription
                var activeSubscription = await _unitOfWork.UserSubscriptionRepository
                    .GetAllAsync(us => us.UserId == request.UserId && us.IsActive && us.PaymentStatus == "Completed")
                    .FirstOrDefaultAsync();

                if (activeSubscription == null)
                {
                    _logger.LogWarning("User {UserId} has no active subscription", request.UserId);
                    return false;
                }

                var today = DateTime.UtcNow.Date;

                // Update or create usage tracking
                var usageTracking = await _unitOfWork.UserUsageTrackingRepository
                    .GetAllAsync(ut => ut.UserId == request.UserId && 
                                      ut.UsageType == request.UsageType && 
                                      ut.SubscriptionTypeId == activeSubscription.SubscriptionTypeId &&
                                      ut.ResetDate == today)
                    .FirstOrDefaultAsync();

                if (usageTracking == null)
                {
                    usageTracking = new UserUsageTracking
                    {
                        UserId = request.UserId,
                        SubscriptionTypeId = activeSubscription.SubscriptionTypeId,
                        UsageType = request.UsageType,
                        UsageCount = 1,
                        ResetDate = today,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.UserUsageTrackingRepository.CreateAsync(usageTracking);
                }
                else
                {
                    usageTracking.UsageCount++;
                    usageTracking.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.UserUsageTrackingRepository.UpdateAsync(usageTracking);
                }

                // Add to usage history
                var usageHistory = new UserUsageHistory
                {
                    UserId = request.UserId,
                    UsageType = request.UsageType,
                    ResourceId = request.ResourceId,
                    ResourceType = request.ResourceType,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.UserUsageHistoryRepository.CreateAsync(usageHistory);

                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("Incremented usage for user {UserId}, action {ActionType}", 
                    request.UserId, request.UsageType);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing usage for user {UserId}: {Message}", 
                    request.UserId, ex.Message);
                return false;
            }
        }

        public async Task<UserSubscriptionStatusResponse> GetUserSubscriptionStatusAsync(int userId)
        {
            try
            {
                var activeSubscription = await _unitOfWork.UserSubscriptionRepository
                    .GetAllAsync(us => us.UserId == userId && us.IsActive && us.PaymentStatus == "Completed")
                    .Include(us => us.User)
                    .Include(us => us.SubscriptionType)
                    .FirstOrDefaultAsync();

                if (activeSubscription == null)
                {
                    // Return default free subscription status
                    return new UserSubscriptionStatusResponse
                    {
                        UserId = userId,
                        SubscriptionCode = "FREE",
                        SubscriptionName = "Free Plan",
                        MaxSolutionViews = 5,
                        MaxAIRequests = 0,
                        IsAIEnabled = false,
                        SubscriptionActive = false,
                        CurrentSolutionViews = 0,
                        CurrentAIRequests = 0,
                        RemainingSolutionViews = "5",
                        RemainingAIRequests = "0",
                        CanViewSolution = false,
                        CanUseAI = false
                    };
                }

                var subscriptionType = activeSubscription.SubscriptionType;
                var today = DateTime.UtcNow.Date;

                // Get current usage
                var solutionUsage = await _unitOfWork.UserUsageTrackingRepository
                    .GetAllAsync(ut => ut.UserId == userId && 
                                      ut.UsageType == "SOLUTION_VIEW" && 
                                      ut.SubscriptionTypeId == subscriptionType.Id &&
                                      ut.ResetDate == today)
                    .FirstOrDefaultAsync();

                var aiUsage = await _unitOfWork.UserUsageTrackingRepository
                    .GetAllAsync(ut => ut.UserId == userId && 
                                      ut.UsageType == "AI_REQUEST" && 
                                      ut.SubscriptionTypeId == subscriptionType.Id &&
                                      ut.ResetDate == today)
                    .FirstOrDefaultAsync();

                var currentSolutionViews = solutionUsage?.UsageCount ?? 0;
                var currentAIRequests = aiUsage?.UsageCount ?? 0;

                // Calculate remaining
                var remainingSolutionViews = subscriptionType.MaxSolutionViews == -1 
                    ? "Unlimited" 
                    : Math.Max(0, subscriptionType.MaxSolutionViews - currentSolutionViews).ToString();

                var remainingAIRequests = subscriptionType.MaxAIRequests == -1 
                    ? "Unlimited" 
                    : Math.Max(0, subscriptionType.MaxAIRequests - currentAIRequests).ToString();

                // Check permissions
                var canViewSolution = subscriptionType.MaxSolutionViews == -1 || 
                                     currentSolutionViews < subscriptionType.MaxSolutionViews;
                var canUseAI = subscriptionType.IsAIEnabled && 
                              (subscriptionType.MaxAIRequests == -1 || currentAIRequests < subscriptionType.MaxAIRequests);

                return new UserSubscriptionStatusResponse
                {
                    UserId = userId,
                    UserEmail = activeSubscription.User.Email,
                    UserName = activeSubscription.User.FullName ?? "Unknown",
                    SubscriptionCode = subscriptionType.SubscriptionCode,
                    SubscriptionName = subscriptionType.SubscriptionName,
                    MaxSolutionViews = subscriptionType.MaxSolutionViews,
                    MaxAIRequests = subscriptionType.MaxAIRequests,
                    IsAIEnabled = subscriptionType.IsAIEnabled,
                    SubscriptionActive = activeSubscription.IsActive,
                    SubscriptionEndDate = activeSubscription.EndDate,
                    CurrentSolutionViews = currentSolutionViews,
                    CurrentAIRequests = currentAIRequests,
                    RemainingSolutionViews = remainingSolutionViews,
                    RemainingAIRequests = remainingAIRequests,
                    CanViewSolution = canViewSolution,
                    CanUseAI = canUseAI
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription status for user {UserId}: {Message}", 
                    userId, ex.Message);
                throw new ApplicationException("An error occurred while retrieving subscription status.", ex);
            }
        }

        public async Task<IEnumerable<UsageTrackingResponse>> GetUserUsageTrackingAsync(int userId)
        {
            try
            {
                var usageTrackings = await _unitOfWork.UserUsageTrackingRepository
                    .GetAllAsync(ut => ut.UserId == userId)
                    .Include(ut => ut.SubscriptionType)
                    .OrderByDescending(ut => ut.ResetDate)
                    .ThenBy(ut => ut.UsageType)
                    .ToListAsync();

                return usageTrackings.Select(ut => new UsageTrackingResponse
                {
                    Id = ut.Id,
                    UserId = ut.UserId,
                    SubscriptionTypeId = ut.SubscriptionTypeId,
                    UsageType = ut.UsageType,
                    UsageCount = ut.UsageCount,
                    ResetDate = ut.ResetDate,
                    CreatedAt = ut.CreatedAt,
                    UpdatedAt = ut.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting usage tracking for user {UserId}: {Message}", 
                    userId, ex.Message);
                throw new ApplicationException("An error occurred while retrieving usage tracking.", ex);
            }
        }

        public async Task<IEnumerable<UsageHistoryResponse>> GetUserUsageHistoryAsync(int userId, int? limit = null)
        {
            try
            {
                var query = _unitOfWork.UserUsageHistoryRepository
                    .GetAllAsync(uh => uh.UserId == userId)
                    .OrderByDescending(uh => uh.CreatedAt);

                if (limit.HasValue)
                {
                    query = query.Take(limit.Value);
                }

                var usageHistories = await query.ToListAsync();

                return usageHistories.Select(uh => new UsageHistoryResponse
                {
                    Id = uh.Id,
                    UserId = uh.UserId,
                    UsageType = uh.UsageType,
                    ResourceId = uh.ResourceId,
                    ResourceType = uh.ResourceType,
                    Description = uh.Description,
                    CreatedAt = uh.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting usage history for user {UserId}: {Message}", 
                    userId, ex.Message);
                throw new ApplicationException("An error occurred while retrieving usage history.", ex);
            }
        }

        public async Task<bool> ResetUserUsageAsync(int userId, string usageType)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var usageTrackings = await _unitOfWork.UserUsageTrackingRepository
                    .GetAllAsync(ut => ut.UserId == userId && 
                                      ut.UsageType == usageType && 
                                      ut.ResetDate == today)
                    .ToListAsync();

                foreach (var usage in usageTrackings)
                {
                    usage.UsageCount = 0;
                    usage.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.UserUsageTrackingRepository.UpdateAsync(usage);
                }

                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("Reset usage for user {UserId}, type {UsageType}", userId, usageType);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting usage for user {UserId}: {Message}", 
                    userId, ex.Message);
                return false;
            }
        }

        public async Task<bool> CheckAndIncrementSolutionViewAsync(int userId, int solutionId)
        {
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

        public async Task<bool> CheckAndIncrementAIRequestAsync(int userId, string description = null)
        {
            var request = new UsageTrackingRequest
            {
                UserId = userId,
                UsageType = "AI_REQUEST",
                ResourceType = "AI_CHAT",
                Description = description ?? "AI chat request"
            };

            return await IncrementUsageAsync(request);
        }
    }
}
