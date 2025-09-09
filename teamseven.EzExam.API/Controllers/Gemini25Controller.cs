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
    [Route("api/gemini-25")]
    [Produces("application/json")]
    public class Gemini25Controller : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<Gemini25Controller> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public Gemini25Controller(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<Gemini25Controller> logger)
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

            string endpoint = useStream ? $"/models/{model}:streamGenerateContent?key={_apiKey}&alt=sse" : $"/models/{model}:generateContent?key={_apiKey}";

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
                    if (line.StartsWith("data: "))
                    {
                        var data = line.Substring(6).Trim();
                        if (data == "[DONE]") break;
                        try
                        {
                            var jsonNode = JsonNode.Parse(data);
                            var contentDelta = jsonNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.GetValue<string>();
                            if (!string.IsNullOrEmpty(contentDelta))
                            {
                                fullResponse.Append(contentDelta);
                            }
                        }
                        catch (JsonException ex)
                        {
                            _logger.LogWarning(ex, "Invalid JSON in Gemini stream data: {Data}", data);
                        }
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
        [SwaggerOperation(Summary = "Solve physics problem with Gemini 2.5 API", Description = "Sends a physics problem to Gemini 2.5 for detailed step-by-step solution with enhanced thinking mode.")]
        [SwaggerResponse(200, "Physics solution retrieved successfully.", typeof(Gemini25ChatResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(500, "Error occurred while calling Gemini API.", typeof(object))]
        public async Task<IActionResult> Solve([FromBody] Gemini25ChatRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Invalid request data for Gemini 2.5 solve.");
                return BadRequest(new { Message = "Message is required." });
            }

            try
            {
                var systemPrompt = "You are an expert physicist specializing in precise, step-by-step solutions using advanced Chain of Thought reasoning. Leverage your thinking capabilities for accurate explanations.";

                var userPrompt = $@"Solve the following physics problem with a detailed explanation in plain text. Provide the solution objectively, without personal pronouns or conversational tone. Structure the response as a formal explanation of the problem, given data, derivations, calculations, verification, and conclusion. Use LaTeX for mathematical expressions, and box the final answer. Ensure the response is comprehensive and flows logically as a single, cohesive mathematical solution.

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

                var responseText = await CallGeminiApiAsync(requestBody, "gemini-2.5-flash", useStream: true);

                if (string.IsNullOrWhiteSpace(responseText))
                {
                    _logger.LogWarning("Flash model returned empty response, falling back to Pro model.");
                    responseText = await CallGeminiApiAsync(requestBody, "gemini-2.5-pro", useStream: true);
                }

                var result = new Gemini25ChatResponse { Response = responseText };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Gemini 2.5 API solve request.");
                return StatusCode(500, new { Message = "An error occurred while processing the request." });
            }
        }

        [HttpPost("latex")]
        [SwaggerOperation(Summary = "Get LaTeX code for physics problem solution with Gemini 2.5", Description = "Sends a physics problem to Gemini 2.5 and returns only the LaTeX code for the solution.")]
        [SwaggerResponse(200, "LaTeX code retrieved successfully.", typeof(Gemini25ChatResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(500, "Error occurred while calling Gemini API.", typeof(object))]
        public async Task<IActionResult> Latex([FromBody] Gemini25ChatRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Invalid request data for Gemini 2.5 latex.");
                return BadRequest(new { Message = "Message is required." });
            }

            try
            {
                var systemPrompt = "You are an expert physicist providing pure LaTeX code for mathematical derivations, optimized for rendering in Manim animations. Ensure high accuracy in formulas and derivations.";

                var userPrompt = $@"For the given problem, provide ONLY the LaTeX code for the solution in a single align* environment. Do not include any explanatory text, natural language, documentclass, packages, begin/end document, or the problem statement. Include all necessary formulas, step-by-step derivations in LaTeX math mode, and the final answer in \boxed{{}}. Make it suitable for integration into Manim for visual display, focusing on clarity and precision without extra formatting.

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

                var responseText = await CallGeminiApiAsync(requestBody, "gemini-2.5-flash", useStream: true);

                if (string.IsNullOrWhiteSpace(responseText))
                {
                    _logger.LogWarning("Flash model returned empty response, falling back to Pro model.");
                    responseText = await CallGeminiApiAsync(requestBody, "gemini-2.5-pro", useStream: true);
                }

                var result = new Gemini25ChatResponse { Response = responseText };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Gemini 2.5 API latex request.");
                return StatusCode(500, new { Message = "An error occurred while processing the request." });
            }
        }

        [HttpPost("chat")]
        [SwaggerOperation(Summary = "General chat with Gemini 2.5 API", Description = "Sends a message to Gemini 2.5 for general response with enhanced reasoning.")]
        [SwaggerResponse(200, "Chat response retrieved successfully.", typeof(Gemini25ChatResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(500, "Error occurred while calling Gemini API.", typeof(object))]
        public async Task<IActionResult> Chat([FromBody] Gemini25ChatRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Invalid request data for Gemini 2.5 chat.");
                return BadRequest(new { Message = "Message is required." });
            }

            try
            {
                var systemPrompt = "You are a helpful and accurate assistant with advanced thinking capabilities.";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new { role = "model", parts = new[] { new { text = systemPrompt } } },
                        new { role = "user", parts = new[] { new { text = request.Message } } }
                    },
                    generationConfig = new
                    {
                        maxOutputTokens = 8192, // Higher for 2.5 models
                        temperature = 0.3
                    }
                };

                var responseText = await CallGeminiApiAsync(requestBody, "gemini-2.5-flash", useStream: true);

                var result = new Gemini25ChatResponse { Response = responseText };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Gemini 2.5 API chat request.");
                return StatusCode(500, new { Message = "An error occurred while processing the request." });
            }
        }
    }
}
