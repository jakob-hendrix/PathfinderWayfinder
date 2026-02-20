using Microsoft.Extensions.Logging;

namespace Wayfinder.App.Services
{
    public class AppLoggingService
    {
        private readonly ILogger<AppLoggingService> _logger;
        public AppLoggingService(ILogger<AppLoggingService> logger)
        {
            _logger = logger;
        }

        public void LogError(string message, Exception? ex = null)
        {
            // Currently, log to VS output window
            _logger.LogError(ex, "Wayfinder Error: {Message}", message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning("Wayfinder Warning: {Message}", message);
        }

        public void LogInfo(string message)
        {
            _logger.LogInformation("Wayfinder Info: {Message}", message);
        }
    }
}
