using Wayfinder.Core.Data;
using Wayfinder.Core.Data.Interfaces;
using Wayfinder.Core.Interfaces;
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

        private string _dataPath = string.Empty;

        public DataSeeder(
            IAppLogger logger,
            IClassLibrary classLibrary,
            IItemLibrary itemLibrary,
            IRaceLibrary raceLibrary,
            ISkillLibrary skillLibrary)
        {
            _logger = logger;
            _classLibrary = classLibrary;
            _deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .Build();
            _itemLibrary = itemLibrary;
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

        private void SeedItems()
        {
            _itemLibrary.Clear();
            var seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Matches Items.yaml, Items.Weapons.yaml, etc
            var files = Directory.GetFiles(_dataPath, "Items*.yaml");

            var itemMapper = new ItemDomainMapper();

            foreach (var file in files)
            {
                try
                {
                    var yaml = File.ReadAllText(file);
                    var rawDtos = _deserializer.Deserialize<List<ItemYamlDto>>(yaml);

                    foreach (var dto in rawDtos)
                    {
                        // 1. Map and Validate simultaneously
                        var mapResult = itemMapper.Map(dto);

                        // 2. Log Non-Fatal Warnings (Graceful Degradations)
                        // Example: "No specific property validator implemented for ItemType 'Weapon'..."
                        foreach (var warning in mapResult.Warnings)
                        {
                            _logger.LogWarning($"[YAML SEED WARNING] {warning}");
                        }

                        // 3. Handle Fatal Errors
                        // Example: Missing an 'ACP' property on an Armor item
                        if (!mapResult.IsValid)
                        {
                            foreach (var error in mapResult.Errors)
                            {
                                _logger.LogError($"[YAML SEED ERROR] {error}");
                            }
                            continue;
                        }

                        // 4. Register the Valid Item
                        if (mapResult.HydratedItem != null)
                        {
                            // Ensure uniqueness across all loaded item files
                            if (!seenIds.Add(mapResult.HydratedItem.Id))
                            {
                                _logger.LogError($"[YAML SEED ERROR] Duplicate Item ID found: '{mapResult.HydratedItem.Id}'");
                                continue;
                            }

                            _itemLibrary.Register(mapResult.HydratedItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Critical failure seeding items from file {file}", ex);
                }
            }
        }
    }
}
