using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Polly;
using Polly.Extensions.Http;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/gemini-15")]
    [Produces("application/json")]
    [Authorize] // Require authentication for AI access
    public class Gemini15Controller : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<Gemini15Controller> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public Gemini15Controller(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<Gemini15Controller> logger)
        {
            _logger = logger;
            _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini:ApiKey is not configured.");
            _baseUrl = "https://generativelanguage.googleapis.com/v1beta";
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests || msg.StatusCode == HttpStatusCode.RequestTimeout)
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(new Random().Next(0, 1000)));
        }

        private async Task<string> CallGeminiApiAsync(object requestBody, string model, bool useStream = false)
        {
            var jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            string endpoint = useStream ? $"/models/{model}:streamGenerateContent?key={_apiKey}" : $"/models/{model}:generateContent?key={_apiKey}";

            HttpResponseMessage response = await GetRetryPolicy().ExecuteAsync(async () =>
            {
                return await _httpClient.PostAsync($"{_baseUrl}{endpoint}", content);
            });

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Gemini API error: {StatusCode}, {Error}", response.StatusCode, errorContent);
                throw new HttpRequestException($"Error calling Gemini API: {response.StatusCode}, Details: {errorContent}");
            }

            if (useStream)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);
                var fullResponse = new StringBuilder();
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (!line.StartsWith("data: ")) continue;
                    var data = line.Substring(6).Trim();
                    if (data == "[DONE]") break;
                    var jsonNode = JsonNode.Parse(data);
                    var contentDelta = jsonNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.GetValue<string>();
                    if (!string.IsNullOrEmpty(contentDelta))
                    {
                        fullResponse.Append(contentDelta);
                    }
                }
                return fullResponse.ToString();
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Gemini API response: {ResponseBody}", responseBody);

                using var jsonDoc = JsonDocument.Parse(responseBody);
                return jsonDoc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? string.Empty;
            }
        }

        [HttpPost("solve")]
        [SwaggerOperation(Summary = "Solve physics problem with Gemini 1.5 API", Description = "Sends a physics problem to Gemini 1.5 for detailed step-by-step solution. Uses streaming for faster response, lower temperature for accuracy.")]
        [SwaggerResponse(200, "Physics solution retrieved successfully.", typeof(Gemini15ChatResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(500, "Error occurred while calling Gemini API.", typeof(object))]
        public async Task<IActionResult> Solve([FromBody] Gemini15ChatRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Invalid request data for Gemini 1.5 solve.");
                return BadRequest(new { Message = "Message is required." });
            }

            try
            {
                var systemPrompt = "You are an expert physicist specializing in precise, step-by-step solutions using Chain of Thought reasoning. Always provide clear, accurate, and concise explanations.";

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
                    contents = new[]
                    {
                        new { role = "model", parts = new[] { new { text = systemPrompt } } },
                        new { role = "user", parts = new[] { new { text = userPrompt } } }
                    },
                    generationConfig = new
                    {
                        maxOutputTokens = 8192,
                        temperature = 0.2
                    }
                };

                var responseText = await CallGeminiApiAsync(requestBody, "gemini-1.5-flash", useStream: true);

                if (string.IsNullOrWhiteSpace(responseText))
                {
                    _logger.LogWarning("Flash model returned empty response, falling back to Pro model.");
                    responseText = await CallGeminiApiAsync(requestBody, "gemini-1.5-pro", useStream: true);
                }

                var result = new Gemini15ChatResponse { Response = responseText };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Gemini 1.5 API solve request.");
                return StatusCode(500, new { Message = "An error occurred while processing the request." });
            }
        }

        [HttpPost("latex")]
        [SwaggerOperation(Summary = "Get LaTeX code for physics problem solution with Gemini 1.5", Description = "Sends a physics problem to Gemini 1.5 and returns only the LaTeX code for the solution.")]
        [SwaggerResponse(200, "LaTeX code retrieved successfully.", typeof(Gemini15ChatResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(500, "Error occurred while calling Gemini API.", typeof(object))]
        public async Task<IActionResult> Latex([FromBody] Gemini15ChatRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Invalid request data for Gemini 1.5 latex.");
                return BadRequest(new { Message = "Message is required." });
            }

            try
            {
                var systemPrompt = "You are an expert physicist providing pure LaTeX mathematical derivations without any text explanations.";

                var userPrompt = $@"For the given problem, provide ONLY the LaTeX code for the solution, including:
- All necessary formulas.
- Step-by-step derivations using LaTeX math mode.
- The final answer in \boxed{{}}.

Do NOT include any explanatory text or natural language.

Problem: {request.Message}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new { role = "model", parts = new[] { new { text = systemPrompt } } },
                        new { role = "user", parts = new[] { new { text = userPrompt } } }
                    },
                    generationConfig = new
                    {
                        maxOutputTokens = 8192,
                        temperature = 0.2
                    }
                };

                var responseText = await CallGeminiApiAsync(requestBody, "gemini-1.5-flash", useStream: true);

                if (string.IsNullOrWhiteSpace(responseText))
                {
                    _logger.LogWarning("Flash model returned empty response, falling back to Pro model.");
                    responseText = await CallGeminiApiAsync(requestBody, "gemini-1.5-pro", useStream: true);
                }

                var result = new Gemini15ChatResponse { Response = responseText };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Gemini 1.5 API latex request.");
                return StatusCode(500, new { Message = "An error occurred while processing the request." });
            }
        }

        [HttpPost("chat")]
        [SwaggerOperation(Summary = "General chat with Gemini 1.5 API", Description = "Sends a message to Gemini 1.5 for general response.")]
        [SwaggerResponse(200, "Chat response retrieved successfully.", typeof(Gemini15ChatResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(500, "Error occurred while calling Gemini API.", typeof(object))]
        public async Task<IActionResult> Chat([FromBody] Gemini15ChatRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Invalid request data for Gemini 1.5 chat.");
                return BadRequest(new { Message = "Message is required." });
            }

            try
            {
                var systemPrompt = "You are a helpful and accurate assistant.";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new { role = "model", parts = new[] { new { text = systemPrompt } } },
                        new { role = "user", parts = new[] { new { text = request.Message } } }
                    },
                    generationConfig = new
                    {
                        maxOutputTokens = 4096,
                        temperature = 0.3
                    }
                };

                var responseText = await CallGeminiApiAsync(requestBody, "gemini-1.5-flash", useStream: true);

                if (string.IsNullOrWhiteSpace(responseText))
                {
                    _logger.LogWarning("Flash model returned empty response, falling back to Pro model.");
                    responseText = await CallGeminiApiAsync(requestBody, "gemini-1.5-pro", useStream: true);
                }

                var result = new Gemini15ChatResponse { Response = responseText };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Gemini 1.5 API chat request.");
                return StatusCode(500, new { Message = "An error occurred while processing the request." });
            }
        }
    }
}
