using Wayfinder.Core.DataServices;
using Wayfinder.Core.Services;
using Wayfinder.Infrastructure.DataValidators;
using Wayfinder.Infrastructure.DTOs;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Wayfinder.Infrastructure.DataSeeders
{
    public class DataSeeder
    {
        private readonly IAppLogger _logger;
        private readonly IClassLibrary _classLibrary;
        private readonly IItemLibrary _itemLibrary;
        private readonly IRaceLibrary _raceLibrary;
        private readonly IDeserializer _deserializer;
        private readonly DomainMapper _mapper;

        private string _dataPath = string.Empty;

        public DataSeeder(
            IAppLogger logger,
            IClassLibrary classLibrary,
            IItemLibrary itemLibrary,
            DomainMapper mapper,
            IRaceLibrary raceLibrary)
        {
            _logger = logger;
            _classLibrary = classLibrary;
            _deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .Build();
            _itemLibrary = itemLibrary;
            _mapper = mapper;
            _raceLibrary = raceLibrary;
        }

        public void SeedAll()
        {
            // TODO: pull the path from config or user input
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameData");

            if (!Directory.Exists(_dataPath))
            {
                _logger.LogError($"Critical Failure: Game data folder not found at {_dataPath}");
                return;
            }

            _classLibrary.Clear();
            _itemLibrary.Clear();

            SeedClasses();
            SeedItems();
            SeedRaces();
        }

        private void SeedRaces()
        {
            _raceLibrary.Clear();

            var files = Directory.GetFiles(_dataPath, "Races.yaml");

            foreach (var file in files)
            {
                try
                {
                    // 1. Deserialize 
                    var yaml = File.ReadAllText(file);
                    var result = _deserializer.Deserialize<List<RaceYamlDto>>(yaml);

                    foreach (var dto in result)
                    {
                        // 2. Map DTO to domain
                        var definition = _mapper.MapRaceToDomain(dto);

                        // 3. Validate
                        var (isValid, errors) = RaceSeedValidator.Validate(definition);
                        if (isValid)
                        {
                            _raceLibrary.Register(definition);
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
                    throw new Exception($"Failed to seed races from file {file}", ex);
                }
            }
            //{
            //    var yaml = File.ReadAllText(file);
            //    var dto = _deserializer.Deserialize<RaceYamlDto>(yaml);

            //    var definition = MapRaceToDomain(dto);
            //    _raceLibrary.Register(definition);
            //}
        }

        public void SeedClasses()
        {
            _classLibrary.Clear();

            var files = Directory.GetFiles(_dataPath, "Classes.yaml");

            foreach (var file in files)
            {
                try
                {
                    // 1. Deserialize 
                    var yaml = File.ReadAllText(file);
                    var result = _deserializer.Deserialize<List<ClassYamlDto>>(yaml);

                    foreach (var dto in result)
                    {
                        // 2. Map DTO to domain
                        var definition = _mapper.MapClassToDomain(dto);

                        // 3. Validate
                        var (isValid, errors) = ClassSeedValidator.Validate(definition);
                        if (isValid)
                        {
                            _classLibrary.Register(definition);
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

        public void SeedItems()
        {
            var files = Directory.GetFiles(_dataPath, "Items*.yaml");

            foreach (var file in files)
            {
                try
                {
                    var yaml = File.ReadAllText(file);
                    var result = _deserializer.Deserialize<List<ItemYamlDto>>(yaml);

                    foreach (var dto in result)
                    {
                        var definition = _mapper.MapItemToDomain(dto);

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
