using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using teamseven.EzExam.Services.Services.JwtHelperService;
using teamseven.EzExam.Services.Services.UsageTrackingService;

namespace teamseven.EzExam.API.Middleware
{
    public class SubscriptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SubscriptionMiddleware> _logger;

        public SubscriptionMiddleware(RequestDelegate next, ILogger<SubscriptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IJwtHelperService jwtHelperService, IUsageTrackingService usageTrackingService)
        {
            if (!context.Request.Path.StartsWithSegments("/api") || 
                !IsAIEndpoint(context.Request.Path))
            {
                await _next(context);
                return;
            }

            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                await _next(context);
                return;
            }

            try
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                var userId = jwtHelperService.GetCurrentUserIdFromToken(authHeader);

                if (userId == null)
                {
                    _logger.LogWarning("Could not extract user ID from token for AI endpoint: {Path}", context.Request.Path);
                    await _next(context);
                    return;
                }

                var canPerformAction = await usageTrackingService.CanUserPerformActionAsync(userId.Value, "AI_REQUEST");

                if (!canPerformAction)
                {
                    _logger.LogWarning("User {UserId} attempted to access AI endpoint {Path} but subscription does not allow", 
                        userId.Value, context.Request.Path);

                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    
                    var errorResponse = new
                    {
                        message = "AI access is not available with your current subscription. Please upgrade to a premium plan.",
                        errorCode = "SUBSCRIPTION_LIMIT_EXCEEDED",
                        subscriptionRequired = true
                    };

                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse));
                    return;
                }

                await usageTrackingService.CheckAndIncrementAIRequestAsync(userId.Value, 
                    $"AI request to {context.Request.Path}");

                _logger.LogInformation("User {UserId} successfully accessed AI endpoint: {Path}", 
                    userId.Value, context.Request.Path);

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SubscriptionMiddleware for path {Path}: {Message}", 
                    context.Request.Path, ex.Message);
                
                await _next(context);
            }
        }

        private static bool IsAIEndpoint(PathString path)
        {
            var pathString = path.Value?.ToLower() ?? "";
            
            var aiEndpoints = new[]
            {
                "/api/gemini-15",
                "/api/gemini-25", 
                "/api/deepseek",
                "/api/grok-3",
                "/api/grok-3-mini"
            };

            return aiEndpoints.Any(endpoint => pathString.StartsWith(endpoint));
        }
    }

    public static class SubscriptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseSubscriptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SubscriptionMiddleware>();
        }
    }
}
