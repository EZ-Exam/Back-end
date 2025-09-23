using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.TestSystemServices
{
    public interface IUserQuestionCartService
    {
        Task<List<UserQuestionCartResponse>> GetUserCartAsync(int userId);
        Task<List<UserQuestionCartResponse>> GetSelectedItemsAsync(int userId);
        Task<bool> AddToCartAsync(AddToCartRequest request);
        Task<bool> RemoveFromCartAsync(int userId, int questionId);
        Task<bool> UpdateCartItemAsync(UpdateCartItemRequest request);
        Task<bool> ToggleSelectionAsync(int userId, int questionId);
        Task<bool> ClearCartAsync(int userId);
        Task<int> GetCartCountAsync(int userId);
        Task<List<UserQuestionCartResponse>> GetBySubjectAsync(int userId, int subjectId);
        Task<List<UserQuestionCartResponse>> GetByDifficultyAsync(int userId, string difficultyLevel);
        Task<bool> IsQuestionInCartAsync(int userId, int questionId);
    }

    public class UserQuestionCartService : IUserQuestionCartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserQuestionCartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<List<UserQuestionCartResponse>> GetUserCartAsync(int userId)
        {
            var cartItems = await _unitOfWork.UserQuestionCartRepository.GetByUserIdAsync(userId);
            return cartItems.Select(MapToResponse).ToList();
        }

        public async Task<List<UserQuestionCartResponse>> GetSelectedItemsAsync(int userId)
        {
            var selectedItems = await _unitOfWork.UserQuestionCartRepository.GetSelectedByUserIdAsync(userId);
            return selectedItems.Select(MapToResponse).ToList();
        }

        public async Task<bool> AddToCartAsync(AddToCartRequest request)
        {
            try
            {
                // Check if item already exists in cart
                var existingItem = await _unitOfWork.UserQuestionCartRepository.GetByUserAndQuestionAsync(request.UserId, request.QuestionId);
                if (existingItem != null)
                {
                    return false; // Item already in cart
                }

                var cartItem = new UserQuestionCart
                {
                    UserId = request.UserId,
                    QuestionId = request.QuestionId,
                    IsSelected = request.IsSelected,
                    UserNotes = request.UserNotes,
                    DifficultyPreference = request.DifficultyPreference,
                    SubjectId = request.SubjectId,
                    ChapterId = request.ChapterId,
                    LessonId = request.LessonId,
                    AddedAt = DateTime.UtcNow
                };

                await _unitOfWork.UserQuestionCartRepository.CreateAsync(cartItem);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int questionId)
        {
            try
            {
                var cartItem = await _unitOfWork.UserQuestionCartRepository.GetByUserAndQuestionAsync(userId, questionId);
                if (cartItem == null)
                {
                    return false;
                }

                await _unitOfWork.UserQuestionCartRepository.DeleteAsync(cartItem);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateCartItemAsync(UpdateCartItemRequest request)
        {
            try
            {
                var cartItem = await _unitOfWork.UserQuestionCartRepository.GetByUserAndQuestionAsync(request.UserId, request.QuestionId);
                if (cartItem == null)
                {
                    return false;
                }

                cartItem.IsSelected = request.IsSelected;
                cartItem.UserNotes = request.UserNotes;
                cartItem.DifficultyPreference = request.DifficultyPreference;

                await _unitOfWork.UserQuestionCartRepository.UpdateAsync(cartItem);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ToggleSelectionAsync(int userId, int questionId)
        {
            try
            {
                var cartItem = await _unitOfWork.UserQuestionCartRepository.GetByUserAndQuestionAsync(userId, questionId);
                if (cartItem == null)
                {
                    return false;
                }

                cartItem.IsSelected = !cartItem.IsSelected;
                await _unitOfWork.UserQuestionCartRepository.UpdateAsync(cartItem);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            try
            {
                var cartItems = await _unitOfWork.UserQuestionCartRepository.GetByUserIdAsync(userId);
                foreach (var item in cartItems)
                {
                    await _unitOfWork.UserQuestionCartRepository.DeleteAsync(item);
                }
                await _unitOfWork.SaveChangesWithTransactionAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetCartCountAsync(int userId)
        {
            return await _unitOfWork.UserQuestionCartRepository.GetCartCountAsync(userId);
        }

        public async Task<List<UserQuestionCartResponse>> GetBySubjectAsync(int userId, int subjectId)
        {
            var cartItems = await _unitOfWork.UserQuestionCartRepository.GetBySubjectAsync(userId, subjectId);
            return cartItems.Select(MapToResponse).ToList();
        }

        public async Task<List<UserQuestionCartResponse>> GetByDifficultyAsync(int userId, string difficultyLevel)
        {
            var cartItems = await _unitOfWork.UserQuestionCartRepository.GetByDifficultyAsync(userId, difficultyLevel);
            return cartItems.Select(MapToResponse).ToList();
        }

        public async Task<bool> IsQuestionInCartAsync(int userId, int questionId)
        {
            return await _unitOfWork.UserQuestionCartRepository.IsQuestionInCartAsync(userId, questionId);
        }

        private static UserQuestionCartResponse MapToResponse(UserQuestionCart cartItem)
        {
            return new UserQuestionCartResponse
            {
                Id = cartItem.Id,
                UserId = cartItem.UserId,
                QuestionId = cartItem.QuestionId,
                IsSelected = cartItem.IsSelected,
                UserNotes = cartItem.UserNotes,
                DifficultyPreference = cartItem.DifficultyPreference,
                SubjectId = cartItem.SubjectId,
                ChapterId = cartItem.ChapterId,
                LessonId = cartItem.LessonId,
                AddedAt = cartItem.AddedAt
            };
        }
    }
}
