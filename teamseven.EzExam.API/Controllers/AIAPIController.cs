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
    [Route("api/ai")]
    [Produces("application/json")]
    [Authorize] 
    public class AIAPIController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIAPIController> _logger;
        private readonly IConfiguration _configuration;

        public AIAPIController(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<AIAPIController> logger)
        {
            _logger = logger;
            _configuration = configuration;
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

        private async Task<string> CallAIProviderAsync(string provider, string model, object requestBody, bool useStream = false)
        {
            var jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            string endpoint = "";
            string baseUrl = "";
            string apiKey = "";

            // Clear previous headers
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _httpClient.DefaultRequestHeaders.Remove("x-api-key");
            _httpClient.DefaultRequestHeaders.Remove("anthropic-version");

            switch (provider.ToLower())
            {
                case "openai":
                    baseUrl = _configuration["OpenAI:BaseUrl"] ?? "https://api.openai.com/v1";
                    apiKey = _configuration["OpenAI:ApiKey"] ?? throw new ArgumentNullException("OpenAI:ApiKey is not configured.");
                    endpoint = "/chat/completions";
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                    break;

                case "deepseek":
                    baseUrl = _configuration["DeepSeek:BaseUrl"] ?? "https://api.deepseek.com";
                    apiKey = _configuration["DeepSeek:ApiKey"] ?? throw new ArgumentNullException("DeepSeek:ApiKey is not configured.");
                    endpoint = "/chat/completions";
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                    break;

                case "gemini":
                    baseUrl = "https://generativelanguage.googleapis.com/v1beta";
                    apiKey = _configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini:ApiKey is not configured.");
                    endpoint = useStream ? $"/models/{model}:streamGenerateContent?key={apiKey}" : $"/models/{model}:generateContent?key={apiKey}";
                    break;

                case "grok":
                case "xai":
                    baseUrl = "https://api.x.ai/v1";
                    apiKey = _configuration["xAI:ApiKey"] ?? throw new ArgumentNullException("xAI:ApiKey is not configured.");
                    endpoint = "/chat/completions";
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                    break;

                case "anthropic":
                case "claude":
                    baseUrl = _configuration["Anthropic:BaseUrl"] ?? "https://api.anthropic.com/v1";
                    apiKey = _configuration["Anthropic:ApiKey"] ?? throw new ArgumentNullException("Anthropic:ApiKey is not configured.");
                    endpoint = "/messages";
                    _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
                    _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
                    break;

                default:
                    throw new ArgumentException($"Unsupported AI provider: {provider}. Supported providers: openai, deepseek, gemini, grok, anthropic");
            }

            HttpResponseMessage response = await GetRetryPolicy().ExecuteAsync(async () =>
            {
                return await _httpClient.PostAsync($"{baseUrl}{endpoint}", content);
            });

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("{Provider} API error: {StatusCode}, {Error}", provider, response.StatusCode, errorContent);
                throw new HttpRequestException($"Error calling {provider} API: {response.StatusCode}, Details: {errorContent}");
            }

            if (useStream)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);
                var fullResponse = new StringBuilder();
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (provider.ToLower() == "gemini" && !line.StartsWith("data: ")) continue;
                    if (provider.ToLower() != "gemini" && !line.StartsWith("data: ")) continue;
                    
                    var data = line.Substring(6).Trim();
                    if (data == "[DONE]") break;
                    
                    try
                    {
                        var jsonNode = JsonNode.Parse(data);
                        string? contentDelta = null;
                        
                        if (provider.ToLower() == "gemini")
                        {
                            contentDelta = jsonNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.GetValue<string>();
                        }
                        else
                        {
                            contentDelta = jsonNode?["choices"]?[0]?["delta"]?["content"]?.GetValue<string>();
                        }
                        
                        if (!string.IsNullOrEmpty(contentDelta))
                        {
                            fullResponse.Append(contentDelta);
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, "Invalid JSON in {Provider} stream data: {Data}", provider, data);
                    }
                }
                return fullResponse.ToString();
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("{Provider} API response: {ResponseBody}", provider, responseBody);

                using var jsonDoc = JsonDocument.Parse(responseBody);
                if (provider.ToLower() == "gemini")
                {
                    return jsonDoc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? string.Empty;
                }
                else if (provider.ToLower() == "anthropic" || provider.ToLower() == "claude")
                {
                    return jsonDoc.RootElement.GetProperty("content")[0].GetProperty("text").GetString() ?? string.Empty;
                }
                else
                {
                    return jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;
                }
            }
        }

        [HttpPost("solve")]
        [SwaggerOperation(Summary = "Solve problem with AI", Description = "Sends a problem to specified AI provider for detailed step-by-step solution")]
        [SwaggerResponse(200, "Solution retrieved successfully.", typeof(AIChatResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(500, "Error occurred while calling AI API.", typeof(object))]
        public async Task<IActionResult> Solve([FromBody] AISolveRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Input))
            {
                _logger.LogWarning("Invalid request data for AI solve.");
                return BadRequest(new { Message = "Input is required." });
            }

            try
            {
                // Use custom prompt if provided, otherwise use default physics prompt
                var systemPrompt = !string.IsNullOrWhiteSpace(request.DefaultPrompt) 
                    ? request.DefaultPrompt 
                    : "You are an expert physicist specializing in precise, step-by-step solutions using Chain of Thought reasoning. Always provide clear, accurate, and concise explanations.";

                var userPrompt = !string.IsNullOrWhiteSpace(request.DefaultPrompt) 
                    ? request.Input 
                    : $@"For the given problem, follow these steps:
1. Restate the problem clearly in English.
2. List all given data and assumptions.
3. Derive the necessary formulas.
4. Solve step by step, explaining each calculation.
5. Verify the solution against all conditions.
6. Provide the final answer in LaTeX format within \boxed{{}}.

Ensure all mathematical expressions are in LaTeX and use clear English.

Problem: {request.Input}";

                object requestBody;
                string model = request.AIModel;

                switch (request.Provider.ToLower())
                {
                    case "openai":
                        requestBody = new
                        {
                            model = model,
                            messages = new[]
                            {
                                new { role = "system", content = systemPrompt },
                                new { role = "user", content = userPrompt }
                            },
                            max_tokens = 8192,
                            temperature = 0.2,
                            stream = true
                        };
                        break;

                    case "deepseek":
                        requestBody = new
                        {
                            model = model,
                            messages = new[]
                            {
                                new { role = "system", content = systemPrompt },
                                new { role = "user", content = userPrompt }
                            },
                            max_tokens = 8192,
                            temperature = 0.2,
                            stream = true
                        };
                        break;

                    case "gemini":
                        requestBody = new
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
                        break;

                    case "grok":
                    case "xai":
                        requestBody = new
                        {
                            model = model,
                            messages = new[]
                            {
                                new { role = "system", content = systemPrompt },
                                new { role = "user", content = userPrompt }
                            },
                            max_tokens = 8192,
                            temperature = 0.2,
                            stream = true
                        };
                        break;

                    case "anthropic":
                    case "claude":
                        requestBody = new
                        {
                            model = model,
                            max_tokens = 8192,
                            messages = new[]
                            {
                                new { role = "user", content = $"{systemPrompt}\n\n{userPrompt}" }
                            }
                        };
                        break;

                    default:
                        return BadRequest(new { Message = "Unsupported AI provider. Supported providers: openai, deepseek, gemini, grok, anthropic" });
                }

                var responseText = await CallAIProviderAsync(request.Provider, model, requestBody, useStream: true);

                // Fallback for DeepSeek if reasoner model fails
                if (request.Provider.ToLower() == "deepseek" && string.IsNullOrWhiteSpace(responseText))
                {
                    _logger.LogWarning("Reasoner model returned empty response, falling back to chat model.");
                    model = "deepseek-chat";
                    requestBody = new
                    {
                        model = model,
                        messages = new[]
                        {
                            new { role = "system", content = "You are a helpful physics assistant providing step-by-step solutions." },
                            new { role = "user", content = $"Solve this physics problem step by step: {request.Input}" }
                        },
                        max_tokens = 4096,
                        temperature = 0.3,
                        stream = true
                    };
                    responseText = await CallAIProviderAsync(request.Provider, model, requestBody, useStream: true);
                }

                var result = new AIChatResponse { Response = responseText, Provider = request.Provider };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AI solve request for provider: {Provider}", request.Provider);
                return StatusCode(500, new { Message = "An error occurred while processing the request." });
            }
        }

        [HttpPost("latex")]
        [SwaggerOperation(Summary = "Get LaTeX code for problem solution", Description = "Sends a problem to specified AI provider and returns only the LaTeX code")]
        [SwaggerResponse(200, "LaTeX code retrieved successfully.", typeof(AIChatResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(500, "Error occurred while calling AI API.", typeof(object))]
        public async Task<IActionResult> Latex([FromBody] AISolveRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Input))
            {
                _logger.LogWarning("Invalid request data for AI latex.");
                return BadRequest(new { Message = "Input is required." });
            }

            try
            {
                var systemPrompt = "You are an expert physicist providing pure LaTeX mathematical derivations without any text explanations.";

                var userPrompt = $@"For the given problem, provide ONLY the LaTeX code for the solution, including:
- All necessary formulas.
- Step-by-step derivations using LaTeX math mode.
- The final answer in \boxed{{}}.

Do NOT include any explanatory text or natural language.

Problem: {request.Input}";

                object requestBody;
                string model = request.AIModel;

                switch (request.Provider.ToLower())
                {
                    case "openai":
                        requestBody = new
                        {
                            model = model,
                            messages = new[]
                            {
                                new { role = "system", content = systemPrompt },
                                new { role = "user", content = userPrompt }
                            },
                            max_tokens = 8192,
                            temperature = 0.2,
                            stream = true
                        };
                        break;

                    case "deepseek":
                        requestBody = new
                        {
                            model = model,
                            messages = new[]
                            {
                                new { role = "system", content = systemPrompt },
                                new { role = "user", content = userPrompt }
                            },
                            max_tokens = 8192,
                            temperature = 0.2,
                            stream = true
                        };
                        break;

                    case "gemini":
                        requestBody = new
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
                        break;

                    case "grok":
                    case "xai":
                        requestBody = new
                        {
                            model = model,
                            messages = new[]
                            {
                                new { role = "system", content = systemPrompt },
                                new { role = "user", content = userPrompt }
                            },
                            max_tokens = 8192,
                            temperature = 0.2,
                            stream = true
                        };
                        break;

                    case "anthropic":
                    case "claude":
                        requestBody = new
                        {
                            model = model,
                            max_tokens = 8192,
                            messages = new[]
                            {
                                new { role = "user", content = $"{systemPrompt}\n\n{userPrompt}" }
                            }
                        };
                        break;

                    default:
                        return BadRequest(new { Message = "Unsupported AI provider. Supported providers: openai, deepseek, gemini, grok, anthropic" });
                }

                var responseText = await CallAIProviderAsync(request.Provider, model, requestBody, useStream: true);

                var result = new AIChatResponse { Response = responseText, Provider = request.Provider };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AI latex request for provider: {Provider}", request.Provider);
                return StatusCode(500, new { Message = "An error occurred while processing the request." });
            }
        }

        [HttpPost("chat")]
        [SwaggerOperation(Summary = "General chat with AI", Description = "Sends a message to specified AI provider for general response")]
        [SwaggerResponse(200, "Chat response retrieved successfully.", typeof(AIChatResponse))]
        [SwaggerResponse(400, "Invalid request data.", typeof(object))]
        [SwaggerResponse(500, "Error occurred while calling AI API.", typeof(object))]
        public async Task<IActionResult> Chat([FromBody] AISolveRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Input))
            {
                _logger.LogWarning("Invalid request data for AI chat.");
                return BadRequest(new { Message = "Input is required." });
            }

            try
            {
                var systemPrompt = !string.IsNullOrWhiteSpace(request.DefaultPrompt) 
                    ? request.DefaultPrompt 
                    : "You are a helpful and accurate assistant.";

                var userPrompt = request.Input;

                object requestBody;
                string model = request.AIModel;

                switch (request.Provider.ToLower())
                {
                    case "openai":
                        requestBody = new
                        {
                            model = model,
                            messages = new[]
                            {
                                new { role = "system", content = systemPrompt },
                                new { role = "user", content = userPrompt }
                            },
                            max_tokens = 4096,
                            temperature = 0.3,
                            stream = true
                        };
                        break;

                    case "deepseek":
                        requestBody = new
                        {
                            model = model,
                            messages = new[]
                            {
                                new { role = "system", content = systemPrompt },
                                new { role = "user", content = userPrompt }
                            },
                            max_tokens = 4096,
                            temperature = 0.3,
                            stream = true
                        };
                        break;

                    case "gemini":
                        requestBody = new
                        {
                            contents = new[]
                            {
                                new { role = "model", parts = new[] { new { text = systemPrompt } } },
                                new { role = "user", parts = new[] { new { text = userPrompt } } }
                            },
                            generationConfig = new
                            {
                                maxOutputTokens = 4096,
                                temperature = 0.3
                            }
                        };
                        break;

                    case "grok":
                    case "xai":
                        requestBody = new
                        {
                            model = model,
                            messages = new[]
                            {
                                new { role = "system", content = systemPrompt },
                                new { role = "user", content = userPrompt }
                            },
                            max_tokens = 4096,
                            temperature = 0.3,
                            stream = true
                        };
                        break;

                    case "anthropic":
                    case "claude":
                        requestBody = new
                        {
                            model = model,
                            max_tokens = 4096,
                            messages = new[]
                            {
                                new { role = "user", content = $"{systemPrompt}\n\n{userPrompt}" }
                            }
                        };
                        break;

                    default:
                        return BadRequest(new { Message = "Unsupported AI provider. Supported providers: openai, deepseek, gemini, grok, anthropic" });
                }

                var responseText = await CallAIProviderAsync(request.Provider, model, requestBody, useStream: true);

                var result = new AIChatResponse { Response = responseText, Provider = request.Provider };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AI chat request for provider: {Provider}", request.Provider);
                return StatusCode(500, new { Message = "An error occurred while processing the request." });
            }
        }
    }
}
