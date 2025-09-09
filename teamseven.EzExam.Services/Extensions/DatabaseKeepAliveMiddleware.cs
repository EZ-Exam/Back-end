using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

public class DatabaseKeepAliveMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseKeepAliveMiddleware> _logger;
    private readonly TimeSpan _queryInterval = TimeSpan.FromMinutes(60); // Truy v?n m?i 45 min
    private DateTime _lastQueryTime;
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private Task _backgroundTask;

    public DatabaseKeepAliveMiddleware(RequestDelegate next, IServiceProvider serviceProvider, ILogger<DatabaseKeepAliveMiddleware> logger)
    {
        _next = next;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _lastQueryTime = DateTime.MinValue;
        _backgroundTask = StartBackgroundQuery(_cts.Token); // B?t d?u task n?n
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Ti?p t?c pipeline
        await _next(context);
    }

    private async Task StartBackgroundQuery(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (DateTime.UtcNow - _lastQueryTime >= _queryInterval)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<teamsevenezexamdbContext>();
                        await dbContext.Database.ExecuteSqlRawAsync("SELECT 1");
                        _lastQueryTime = DateTime.UtcNow;
                        _logger.LogInformation("Database keep-alive query executed at {Time}", _lastQueryTime);
                        Console.WriteLine("Database keep-alive query executed");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute keep-alive query");
                Console.WriteLine($"Keep-alive query failed: {ex.Message}");
            }

            // Ch? 30 phút tru?c khi ki?m tra l?i
            await Task.Delay(_queryInterval, cancellationToken);
        }
    }

    // H?y task khi middleware b? dispose (tùy ch?n)
    public void Dispose()
    {
        _cts.Cancel();
    }
}

// Extension method d? dang ký middleware
public static class DatabaseKeepAliveMiddlewareExtensions
{
    public static IApplicationBuilder UseDatabaseKeepAlive(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<DatabaseKeepAliveMiddleware>();
    }
}
