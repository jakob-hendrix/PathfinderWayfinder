using Microsoft.Extensions.Logging;
using Wayfinder.App.Services;
using Wayfinder.App.ViewModels;
using Wayfinder.Core.Configuration;
using Wayfinder.Core.Data;
using Wayfinder.Core.Data.Interfaces;
using Wayfinder.Core.DataServices;
using Wayfinder.Core.Factories;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Logic.Features;
using Wayfinder.Core.Logic.Interfaces;
using Wayfinder.Core.Rules.Engines;
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

            #region Data Services
            builder.Services.AddScoped<ICharacterStorage, CharacterStorageService>();
            builder.Services.AddSingleton<DataSeeder>();
            #endregion

            #region UI Services
            builder.Services.AddScoped<AppStateService>();
            builder.Services.AddScoped<CharacterStateService>();
            builder.Services.AddSingleton<IAppLogger, AppLoggingService>();
            builder.Services.AddSingleton<DataSeederService>();
            builder.Services.AddSingleton<SampleCharacterSeeder>(); //DEV only
            builder.Services.AddTransient<CharacterSheetViewModel>();
            builder.Services.AddTransient<BaseCharacterViewModel>();
            builder.Services.AddTransient<InventoryViewModel>();
            builder.Services.AddTransient<ClassLevelsViewModel>();
            builder.Services.AddTransient<ClassLevelDetailViewModel>();
            builder.Services.AddTransient<SkillViewModel>();
            #endregion

            #region Pathfinder services
            // The compendiums seeded from user files
            builder.Services.AddSingleton<IItemLibrary, ItemLibrary>();
            builder.Services.AddSingleton<IClassLibrary, ClassLibrary>();
            builder.Services.AddSingleton<IRaceLibrary, RaceLibrary>();
            builder.Services.AddSingleton<ISkillLibrary, SkillLibrary>();
            builder.Services.AddSingleton<IPathfinderDataLibrary, PathfinderDataLibrary>();

            // The factories
            builder.Services.AddSingleton<IClassFeatureRegistry, ClassFeatureRegistry>();
            builder.Services.AddSingleton<IClassFactory, ClassFactory>();
            builder.Services.AddSingleton<IItemFactory, ItemFactory>();
            builder.Services.AddSingleton<IRaceFactory, RaceFactory>();

            // Set up bundled subsystems
            builder.Services.AddClassFeatures();
            builder.Services.AddSingleton<IClassLevelEngine, ClassLevelEngine>();
            builder.Services.AddSingleton<ISkillEngine, SkillEngine>();
            builder.Services.AddSingleton<IPathfinderRulesEngine, PathfinderRulesEngine>();
            #endregion
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
