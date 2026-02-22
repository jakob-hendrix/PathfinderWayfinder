using Wayfinder.Core.Data.Definitions;
using Wayfinder.Core.DataServices;
using Wayfinder.Core.Services;
using Wayfinder.Infrastructure.DataValidators;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Wayfinder.Infrastructure.DataSeeders
{
    public class DataSeeder
    {
        private readonly IAppLogger _logger;
        private readonly IClassLibrary _classLibrary;
        private readonly IItemLibrary _itemLibrary;
        private readonly IDeserializer _deserializer;

        public DataSeeder(
            IAppLogger logger,
            IClassLibrary classLibrary,
            IItemLibrary itemLibrary)
        {
            _logger = logger;
            _classLibrary = classLibrary;
            _deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .Build();
            _itemLibrary = itemLibrary;
        }

        public void SeedAll()
        {
            // TODO: pull the path from config or user input
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataFiles");

            SeedClasses(basePath);
            SeedItems(basePath);
        }

        public void SeedClasses(string filePath)
        {
            foreach (var file in Directory.GetFiles(filePath, "Classes.yaml"))
            {
                try
                {
                    var yaml = File.ReadAllText(file);
                    var result = _deserializer.Deserialize<ClassDefinition>(yaml);

                    // Validate user input
                    // TODO: add new validator for class data
                    if (result.Levels.Keys.Any(level => level < 1 || level > 20))
                    {
                        throw new ArgumentException("Level keys must be between 1 and 20.");
                    }

                    _classLibrary.Register(result);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public void SeedItems(string filePath)
        {
            foreach (var file in Directory.GetFiles(filePath, "Items*.yaml"))
            {
                try
                {
                    var yaml = File.ReadAllText(file);
                    var result = _deserializer.Deserialize<List<ItemDefinition>>(yaml);

                    foreach (var definition in result)
                    {
                        var (isValid, errors) = ItemSeedValidator.Validate(definition);
                        if (isValid)
                        {
                            _itemLibrary.Register(definition);
                            continue;
                        }
                        else
                        {
                            foreach (var error in errors)
                            {
                                _logger.LogError($"[YAML SEED ERROR] '{definition.Name}': {error}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to seed items from file {file}", ex);
                }
            }
        }
    }
}
