using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wayfinder.App.Services;
using Wayfinder.Core.DataServices;
using Wayfinder.Core.Rules.Services;
using Wayfinder.Infrastructure.DataServices;
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

            // Set up database
            builder.Services.AddDbContext<WayfinderDbContext>(options =>
                options.UseInMemoryDatabase("WayfinderDb"));

            // Set up data services
            builder.Services.AddScoped<IRaceService, RaceService>();

            // Set up other services
            builder.Services.AddSingleton<IStatCalculator, StatCalculator>();
            builder.Services.AddScoped<CharacterStateViewModel>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Seed the DB from data files
            SeedDataFromDataFiles(app);

            return app;
        }

        private static void SeedDataFromDataFiles(MauiApp app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WayfinderDbContext>();
                DbInitializer.SeedData(dbContext);
            }
        }
    }
}
