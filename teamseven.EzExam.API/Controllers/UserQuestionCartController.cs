using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using teamseven.EzExam.Services.Services.ServiceProvider;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class UserQuestionCartController : ControllerBase
    {
        private readonly IServiceProviders _serviceProviders;
        private readonly ILogger<UserQuestionCartController> _logger;

        public UserQuestionCartController(IServiceProviders serviceProviders, ILogger<UserQuestionCartController> logger)
        {
            _serviceProviders = serviceProviders ?? throw new ArgumentNullException(nameof(serviceProviders));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get user's question cart
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of cart items</returns>
        [HttpGet("{userId}")]
        [SwaggerOperation(Summary = "Get user's question cart", Description = "Retrieves all items in the user's question cart")]
        [SwaggerResponse(200, "Successfully retrieved cart items", typeof(List<UserQuestionCartResponse>))]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetUserCart(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var cartItems = await _serviceProviders.UserQuestionCartService.GetUserCartAsync(userId);
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user cart for user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while retrieving the cart" });
            }
        }

        /// <summary>
        /// Get selected items from user's cart
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of selected cart items</returns>
        [HttpGet("{userId}/selected")]
        [SwaggerOperation(Summary = "Get selected cart items", Description = "Retrieves only selected items from the user's question cart")]
        [SwaggerResponse(200, "Successfully retrieved selected items", typeof(List<UserQuestionCartResponse>))]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetSelectedItems(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var selectedItems = await _serviceProviders.UserQuestionCartService.GetSelectedItemsAsync(userId);
                return Ok(selectedItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving selected items for user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while retrieving selected items" });
            }
        }

        /// <summary>
        /// Add question to cart
        /// </summary>
        /// <param name="request">Add to cart request</param>
        /// <returns>Success status</returns>
        [HttpPost("add")]
        [SwaggerOperation(Summary = "Add question to cart", Description = "Adds a question to the user's question cart")]
        [SwaggerResponse(200, "Question added successfully")]
        [SwaggerResponse(400, "Invalid request data")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _serviceProviders.UserQuestionCartService.AddToCartAsync(request);
                if (result)
                {
                    return Ok(new { message = "Question added to cart successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to add question to cart or question already exists" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding question to cart for user {UserId}", request.UserId);
                return StatusCode(500, new { message = "An error occurred while adding question to cart" });
            }
        }

        /// <summary>
        /// Remove question from cart
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="questionId">Question ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{userId}/{questionId}")]
        [SwaggerOperation(Summary = "Remove question from cart", Description = "Removes a question from the user's question cart")]
        [SwaggerResponse(200, "Question removed successfully")]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> RemoveFromCart(int userId, int questionId)
        {
            try
            {
                if (userId <= 0 || questionId <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID or question ID" });
                }

                var result = await _serviceProviders.UserQuestionCartService.RemoveFromCartAsync(userId, questionId);
                if (result)
                {
                    return Ok(new { message = "Question removed from cart successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to remove question from cart or item not found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing question from cart for user {UserId}, question {QuestionId}", userId, questionId);
                return StatusCode(500, new { message = "An error occurred while removing question from cart" });
            }
        }

        /// <summary>
        /// Update cart item
        /// </summary>
        /// <param name="request">Update cart item request</param>
        /// <returns>Success status</returns>
        [HttpPut("update")]
        [SwaggerOperation(Summary = "Update cart item", Description = "Updates a cart item's properties")]
        [SwaggerResponse(200, "Cart item updated successfully")]
        [SwaggerResponse(400, "Invalid request data")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _serviceProviders.UserQuestionCartService.UpdateCartItemAsync(request);
                if (result)
                {
                    return Ok(new { message = "Cart item updated successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to update cart item or item not found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item for user {UserId}, question {QuestionId}", request.UserId, request.QuestionId);
                return StatusCode(500, new { message = "An error occurred while updating cart item" });
            }
        }

        /// <summary>
        /// Toggle selection of cart item
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="questionId">Question ID</param>
        /// <returns>Success status</returns>
        [HttpPut("{userId}/{questionId}/toggle")]
        [SwaggerOperation(Summary = "Toggle cart item selection", Description = "Toggles the selection status of a cart item")]
        [SwaggerResponse(200, "Selection toggled successfully")]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> ToggleSelection(int userId, int questionId)
        {
            try
            {
                if (userId <= 0 || questionId <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID or question ID" });
                }

                var result = await _serviceProviders.UserQuestionCartService.ToggleSelectionAsync(userId, questionId);
                if (result)
                {
                    return Ok(new { message = "Selection toggled successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to toggle selection or item not found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling selection for user {UserId}, question {QuestionId}", userId, questionId);
                return StatusCode(500, new { message = "An error occurred while toggling selection" });
            }
        }

        /// <summary>
        /// Clear user's cart
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{userId}/clear")]
        [SwaggerOperation(Summary = "Clear user's cart", Description = "Removes all items from the user's question cart")]
        [SwaggerResponse(200, "Cart cleared successfully")]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var result = await _serviceProviders.UserQuestionCartService.ClearCartAsync(userId);
                if (result)
                {
                    return Ok(new { message = "Cart cleared successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to clear cart" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while clearing cart" });
            }
        }

        /// <summary>
        /// Get cart count
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Cart count</returns>
        [HttpGet("{userId}/count")]
        [SwaggerOperation(Summary = "Get cart count", Description = "Gets the number of selected items in the user's cart")]
        [SwaggerResponse(200, "Successfully retrieved cart count", typeof(int))]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetCartCount(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var count = await _serviceProviders.UserQuestionCartService.GetCartCountAsync(userId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart count for user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while getting cart count" });
            }
        }

        /// <summary>
        /// Check if question is in cart
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="questionId">Question ID</param>
        /// <returns>Boolean indicating if question is in cart</returns>
        [HttpGet("{userId}/{questionId}/exists")]
        [SwaggerOperation(Summary = "Check if question is in cart", Description = "Checks if a specific question exists in the user's cart")]
        [SwaggerResponse(200, "Successfully checked cart status", typeof(bool))]
        [SwaggerResponse(400, "Invalid request parameters")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> IsQuestionInCart(int userId, int questionId)
        {
            try
            {
                if (userId <= 0 || questionId <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID or question ID" });
                }

                var exists = await _serviceProviders.UserQuestionCartService.IsQuestionInCartAsync(userId, questionId);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if question is in cart for user {UserId}, question {QuestionId}", userId, questionId);
                return StatusCode(500, new { message = "An error occurred while checking cart status" });
            }
        }
    }
}
