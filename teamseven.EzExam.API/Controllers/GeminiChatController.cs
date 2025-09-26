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
    public class GeminiChatController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeminiChatController> _logger;

        // Cache key prefix for user sessions
        private const string SESSION_PREFIX = "gemini_chat_session_";
        private const int SESSION_TIMEOUT_MINUTES = 30;

        public GeminiChatController(
            IMemoryCache memoryCache,
            IConfiguration configuration,
            HttpClient httpClient,
            ILogger<GeminiChatController> logger)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Send a message to Gemini AI with session management
        /// </summary>
        /// <param name="request">Chat request containing message and optional sessionId</param>
        /// <returns>AI response with session information</returns>
        [HttpPost("chat")]
        [SwaggerOperation(
            Summary = "Chat with Gemini AI",
            Description = "Send a message to Gemini AI with automatic session management. " +
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

                // Prepare Gemini API request
                var geminiRequest = new GeminiRequest
                {
                    Contents = conversationHistory.Select(msg => new GeminiContent
                    {
                        Role = msg.Role == "user" ? "user" : "model",
                        Parts = new List<GeminiPart>
                        {
                            new GeminiPart { Text = msg.Content }
                        }
                    }).ToList(),
                    GenerationConfig = new GeminiGenerationConfig
                    {
                        Temperature = 0.7,
                        TopK = 40,
                        TopP = 0.95,
                        MaxOutputTokens = 1024
                    }
                };

                // Call Gemini API
                var response = await CallGeminiAPI(geminiRequest);
                if (response == null)
                {
                    return StatusCode(500, new { message = "Failed to get response from Gemini AI" });
                }

                // Add AI response to conversation history
                conversationHistory.Add(new ChatMessage
                {
                    Role = "model",
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
                _logger.LogError(ex, "Error in Gemini chat endpoint");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get conversation history for a specific session
        /// </summary>
        /// <param name="sessionId">Session ID to retrieve history for</param>
        /// <returns>Conversation history</returns>
        [HttpGet("history/{sessionId}")]
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
        /// List available Gemini models
        /// </summary>
        /// <returns>List of available models</returns>
        [HttpGet("models")]
        [SwaggerOperation(
            Summary = "List available Gemini models",
            Description = "Retrieve a list of all available Gemini AI models that can be used for chat. " +
                         "Useful for checking which models are accessible with your API key."
        )]
        [SwaggerResponse(200, "Models list retrieved successfully", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing JWT token", typeof(object))]
        [SwaggerResponse(500, "Failed to fetch models", typeof(object))]
        public async Task<IActionResult> ListModels()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var apiKey = _configuration["Gemini:ApiKey"];
                var baseUrl = _configuration["Gemini:BaseUrl"];

                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(baseUrl))
                {
                    return BadRequest(new { message = "Gemini API configuration missing" });
                }

                var url = $"{baseUrl}/models?key={apiKey}";
                _logger.LogInformation("Fetching models from: {Url}", url);

                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Models response: {Content}", content);
                    
                    var modelsResponse = JsonSerializer.Deserialize<ModelsResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    return Ok(new 
                    { 
                        message = "Models retrieved successfully",
                        models = modelsResponse?.Models?.Select(m => new 
                        {
                            name = m.Name,
                            displayName = m.DisplayName,
                            description = m.Description,
                            supportedGenerationMethods = m.SupportedGenerationMethods
                        }).ToList(),
                        timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error fetching models: {StatusCode} - {Content}", response.StatusCode, errorContent);
                    return StatusCode(500, new { message = "Failed to fetch models", error = errorContent });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing Gemini models");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Test Gemini API connection and list available models
        /// </summary>
        /// <returns>Available models and connection status</returns>
        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var apiKey = _configuration["Gemini:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    return BadRequest(new { message = "Gemini API key not configured" });
                }

                // Test with a simple request
                var testRequest = new GeminiRequest
                {
                    Contents = new List<GeminiContent>
                    {
                        new GeminiContent
                        {
                            Role = "user",
                            Parts = new List<GeminiPart>
                            {
                                new GeminiPart { Text = "Hello, this is a test message." }
                            }
                        }
                    },
                    GenerationConfig = new GeminiGenerationConfig
                    {
                        Temperature = 0.1,
                        MaxOutputTokens = 50
                    }
                };

                var response = await CallGeminiAPI(testRequest);
                
                if (response != null)
                {
                    return Ok(new 
                    { 
                        message = "Gemini API connection successful",
                        testResponse = response,
                        timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Gemini API connection failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing Gemini API connection");
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

        private async Task<string?> CallGeminiAPI(GeminiRequest request)
        {
            try
            {
                var apiKey = _configuration["Gemini:ApiKey"];
                var baseUrl = _configuration["Gemini:BaseUrl"];
                var model = _configuration["Gemini:Model"];

                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(model))
                {
                    _logger.LogError("Gemini API configuration is missing");
                    return null;
                }

                // Build the correct URL format for Gemini API
                var url = $"{baseUrl}/models/{model}:generateContent?key={apiKey}";

                _logger.LogInformation("Gemini API Configuration:");
                _logger.LogInformation("- BaseUrl: {BaseUrl}", baseUrl);
                _logger.LogInformation("- Model: {Model}", model);
                _logger.LogInformation("- API Key: {ApiKey}", apiKey?.Substring(0, 10) + "...");
                _logger.LogInformation("- Full URL: {Url}", url);

                // Create the request payload in the correct format
                var payload = new
                {
                    contents = request.Contents.Select(c => new
                    {
                        role = c.Role,
                        parts = c.Parts.Select(p => new { text = p.Text }).ToArray()
                    }).ToArray(),
                    generationConfig = new
                    {
                        temperature = request.GenerationConfig.Temperature,
                        topK = request.GenerationConfig.TopK,
                        topP = request.GenerationConfig.TopP,
                        maxOutputTokens = request.GenerationConfig.MaxOutputTokens
                    }
                };

                var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                _logger.LogInformation("Sending request to Gemini API: {Url}", url);
                _logger.LogInformation("Request payload: {Json}", json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Add timeout to prevent hanging requests
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var response = await _httpClient.PostAsync(url, content, cts.Token);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                    
                    // Handle rate limiting with retry
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        _logger.LogWarning("Rate limit exceeded, retrying in 8 seconds...");
                        await Task.Delay(8000); // Wait 8 seconds
                        
                        // Retry once
                        response = await _httpClient.PostAsync(url, content, cts.Token);
                        if (!response.IsSuccessStatusCode)
                        {
                            var retryErrorContent = await response.Content.ReadAsStringAsync();
                            _logger.LogError("Gemini API retry failed: {StatusCode} - {Content}", response.StatusCode, retryErrorContent);
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Gemini API response: {Response}", responseContent);

                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var result = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
                _logger.LogInformation("Successfully received response from Gemini API");
                return result;
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Gemini API request timed out");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Gemini API");
                return null;
            }
        }

        #endregion
    }

    #region Request/Response Models

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
        public string? SessionId { get; set; }
    }

    public class ChatResponse
    {
        public string Message { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int MessageCount { get; set; }
    }

    public class ChatMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class GeminiRequest
    {
        public List<GeminiContent> Contents { get; set; } = new();
        public GeminiGenerationConfig GenerationConfig { get; set; } = new();
    }

    public class GeminiContent
    {
        public string Role { get; set; } = string.Empty;
        public List<GeminiPart> Parts { get; set; } = new();
    }

    public class GeminiPart
    {
        public string Text { get; set; } = string.Empty;
    }

    public class GeminiGenerationConfig
    {
        public double Temperature { get; set; }
        public int TopK { get; set; }
        public double TopP { get; set; }
        public int MaxOutputTokens { get; set; }
    }

    public class GeminiResponse
    {
        public List<GeminiCandidate>? Candidates { get; set; }
    }

    public class GeminiCandidate
    {
        public GeminiContent? Content { get; set; }
    }

    public class ModelsResponse
    {
        public List<GeminiModel>? Models { get; set; }
    }

    public class GeminiModel
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> SupportedGenerationMethods { get; set; } = new();
    }

    #endregion
}
