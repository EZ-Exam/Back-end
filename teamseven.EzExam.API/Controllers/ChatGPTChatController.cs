using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatGPTChatController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ChatGPTChatController> _logger;

        // Cache key prefix for user sessions
        private const string SESSION_PREFIX = "chatgpt_chat_session_";
        private const int SESSION_TIMEOUT_MINUTES = 30;

        public ChatGPTChatController(
            IMemoryCache memoryCache,
            IConfiguration configuration,
            HttpClient httpClient,
            ILogger<ChatGPTChatController> logger)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Send a message to ChatGPT with session management
        /// </summary>
        /// <param name="request">Chat request containing message and optional sessionId</param>
        /// <returns>AI response with session information</returns>
        [HttpPost("chat")]
        [SwaggerOperation(
            Summary = "Chat with ChatGPT AI",
            Description = "Send a message to ChatGPT AI with automatic session management. " +
                         "For first-time chat, omit sessionId to create a new session. " +
                         "For continuing conversation, include sessionId to maintain context."
        )]
        [SwaggerResponse(200, "Chat response received successfully", typeof(ChatResponse))]
        [SwaggerResponse(400, "Invalid request data", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing JWT token", typeof(object))]
        [SwaggerResponse(500, "Internal server error", typeof(object))]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            try
            {
                // Get user ID from JWT token
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                // Validate request
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return BadRequest(new { message = "Message cannot be empty" });
                }

                // Get or create session
                var sessionId = request.SessionId ?? Guid.NewGuid().ToString();
                var sessionKey = $"{SESSION_PREFIX}{userId}_{sessionId}";
                
                // Get conversation history from cache (empty if new session)
                var conversationHistory = GetConversationHistory(sessionKey);
                
                // Log session info for debugging
                _logger.LogInformation("Session Info - UserId: {UserId}, SessionId: {SessionId}, HistoryCount: {Count}", 
                    userId, sessionId, conversationHistory.Count);

                // Add user message to history
                conversationHistory.Add(new ChatMessage
                {
                    Role = "user",
                    Content = request.Message,
                    Timestamp = DateTime.UtcNow
                });

                // Prepare ChatGPT API request
                var chatGPTRequest = new ChatGPTRequest
                {
                    Model = _configuration["OpenAI:Model"] ?? "gpt-3.5-turbo",
                    Messages = conversationHistory.Select(msg => new ChatGPTMessage
                    {
                        Role = msg.Role,
                        Content = msg.Content
                    }).ToList(),
                    MaxTokens = 1024,
                    Temperature = 0.7,
                    Stream = false
                };

                // Call ChatGPT API
                var response = await CallChatGPTAPI(chatGPTRequest);
                if (response == null)
                {
                    return StatusCode(500, new { message = "Failed to get response from ChatGPT" });
                }

                // Add AI response to conversation history
                conversationHistory.Add(new ChatMessage
                {
                    Role = "assistant",
                    Content = response,
                    Timestamp = DateTime.UtcNow
                });

                // Save updated conversation history to cache
                SaveConversationHistory(sessionKey, conversationHistory);

                // Return response
                return Ok(new ChatResponse
                {
                    Message = response,
                    SessionId = sessionId,
                    Timestamp = DateTime.UtcNow,
                    MessageCount = conversationHistory.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ChatGPT chat endpoint");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get conversation history for a specific session
        /// </summary>
        /// <param name="sessionId">Session ID to retrieve history for</param>
        /// <returns>Conversation history</returns>
        [HttpGet("history/{sessionId}")]
        [SwaggerOperation(
            Summary = "Get conversation history",
            Description = "Retrieve the complete conversation history for a specific session. " +
                         "Returns all messages in chronological order with timestamps."
        )]
        [SwaggerResponse(200, "Conversation history retrieved successfully", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing JWT token", typeof(object))]
        [SwaggerResponse(500, "Internal server error", typeof(object))]
        public IActionResult GetHistory(string sessionId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var sessionKey = $"{SESSION_PREFIX}{userId}_{sessionId}";
                var conversationHistory = GetConversationHistory(sessionKey);

                return Ok(new
                {
                    SessionId = sessionId,
                    Messages = conversationHistory,
                    MessageCount = conversationHistory.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conversation history");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Clear conversation history for a specific session
        /// </summary>
        /// <param name="sessionId">Session ID to clear</param>
        /// <returns>Success status</returns>
        [HttpDelete("history/{sessionId}")]
        [SwaggerOperation(
            Summary = "Clear conversation history",
            Description = "Permanently delete all conversation history for a specific session. " +
                         "This action cannot be undone. The session will start fresh on next chat."
        )]
        [SwaggerResponse(200, "Conversation history cleared successfully", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing JWT token", typeof(object))]
        [SwaggerResponse(500, "Internal server error", typeof(object))]
        public IActionResult ClearHistory(string sessionId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var sessionKey = $"{SESSION_PREFIX}{userId}_{sessionId}";
                _memoryCache.Remove(sessionKey);

                return Ok(new { message = "Conversation history cleared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing conversation history");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get all active sessions for the current user
        /// </summary>
        /// <returns>List of active sessions</returns>
        [HttpGet("sessions")]
        public IActionResult GetActiveSessions()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                // Note: This is a simplified implementation
                // In a production environment, you might want to store session metadata separately
                return Ok(new { message = "Active sessions feature not fully implemented" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active sessions");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Test ChatGPT API connection
        /// </summary>
        /// <returns>Connection status</returns>
        [HttpGet("test-connection")]
        [SwaggerOperation(
            Summary = "Test ChatGPT API connection",
            Description = "Test the connection to ChatGPT API with a simple message. " +
                         "Useful for debugging and verifying API configuration."
        )]
        [SwaggerResponse(200, "API connection successful", typeof(object))]
        [SwaggerResponse(400, "API configuration missing", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing JWT token", typeof(object))]
        [SwaggerResponse(500, "API connection failed", typeof(object))]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var apiKey = _configuration["OpenAI:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    return BadRequest(new { message = "OpenAI API key not configured" });
                }

                // Test with a simple request
                var testRequest = new ChatGPTRequest
                {
                    Model = _configuration["OpenAI:Model"] ?? "gpt-3.5-turbo",
                    Messages = new List<ChatGPTMessage>
                    {
                        new ChatGPTMessage
                        {
                            Role = "user",
                            Content = "Hello, this is a test message."
                        }
                    },
                    MaxTokens = 50,
                    Temperature = 0.1,
                    Stream = false
                };

                var response = await CallChatGPTAPI(testRequest);
                
                if (response != null)
                {
                    return Ok(new 
                    { 
                        message = "ChatGPT API connection successful",
                        testResponse = response,
                        model = testRequest.Model,
                        timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "ChatGPT API connection failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing ChatGPT API connection");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        #region Private Methods

        private string GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                userIdClaim = User.FindFirst("userId")?.Value;
            }
            return userIdClaim;
        }

        private List<ChatMessage> GetConversationHistory(string sessionKey)
        {
            if (_memoryCache.TryGetValue(sessionKey, out List<ChatMessage>? history))
            {
                return history ?? new List<ChatMessage>();
            }
            return new List<ChatMessage>();
        }

        private void SaveConversationHistory(string sessionKey, List<ChatMessage> history)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(SESSION_TIMEOUT_MINUTES),
                SlidingExpiration = TimeSpan.FromMinutes(10), // Reset timeout on access
                Priority = CacheItemPriority.Normal,
                Size = history.Count // Add size for cache entry
            };

            _memoryCache.Set(sessionKey, history, cacheOptions);
        }

        private async Task<string?> CallChatGPTAPI(ChatGPTRequest request)
        {
            try
            {
                var apiKey = _configuration["OpenAI:ApiKey"];
                var baseUrl = _configuration["OpenAI:BaseUrl"];

                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(baseUrl))
                {
                    _logger.LogError("OpenAI API configuration is missing");
                    return null;
                }

                var url = $"{baseUrl}/chat/completions";

                _logger.LogInformation("ChatGPT API Configuration:");
                _logger.LogInformation("- BaseUrl: {BaseUrl}", baseUrl);
                _logger.LogInformation("- Model: {Model}", request.Model);
                _logger.LogInformation("- API Key: {ApiKey}", apiKey?.Substring(0, 10) + "...");
                _logger.LogInformation("- Full URL: {Url}", url);

                var payload = new
                {
                    model = request.Model,
                    messages = request.Messages.Select(m => new
                    {
                        role = m.Role,
                        content = m.Content
                    }).ToArray(),
                    max_tokens = request.MaxTokens,
                    temperature = request.Temperature,
                    stream = request.Stream
                };

                var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });

                _logger.LogInformation("Sending request to ChatGPT API: {Url}", url);
                _logger.LogInformation("Request payload: {Json}", json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                // Add timeout to prevent hanging requests
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var response = await _httpClient.PostAsync(url, content, cts.Token);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("ChatGPT API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                    
                    // Handle rate limiting with retry
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        _logger.LogWarning("Rate limit exceeded, retrying in 5 seconds...");
                        await Task.Delay(5000); // Wait 5 seconds
                        
                        // Retry once
                        response = await _httpClient.PostAsync(url, content, cts.Token);
                        if (!response.IsSuccessStatusCode)
                        {
                            var retryErrorContent = await response.Content.ReadAsStringAsync();
                            _logger.LogError("ChatGPT API retry failed: {StatusCode} - {Content}", response.StatusCode, retryErrorContent);
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("ChatGPT API response: {Response}", responseContent);

                var chatGPTResponse = JsonSerializer.Deserialize<ChatGPTResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });

                var result = chatGPTResponse?.Choices?.FirstOrDefault()?.Message?.Content;
                _logger.LogInformation("Successfully received response from ChatGPT API");
                return result;
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("ChatGPT API request timed out");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling ChatGPT API");
                return null;
            }
        }

        #endregion
    }

    #region Request/Response Models

    /// <summary>
    /// Request model for ChatGPT API calls
    /// </summary>
    public class ChatGPTRequest
    {
        /// <summary>
        /// The model to use (e.g., gpt-3.5-turbo)
        /// </summary>
        public string Model { get; set; } = string.Empty;
        
        /// <summary>
        /// List of messages in the conversation
        /// </summary>
        public List<ChatGPTMessage> Messages { get; set; } = new();
        
        /// <summary>
        /// Maximum number of tokens to generate
        /// </summary>
        public int MaxTokens { get; set; } = 1024;
        
        /// <summary>
        /// Temperature for response randomness (0.0 to 1.0)
        /// </summary>
        public double Temperature { get; set; } = 0.7;
        
        /// <summary>
        /// Whether to stream the response
        /// </summary>
        public bool Stream { get; set; } = false;
    }

    /// <summary>
    /// Individual message in ChatGPT conversation
    /// </summary>
    public class ChatGPTMessage
    {
        /// <summary>
        /// Role of the message sender (user, assistant, system)
        /// </summary>
        public string Role { get; set; } = string.Empty;
        
        /// <summary>
        /// Content of the message
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response model from ChatGPT API
    /// </summary>
    public class ChatGPTResponse
    {
        /// <summary>
        /// List of response choices from ChatGPT
        /// </summary>
        public List<ChatGPTChoice>? Choices { get; set; }
    }

    /// <summary>
    /// Individual choice in ChatGPT response
    /// </summary>
    public class ChatGPTChoice
    {
        /// <summary>
        /// The message content from ChatGPT
        /// </summary>
        public ChatGPTMessage? Message { get; set; }
    }

    #endregion
}
