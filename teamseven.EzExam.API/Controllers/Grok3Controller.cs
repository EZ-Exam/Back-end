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
    [Route("api/grok-3")]
    [Produces("application/json")]
    public class Grok3Controller : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<Grok3Controller> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public Grok3Controller(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<Grok3Controller> logger)
        {
            _logger = logger;
            _apiKey = configuration["xAI:ApiKey"] ?? throw new ArgumentNullException("xAI:ApiKey is not configured.");
            _baseUrl = "https://api.x.ai/v1";
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests || msg.StatusCode == HttpStatusCode.RequestTimeout)
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(new Random().Next(0, 1000)));
        }

        private async Task<string> CallGrokApiAsync(object requestBody, bool useStream = false)
        {
            var jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            string endpoint = "/chat/completions";

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}{endpoint}")
            {
                Content = content
            };

            if (useStream)
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));
            }

            HttpResponseMessage response = await GetRetryPolicy().ExecuteAsync(async () =>
            {
                return await _httpClient.SendAsync(request);
            });

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Grok API error: {StatusCode}, {Error}", response.StatusCode, errorContent);
                throw new HttpRequestException($"Error calling Grok API: {response.StatusCode}, Details: {errorContent}");
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
                    var contentDelta = jsonNode?["choices"]?[0]?["delta"]?["content"]?.GetValue<string>();
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
                _logger.LogInformation("Grok API response: {ResponseBody}", responseBody);

                using var jsonDoc = JsonDocument.Parse(responseBody);
                return jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;
            }
        }

        [HttpPost("solve")]
        [SwaggerOperation(Summary = "Solve physics problem with Grok-3 API", Description = "Sends a physics problem to Grok-3 for detailed step-by-step solution. Uses streaming for faster response, lower temperature for accuracy.")]
        [SwaggerResponse(200, "Physics solution retrieved successfully.", typeof(Gemini15ChatResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(500, "Error occurred while calling Grok API.", typeof(object))]
        public async Task<IActionResult> Solve([FromBody] Gemini15ChatRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Invalid request data for Grok-3 solve.");
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
                    model = "grok-3",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userPrompt }
                    },
                    max_tokens = 8192,
                    temperature = 0.2,
                    stream = true
                };

                var responseText = await CallGrokApiAsync(requestBody, useStream: true);

                var result = new Gemini15ChatResponse { Response = responseText };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Grok-3 API solve request.");
                return StatusCode(500, new { Message = "An error occurred while processing the request." });
            }
        }

        [HttpPost("latex")]
        [SwaggerOperation(Summary = "Get LaTeX code for physics problem solution with Grok-3", Description = "Sends a physics problem to Grok-3 and returns only the LaTeX code for the solution.")]
        [SwaggerResponse(200, "LaTeX code retrieved successfully.", typeof(Gemini15ChatResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(500, "Error occurred while calling Grok API.", typeof(object))]
        public async Task<IActionResult> Latex([FromBody] Gemini15ChatRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Invalid request data for Grok-3 latex.");
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
                    model = "grok-3",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userPrompt }
                    },
                    max_tokens = 8192,
                    temperature = 0.2,
                    stream = true
                };

                var responseText = await CallGrokApiAsync(requestBody, useStream: true);

                var result = new Gemini15ChatResponse { Response = responseText };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Grok-3 API latex request.");
                return StatusCode(500, new { Message = "An error occurred while processing the request." });
            }
        }

        [HttpPost("chat")]
        [SwaggerOperation(Summary = "General chat with Grok-3 API", Description = "Sends a message to Grok-3 for general response.")]
        [SwaggerResponse(200, "Chat response retrieved successfully.", typeof(Gemini15ChatResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(500, "Error occurred while calling Grok API.", typeof(object))]
        public async Task<IActionResult> Chat([FromBody] Gemini15ChatRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Invalid request data for Grok-3 chat.");
                return BadRequest(new { Message = "Message is required." });
            }

            try
            {
                var systemPrompt = "You are a helpful and accurate assistant.";

                var requestBody = new
                {
                    model = "grok-3",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = request.Message }
                    },
                    max_tokens = 4096,
                    temperature = 0.3,
                    stream = true
                };

                var responseText = await CallGrokApiAsync(requestBody, useStream: true);

                var result = new Gemini15ChatResponse { Response = responseText };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Grok-3 API chat request.");
                return StatusCode(500, new { Message = "An error occurred while processing the request." });
            }
        }
    }
}
