using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.BalanceService
{
    public class BalanceService : IBalanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BalanceService> _logger;
        private readonly IConfiguration _configuration;

        public BalanceService(
            IUnitOfWork unitOfWork, 
            ILogger<BalanceService> logger,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<BalanceResponse> AddBalanceAsync(int userId, AddBalanceRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("Add balance request is null.");
                throw new ArgumentNullException(nameof(request), "Add balance request cannot be null.");
            }

            try
            {
                // Validate super secret key
                var validSuperSecretKey = _configuration["Security:SuperSecretKey"];
                if (string.IsNullOrEmpty(validSuperSecretKey))
                {
                    _logger.LogError("Super secret key is not configured in appsettings.json");
                    throw new InvalidOperationException("Super secret key is not configured.");
                }

                if (request.SuperSecretKey != validSuperSecretKey)
                {
                    _logger.LogWarning("Invalid super secret key provided for user {UserId}", userId);
                    throw new UnauthorizedAccessException("Invalid super secret key.");
                }

                // Get user
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    throw new NotFoundException($"User with ID {userId} not found.");
                }

                var previousBalance = user.Balance ?? 0;
                var newBalance = previousBalance + request.Amount;

                // Update user balance
                user.Balance = newBalance;
                user.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.UserRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("Added {Amount} to balance for user {UserId}. Previous: {PreviousBalance}, New: {NewBalance}", 
                    request.Amount, userId, previousBalance, newBalance);

                return new BalanceResponse
                {
                    UserId = userId,
                    UserEmail = user.Email,
                    UserName = user.FullName ?? "Unknown",
                    PreviousBalance = previousBalance,
                    AddedAmount = request.Amount,
                    NewBalance = newBalance,
                    Description = request.Description,
                    UpdatedAt = user.UpdatedAt
                };
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding balance for user {UserId}: {Message}", userId, ex.Message);
                throw new ApplicationException("An error occurred while adding balance.", ex);
            }
        }

        public async Task<BalanceResponse> MasterDepositAsync(MasterDepositRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("Master deposit request is null.");
                throw new ArgumentNullException(nameof(request), "Master deposit request cannot be null.");
            }

            try
            {
                // Validate super secret key
                var validSuperSecretKey = _configuration["Security:SuperSecretKey"];
                if (string.IsNullOrEmpty(validSuperSecretKey))
                {
                    _logger.LogError("Super secret key is not configured in appsettings.json");
                    throw new InvalidOperationException("Super secret key is not configured.");
                }

                if (request.SuperSecretKey != validSuperSecretKey)
                {
                    _logger.LogWarning("Invalid super secret key provided for master deposit to user {UserId}", request.UserId);
                    throw new UnauthorizedAccessException("Invalid super secret key.");
                }

                // Get user
                var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for master deposit", request.UserId);
                    throw new NotFoundException($"User with ID {request.UserId} not found.");
                }

                var previousBalance = user.Balance ?? 0;
                var newBalance = previousBalance + request.Amount;

                // Update user balance
                user.Balance = newBalance;
                user.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.UserRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                _logger.LogInformation("Master deposit: Added {Amount} to balance for user {UserId}. Previous: {PreviousBalance}, New: {NewBalance}", 
                    request.Amount, request.UserId, previousBalance, newBalance);

                return new BalanceResponse
                {
                    UserId = request.UserId,
                    UserEmail = user.Email,
                    UserName = user.FullName ?? "Unknown",
                    PreviousBalance = previousBalance,
                    AddedAmount = request.Amount,
                    NewBalance = newBalance,
                    Description = request.Description ?? "Master deposit",
                    UpdatedAt = user.UpdatedAt
                };
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in master deposit for user {UserId}: {Message}", request.UserId, ex.Message);
                throw new ApplicationException("An error occurred while processing master deposit.", ex);
            }
        }

        public async Task<decimal> GetUserBalanceAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    throw new NotFoundException($"User with ID {userId} not found.");
                }

                return user.Balance ?? 0;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting balance for user {UserId}: {Message}", userId, ex.Message);
                throw new ApplicationException("An error occurred while retrieving balance.", ex);
            }
        }

        public async Task<BalanceResponse> GetUserBalanceInfoAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    throw new NotFoundException($"User with ID {userId} not found.");
                }

                var currentBalance = user.Balance ?? 0;

                return new BalanceResponse
                {
                    UserId = userId,
                    UserEmail = user.Email,
                    UserName = user.FullName ?? "Unknown",
                    PreviousBalance = currentBalance,
                    AddedAmount = 0,
                    NewBalance = currentBalance,
                    Description = "Current balance",
                    UpdatedAt = user.UpdatedAt
                };
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting balance info for user {UserId}: {Message}", userId, ex.Message);
                throw new ApplicationException("An error occurred while retrieving balance info.", ex);
            }
        }
    }
}
