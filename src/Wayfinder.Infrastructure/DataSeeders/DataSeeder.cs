using Wayfinder.Core.Data;
using Wayfinder.Core.Data.Interfaces;
using Wayfinder.Core.Interfaces;
using Wayfinder.Infrastructure.DataValidators;
using Wayfinder.Infrastructure.DTOs;
using Wayfinder.Infrastructure.Mappers;
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
        private readonly ISkillLibrary _skillLibrary;
        private readonly IDeserializer _deserializer;
        private readonly DomainMapper _mapper;

        private string _dataPath = string.Empty;

        public DataSeeder(
            IAppLogger logger,
            IClassLibrary classLibrary,
            IItemLibrary itemLibrary,
            DomainMapper mapper,
            IRaceLibrary raceLibrary,
            ISkillLibrary skillLibrary)
        {
            _logger = logger;
            _classLibrary = classLibrary;
            _deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .Build();
            _itemLibrary = itemLibrary;
            _mapper = mapper;
            _raceLibrary = raceLibrary;
            _skillLibrary = skillLibrary;
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
            SeedSkills();
        }

        private void SeedSkills()
        {
            _skillLibrary.Seed(StandardSkills.GetCoreSkills());
        }

        private void SeedRaces()
        {
            _raceLibrary.Clear();
            var seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var files = Directory.GetFiles(_dataPath, "Races.yaml");

            // Instantiate your new permissive mapper
            var raceMapper = new RaceDomainMapper();

            foreach (var file in files)
            {
                try
                {
                    var yaml = File.ReadAllText(file);
                    var rawDtos = _deserializer.Deserialize<List<RaceYamlDto>>(yaml);

                    foreach (var dto in rawDtos)
                    {
                        // 1. Map and Validate simultaneously
                        var mapResult = raceMapper.Map(dto);

                        // 2. Always log the non-fatal errors so the developer can fix the YAML later
                        foreach (var error in mapResult.Errors)
                        {
                            _logger.LogWarning($"[YAML SEED WARNING] {error}");
                        }

                        // 3. Register if the critical base data (like the Name) survived
                        if (mapResult.IsValid && mapResult.HydratedRace != null)
                        {
                            if (!seenIds.Add(mapResult.HydratedRace.Id))
                            {
                                _logger.LogError($"[YAML SEED ERROR] Duplicate Race ID found: '{mapResult.HydratedRace.Id}'");
                                continue;
                            }

                            _raceLibrary.Register(mapResult.HydratedRace);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Critical failure seeding races from file {file}", ex);
                }
            }
        }

        private void SeedClasses()
        {
            _classLibrary.Clear();
            var seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var files = Directory.GetFiles(_dataPath, "Classes.yaml");

            var classMapper = new ClassDomainMapper();

            foreach (var file in files)
            {
                try
                {
                    var yaml = File.ReadAllText(file);
                    var rawDtos = _deserializer.Deserialize<List<ClassYamlDto>>(yaml);

                    foreach (var dto in rawDtos)
                    {
                        var mapResult = classMapper.Map(dto);

                        // 1. Log Non-Fatal Warnings (Graceful Degradations)
                        foreach (var warning in mapResult.Warnings)
                        {
                            _logger.LogWarning($"[YAML SEED WARNING] {warning}");
                        }

                        // 2. Handle Fatal Errors
                        if (!mapResult.IsValid)
                        {
                            foreach (var error in mapResult.Errors)
                            {
                                _logger.LogError($"[YAML SEED ERROR] {error}");
                            }
                            continue; // Skip registration
                        }

                        // 3. Register the Valid Class
                        if (mapResult.HydratedClass != null)
                        {
                            // Assuming you use Name for uniqueness, or dto.Id if you have it
                            if (!seenIds.Add(mapResult.HydratedClass.Name))
                            {
                                _logger.LogError($"[YAML SEED ERROR] Duplicate Class Name found: '{mapResult.HydratedClass.Name}'");
                                continue;
                            }

                            _classLibrary.Register(mapResult.HydratedClass);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Critical failure seeding classes from file {file}", ex);
                }
            }
        }

        public void SeedItems()
        {
            var files = Directory.GetFiles(_dataPath, "Items*.yaml");
            var seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var file in files)
            {
                try
                {
                    var yaml = File.ReadAllText(file);
                    var result = _deserializer.Deserialize<List<ItemYamlDto>>(yaml);

                    foreach (var dto in result)
                    {
                        var definition = _mapper.MapItemToDomain(dto);

                        if (!seenIds.Add(definition.Name))
                        {
                            _logger.LogError($"[YAML SEED ERROR] Duplicate Item found across files: '{definition.Name}'");
                            continue;
                        }

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
