using Microsoft.Extensions.Logging;
using Wayfinder.App.Services;
using Wayfinder.Core.DataServices;
using Wayfinder.Core.Factories;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Rules.Services;
using Wayfinder.Core.Services;
using Wayfinder.Infrastructure.DataSeeders;
using Wayfinder.Infrastructure.Persistence;

namespace Wayfinder.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            // Set up data services
            builder.Services.AddScoped<ICharacterStorage, CharacterStorageService>();
            builder.Services.AddSingleton<DataSeeder>();
            builder.Services.AddSingleton<DomainMapper>();

            // Set app services
            builder.Services.AddSingleton<AppStateService>();
            builder.Services.AddSingleton<CharacterStateService>();
            builder.Services.AddSingleton<IAppLogger, AppLoggingService>();
            builder.Services.AddSingleton<DataSeederService>();
            builder.Services.AddSingleton<SampleCharacterSeeder>(); //DEV only

            // ViewModels
            builder.Services.AddScoped<CharacterSheetViewModel>();
            builder.Services.AddScoped<BaseCharacterViewModel>();

            // Set up Pathfinder services
            builder.Services.AddSingleton<IBabCalculator, BabCalculator>();
            //builder.Services.AddSingleton<IAbilityScoreCalculator, AbilityScoreCalculator>();
            builder.Services.AddSingleton<ISaveCalculator, SaveCalculator>();
            builder.Services.AddSingleton<IStatCalculator, StatCalculator>();

            // The compendiums seeded from user files
            builder.Services.AddSingleton<IItemLibrary, ItemLibrary>();
            builder.Services.AddSingleton<IClassLibrary, ClassLibrary>();
            builder.Services.AddSingleton<IRaceLibrary, RaceLibrary>();
            builder.Services.AddSingleton<IPathfinderDataLibrary, PathfinderDataLibrary>();

            // The factories
            builder.Services.AddSingleton<IClassFactory, ClassFactory>();
            builder.Services.AddSingleton<IItemFactory, ItemFactory>();
            builder.Services.AddSingleton<IRaceFactory, RaceFactory>();

            // Set up bundled subsystems
            builder.Services.AddSingleton<IEquipmentManager, EquipmentManager>();
            builder.Services.AddSingleton<IPathfinderRulesEngine, PathfinderRulesEngine>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif

            var app = builder.Build();

            // Initial seed of data
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                seeder.SeedAll(); //TODO: provide the path from user settings or ask the user
            }

            return app;
        }
    }
}
