using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using teamseven.EzExam.Services.Services.SubscriptionService;

namespace teamseven.EzExam.API.Services
{
    public class SubscriptionExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SubscriptionExpirationService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Check every hour

        public SubscriptionExpirationService(
            IServiceProvider serviceProvider,
            ILogger<SubscriptionExpirationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Subscription Expiration Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();

                    _logger.LogInformation("Checking for expired subscriptions...");
                    
                    var result = await subscriptionService.CheckAndExpireSubscriptionsAsync();
                    
                    if (result)
                    {
                        _logger.LogInformation("Subscription expiration check completed - some subscriptions were expired");
                    }
                    else
                    {
                        _logger.LogInformation("Subscription expiration check completed - no expired subscriptions found");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking expired subscriptions: {Message}", ex.Message);
                }

                // Wait for the next check interval
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Subscription Expiration Service stopped");
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Subscription Expiration Service...");
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Subscription Expiration Service...");
            await base.StopAsync(cancellationToken);
        }
    }
}

