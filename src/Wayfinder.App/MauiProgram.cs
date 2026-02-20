using Microsoft.Extensions.Logging;
using Wayfinder.App.Services;
using Wayfinder.Core.DataServices;
using Wayfinder.Core.Rules.Services;
using Wayfinder.Infrastructure.Persistence;
using Wayfinder.Tests.Core;

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

            // Set up database
            //builder.Services.AddDbContext<WayfinderDbContext>(options =>
            //    options.UseInMemoryDatabase("WayfinderDb"));

            // Set up data services
            //builder.Services.AddScoped<IRaceService, RaceService>();
            builder.Services.AddScoped<ICharacterStorage, CharacterStorageService>();

            // Set app services
            builder.Services.AddSingleton<AppLoggingService>();

            // Set up Pathfinder services
            builder.Services.AddSingleton<IBabCalculator, BabCalculator>();
            builder.Services.AddSingleton<IAbilityScoreCalculator, AbilityScoreCalculator>();
            builder.Services.AddSingleton<ISaveCalculator, SaveCalculator>();
            builder.Services.AddSingleton<IStatCalculator, StatCalculator>();
            builder.Services.AddSingleton<IClassRegistry, ClassRegistry>();
            builder.Services.AddScoped<CharacterStateViewModel>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif

            var app = builder.Build();

            // Seed the DB from data files
            //SeedDataFromDataFiles(app);

            return app;
        }

        //private static void SeedDataFromDataFiles(MauiApp app)
        //{
        //    using (var scope = app.Services.CreateScope())
        //    {
        //        var dbContext = scope.ServiceProvider.GetRequiredService<WayfinderDbContext>();
        //        DbInitializer.SeedData(dbContext);
        //    }
        //}
    }
}
