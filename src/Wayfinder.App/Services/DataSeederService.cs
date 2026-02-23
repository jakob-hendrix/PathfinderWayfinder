using Wayfinder.Core.Services;
using Wayfinder.Infrastructure.DataSeeders;

namespace Wayfinder.App.Services
{
    public class DataSeederService
    {
        private readonly DataSeeder _dataSeeder;
        private readonly AppStateService _appStateService;
        private readonly IAppLogger _logger;

        public DataSeederService(DataSeeder dataSeeder, AppStateService appStateService, IAppLogger logger)
        {
            _dataSeeder = dataSeeder;
            _appStateService = appStateService;
            _logger = logger;
        }

        public async Task ReloadSeedDataAsync()
        {
            _appStateService.SetLoading(true);
            _logger.LogInfo("Loading data from data files initiated");

            // Minor delay for UX visibility
            await Task.Delay(500);

            try
            {
                await Task.Run(() => _dataSeeder.SeedAll());
                _logger.LogInfo("Data reload completed");
                _appStateService.NotifyDataRefreshed();

            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to load data from data files.", ex);
            }
            finally
            {
                _appStateService.SetLoading(false);
            }
        }
    }
}
