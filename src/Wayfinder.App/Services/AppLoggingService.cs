using Microsoft.Extensions.Logging;
using Wayfinder.Core.Interfaces;

namespace Wayfinder.App.Services
{
    public class AppLoggingService : IAppLogger
    {
        private readonly ILogger<AppLoggingService> _logger;
        public AppLoggingService(ILogger<AppLoggingService> logger)
        {
            _logger = logger;
        }

        public void LogError(string message, Exception? ex = null)
        {
            // Currently, log to VS output window
            _logger.LogError(ex, "[ERROR] Wayfinder Error: {Message}", message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning("[WARN] Wayfinder Warning: {Message}", message);
        }

        public void LogInfo(string message)
        {
            _logger.LogInformation("[INFO] Wayfinder Info: {Message}", message);
        }
    }
}
