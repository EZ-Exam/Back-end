using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Polly;
using Polly.Extensions.Http;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/deepseek")]
    [Produces("application/json")]
    [Authorize] // Require authentication for AI access
    public class DeepSeekController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DeepSeekController> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public DeepSeekController(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<DeepSeekController> logger)
        {
            _logger = logger;
            _apiKey = configuration["DeepSeek:ApiKey"] ?? throw new ArgumentNullException("DeepSeek:ApiKey is not configured.");
            _baseUrl = configuration["DeepSeek:BaseUrl"] ?? "https://api.deepseek.com";
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(10); // Keep high timeout, but handle internally
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests || msg.StatusCode == HttpStatusCode.RequestTimeout)
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(new Random().Next(0, 1000)), // Exponential backoff with jitter
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds due to {outcome.Result?.StatusCode}");
                    });
        }

        private async Task<string> CallDeepSeekApiAsync(object requestBody, bool useStream = false)
        {
            var jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await GetRetryPolicy().ExecuteAsync(async () =>
            {
                return await _httpClient.PostAsync($"{_baseUrl}/chat/completions", content);
            });

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("DeepSeek API error: {StatusCode}, {Error}", response.StatusCode, errorContent);
                throw new HttpRequestException($"Error calling DeepSeek API: {response.StatusCode}, Details: {errorContent}");
            }

            if (useStream)
            {
                // For simplicity, collect streamed response into string; in production, yield chunks
                var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);
                var fullResponse = new StringBuilder();
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (line.StartsWith("data: "))
                    {
                        var data = line.Substring(6).Trim();
                        if (data == "[DONE]") break;
                        var jsonNode = JsonNode.Parse(data);
                        var contentDelta = jsonNode?["choices"]?[0]?["delta"]?["content"]?.GetValue<string>();
                        if (!string.IsNullOrEmpty(contentDelta))
                        {
                            fullResponse.Append(contentDelta);
                        }
                    }
                }
                return fullResponse.ToString();
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("DeepSeek API response: {ResponseBody}", responseBody);

                using var jsonDoc = JsonDocument.Parse(responseBody);
                if (jsonDoc.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    return jsonDoc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString() ?? string.Empty;
                }

                return string.Empty;
            }
        }

        [HttpPost("solve")]
        [SwaggerOperation(Summary = "Solve physics problem with DeepSeek API", Description = "Sends a physics problem to DeepSeek API for detailed step-by-step solution. Uses streaming for faster response, lower temperature for accuracy, and fallback if empty.")]
        [SwaggerResponse(200, "Physics solution retrieved successfully.", typeof(DeepSeekChatResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(500, "Error occurred while calling DeepSeek API.", typeof(object))]
        public async Task<IActionResult> Solve([FromBody] DeepSeekChatRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Invalid request data for DeepSeek solve.");
                return BadRequest(new { Message = "Message is required." });
            }

            try
            {
                // Improved prompt with system role for better accuracy
                var systemPrompt = @"You are an expert physicist specializing in precise, step-by-step solutions using Chain of Thought reasoning. Always provide clear, accurate, and concise explanations.";

                var userPrompt = $@"For the given problem, follow these steps:
1. Restate the problem clearly in English.
2. List all given data and assumptions.
3. Derive the necessary formulas.
4. Solve step by step, explaining each calculation.
5. Verify the solution against all conditions.
6. Provide the final answer in LaTeX format within \boxed{{}}.

Ensure all mathematical expressions are in LaTeX and use clear English.

Problem: {request.Message}";

                var requestBody = new
                {
                    model = "deepseek-reasoner",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userPrompt }
                    },
                    max_tokens = 8192, // Increased for complex problems, based on docs up to 32k-64k
                    temperature = 0.2, // Lowered significantly for higher accuracy and consistency
                    stream = true // Enable streaming to reduce perceived latency
                };

                _logger.LogInformation("DeepSeek API solve request body: {RequestBody}", JsonSerializer.Serialize(requestBody));

                var responseText = await CallDeepSeekApiAsync(requestBody, useStream: true);

                // Fallback to chat model if response is empty
                if (string.IsNullOrWhiteSpace(responseText))
                {
                    _logger.LogWarning("Reasoner model returned empty response, falling back to chat model.");

                    var fallbackSystemPrompt = "You are a helpful physics assistant providing step-by-step solutions.";
                    var fallbackUserPrompt = $"Solve this physics problem step by step: {request.Message}";

                    var fallbackBody = new
                    {
                        model = "deepseek-chat",
                        messages = new[]
                        {
                            new { role = "system", content = fallbackSystemPrompt },
                            new { role = "user", content = fallbackUserPrompt }
                        },
                        max_tokens = 4096,
                        temperature = 0.3, // Slightly higher for chat but still low for accuracy
                        stream = true
                    };

                    responseText = await CallDeepSeekApiAsync(fallbackBody, useStream: true);
                }

                var result = new DeepSeekChatResponse { Response = responseText };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing DeepSeek API solve request.");
                return StatusCode(500, new { Message = "An error occurred while processing the request." });
            }
        }

        [HttpPost("latex")]
        [SwaggerOperation(Summary = "Get LaTeX code for physics problem solution", Description = "Sends a physics problem to DeepSeek API and returns only the LaTeX code for the solution. Uses streaming, lower temperature, and fallback if empty.")]
        [SwaggerResponse(200, "LaTeX code retrieved successfully.", typeof(DeepSeekChatResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(500, "Error occurred while calling DeepSeek API.", typeof(object))]
        public async Task<IActionResult> Latex([FromBody] DeepSeekChatRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Invalid request data for DeepSeek latex.");
                return BadRequest(new { Message = "Message is required." });
            }

            try
            {
                // Improved prompt with system role
                var systemPrompt = @"You are an expert physicist providing pure LaTeX mathematical derivations without any text explanations.";

                var userPrompt = $@"For the given problem, provide ONLY the LaTeX code for the solution, including:
- All necessary formulas.
- Step-by-step derivations using LaTeX math mode.
- The final answer in \boxed{{}}.

Do NOT include any explanatory text or natural language.

Problem: {request.Message}";

                var requestBody = new
                {
                    model = "deepseek-reasoner",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userPrompt }
                    },
                    max_tokens = 8192,
                    temperature = 0.2,
                    stream = true
                };

                _logger.LogInformation("DeepSeek API latex request body: {RequestBody}", JsonSerializer.Serialize(requestBody));

                var responseText = await CallDeepSeekApiAsync(requestBody, useStream: true);

                // Fallback
                if (string.IsNullOrWhiteSpace(responseText))
                {
                    _logger.LogWarning("Reasoner model returned empty response, falling back to chat model.");

                    var fallbackSystemPrompt = "You are a LaTeX expert for physics solutions.";
                    var fallbackUserPrompt = $"Provide only the LaTeX code to solve this physics problem: {request.Message}";

                    var fallbackBody = new
                    {
                        model = "deepseek-chat",
                        messages = new[]
                        {
                            new { role = "system", content = fallbackSystemPrompt },
                            new { role = "user", content = fallbackUserPrompt }
                        },
                        max_tokens = 4096,
                        temperature = 0.3,
                        stream = true
                    };

                    responseText = await CallDeepSeekApiAsync(fallbackBody, useStream: true);
                }

                var result = new DeepSeekChatResponse { Response = responseText };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing DeepSeek API latex request.");
                return StatusCode(500, new { Message = "An error occurred while processing the request." });
            }
        }

        [HttpPost("chat")]
        [SwaggerOperation(Summary = "General chat with DeepSeek API", Description = "Sends a message to DeepSeek chat model for general response. Uses streaming and low temperature for consistency.")]
        [SwaggerResponse(200, "Chat response retrieved successfully.", typeof(DeepSeekChatResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(500, "Error occurred while calling DeepSeek API.", typeof(object))]
        public async Task<IActionResult> Chat([FromBody] DeepSeekChatRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Invalid request data for DeepSeek chat.");
                return BadRequest(new { Message = "Message is required." });
            }

            try
            {
                var systemPrompt = "You are a helpful and accurate assistant.";

                var requestBody = new
                {
                    model = "deepseek-chat",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = request.Message }
                    },
                    max_tokens = 4096,
                    temperature = 0.3, // Balanced for general chat with improved accuracy
                    stream = true
                };

                _logger.LogInformation("DeepSeek API chat request body: {RequestBody}", JsonSerializer.Serialize(requestBody));

                var responseText = await CallDeepSeekApiAsync(requestBody, useStream: true);

                var result = new DeepSeekChatResponse { Response = responseText };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing DeepSeek API chat request.");
                return StatusCode(500, new { Message = "An error occurred while processing the request." });
            }
        }
    }
}
