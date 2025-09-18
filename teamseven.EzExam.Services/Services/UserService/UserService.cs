using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository.Repository;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Requests;

namespace teamseven.EzExam.Services.Services.UserService
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<UserResponse> GetUserByIdAsync(int id);
        Task<UserResponse> GetMyProfileAsync(int userId);
        Task UpdateUserAsync(UserResponse user);
        Task<UserResponse> SoftDeleteUserAsync(int id);
        Task<bool> UpgradeToPremiumAsync(int userId);
        Task<(bool IsSuccess, string ResultOrError)> UpdateUserProfileAsync(int id, UpdateUserProfileRequest request);
        Task<string?> GetOnlyUserNameById(int id);
    }

    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<UserResponse> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("User not found");

            return new UserResponse
            {
                Id = user.Id,
                Balance = user.Balance,
                Email = user.Email,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                RoleId = user.RoleId,
                IsActive = user.IsActive,
                EmailVerifiedAt = user.EmailVerifiedAt,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<UserResponse> GetMyProfileAsync(int userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException($"User with ID {userId} not found");

            return new UserResponse
            {
                Id = user.Id,
                Balance = user.Balance,
                Email = user.Email,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                RoleId = user.RoleId,
                IsActive = user.IsActive,
                EmailVerifiedAt = user.EmailVerifiedAt,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<UserResponse> SoftDeleteUserAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.SoftDeleteUserAsync(id);
            if (user == null)
            {
                throw new InvalidOperationException("User is already soft deleted or not found.");
            }

            int affectedRows = await _unitOfWork.SaveChangesWithTransactionAsync();
            if (affectedRows == 0)
            {
                throw new InvalidOperationException("Failed to update user status in database");
            }

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                RoleId = user.RoleId,
                IsActive = user.IsActive,
                EmailVerifiedAt = user.EmailVerifiedAt,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<(bool IsSuccess, string ResultOrError)> UpdateUserProfileAsync(int id, UpdateUserProfileRequest request)
        {
            if (request == null)
            {
                return (false, "Update data is required");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return (false, $"User with ID {id} not found");
            }

            // Update only fields that have values from request
            if (!string.IsNullOrEmpty(request.FullName))
            {
                user.FullName = request.FullName;
            }
            if (!string.IsNullOrEmpty(request.Email))
            {
                user.Email = request.Email;
            }
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                user.PhoneNumber = request.PhoneNumber;
            }
            if (!string.IsNullOrEmpty(request.AvatarUrl))
            {
                user.AvatarUrl = request.AvatarUrl;
            }

            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.UserRepository.Update(user);
            
            int affectedRows = await _unitOfWork.SaveChangesWithTransactionAsync();
            if (affectedRows == 0)
            {
                return (false, "Failed to update user profile");
            }

            return (true, "User profile updated successfully");
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _unitOfWork.UserRepository.GetAllUserAsync() ?? throw new KeyNotFoundException("No users found in database");
        }

        public async Task<string?> GetOnlyUserNameById(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            return user?.FullName;
        }

        public async Task UpdateUserAsync(UserResponse user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
                throw new NotFoundException($"User with ID {user.Id} not found.");

            existingUser.Balance = user.Balance;
            existingUser.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.UserRepository.UpdateAsync(existingUser);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task<bool> UpgradeToPremiumAsync(int userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            if (user.IsPremium == true)
                return false; // Already premium

            if (user.Balance < 10000)
                return false; // Insufficient balance

            user.Balance -= 10000;
            user.IsPremium = true;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            return true;
        }
    }
}