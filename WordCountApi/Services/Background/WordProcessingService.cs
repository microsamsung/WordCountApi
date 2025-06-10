namespace WordCountApi.Services.Background
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;

    namespace WordCountApi.Services.Background
    {
        public class WordProcessingService : BackgroundService
        {
            private readonly ILogger<WordProcessingService> _logger;

            public WordProcessingService(ILogger<WordProcessingService> logger)
            {
                _logger = logger;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                _logger.LogInformation("WordProcessingService started.");

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        // Simulate processing or monitor a queue
                        _logger.LogInformation("WordProcessingService is running...");

                        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("WordProcessingService cancelled.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception in WordProcessingService.");
                }
                finally
                {
                    _logger.LogInformation("WordProcessingService is stopping.");
                }
            }
        }
    }

}
