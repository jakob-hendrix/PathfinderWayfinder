using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wayfinder.Core.Domain.Models;

namespace Wayfinder.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static void SeedData(WayfinderDbContext context)
        {
            context.Database.EnsureCreated();
            if (!context.Races.Any())
            {
                SeedRaceData(context);
            }
        }

        // TODO: change this to look for a file in a folder on disk instead of an embedded resource
        // we can copy base files to that folder on app startup if they don't exist or our version is newer
        private static void SeedRaceData(WayfinderDbContext context)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Wayfinder.Infrastructure.Data.Races.json";

            using Stream stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException("Embedded resource not found for Races", resourceName);
            using StreamReader reader = new(stream);

            var json = reader.ReadToEnd();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Handles enum deserialization from strings
            options.Converters.Add(new JsonStringEnumConverter());

            var races = JsonSerializer.Deserialize<List<Race>>(json, options);
            if (races != null)
            {
                context.Races.AddRange(races);
                context.SaveChanges();
            }
        }
    }
}
