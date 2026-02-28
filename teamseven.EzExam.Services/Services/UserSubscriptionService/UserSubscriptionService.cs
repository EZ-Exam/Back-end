using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Dtos;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository.Repository;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Interfaces;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.UserSubscriptionService
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserSubscriptionService> _logger;
        private readonly AutoMapper.IMapper _mapper;

        public UserSubscriptionService(IUnitOfWork unitOfWork, ILogger<UserSubscriptionService> logger, AutoMapper.IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        private string GetSubscriptionAction(UserSubscription subscription)
        {
            return string.IsNullOrEmpty(subscription.PaymentGatewayTransactionId)
                ? "BUY_SUBSCRIPTION"
                : "TOPUP_GATEWAY";
        }

        public async Task<IEnumerable<UserSubscriptionDataResponse>> GetAllSubscriptionsAsync()
        {
            var subscriptions = await _unitOfWork.UserSubscriptionRepository.GetAllSubscriptionsAsync();
            var responses = _mapper.Map<IEnumerable<UserSubscriptionDataResponse>>(subscriptions);
            
            var responseList = responses.ToList();
            for (int i = 0; i < responseList.Count; i++)
            {
                responseList[i].ActionType = GetSubscriptionAction(subscriptions.ElementAt(i));
            }
            return responseList;
        }

        public async Task AddSubscriptionAsync(UserSubscriptionRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("CreateSubscriptionRequest is null.");
                throw new ArgumentNullException(nameof(request), "Subsription creation request cannot be null.");
            }
            var userSubscription = new UserSubscription
            {
                UserId = request.UserId,
                SubscriptionTypeId = request.SubscriptionTypeId,
                StartDate = DateTime.UtcNow,
                EndDate = request.EndDate ?? DateTime.UtcNow.AddMonths(1),
                IsActive = true,
                PaymentStatus = "Pending",
                Amount = request.Amount,
                PaymentGatewayTransactionId = request.PaymentGatewayTransactionId,
                CreatedAt = DateTime.UtcNow
            };
            try
            {
                await _unitOfWork.UserSubscriptionRepository.CreateAsync(userSubscription);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                _logger.LogInformation("Created sub with ID {SubId}.", userSubscription.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sub: {Message}", ex.Message);
                throw new ApplicationException("An error occurred while creating the sub.", ex);
            }
        }

        public async Task<UserSubscriptionResponse> GetSubscriptionByIdAsync(int id)
        {
            var subscription = await _unitOfWork.UserSubscriptionRepository.GetByIdAsync(id);
            if (subscription == null) throw new KeyNotFoundException("Subscription not found");
            return _mapper.Map<UserSubscriptionResponse>(subscription);
        }
        public async Task<RevenueSummaryResponse> GetRevenueAsync(DateTime? fromUtc, DateTime? toUtc)
        {
            var total = await _unitOfWork.UserSubscriptionRepository.SumCompletedAmountAsync(fromUtc, toUtc);
            var count = await _unitOfWork.UserSubscriptionRepository.CountCompletedAsync(fromUtc, toUtc);

            return new RevenueSummaryResponse
            {
                TotalAmount = total,
                CompletedCount = count,
                From = fromUtc,
                To = toUtc
            };
        }

        public async Task<UserSubscriptionResponse> GetByPaymentGatewayTransactionIdAsync(string transactionId)
        {
            var subscription = await  _unitOfWork.UserSubscriptionRepository.GetByPaymentGatewayTransactionIdAsync(transactionId);
            if (subscription == null) throw new KeyNotFoundException("Subscription not found");
            return _mapper.Map<UserSubscriptionResponse>(subscription);
        }
        public async Task UpdateAsync(UserSubscriptionResponse subscription)
        {
            if (subscription == null)
            {
                _logger.LogWarning("Update subscription is null.");
                throw new ArgumentNullException(nameof(subscription), "Subscription cannot be null.");
            }

            var existingSubscription = await _unitOfWork.UserSubscriptionRepository.GetByIdAsync(subscription.Id);
            if (existingSubscription == null)
                throw new NotFoundException($"Subscription with ID {subscription.Id} not found.");

            _mapper.Map(subscription, existingSubscription);
            existingSubscription.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.UserSubscriptionRepository.UpdateAsync(existingSubscription);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("Updated subscription with ID {SubscriptionId}.", subscription.Id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating subscription with ID {SubscriptionId}: {Message}", subscription.Id, ex.Message);
                throw new ApplicationException("A concurrency error occurred while updating the subscription.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription with ID {SubscriptionId}: {Message}", subscription.Id, ex.Message);
                throw new ApplicationException("An error occurred while updating the subscription.", ex);
            }
        }
        public async Task<IEnumerable<UserSubscriptionResponse>> GetAllByUserIdAsync(int userId)
        {
            var subscriptions = await _unitOfWork.UserSubscriptionRepository
                .GetByUserIdAsync(userId);

            if (subscriptions == null || !subscriptions.Any())
                throw new NotFoundException($"No subscriptions found for user with ID {userId}.");

            return _mapper.Map<IEnumerable<UserSubscriptionResponse>>(subscriptions);
        }

    }
}