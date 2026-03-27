using Wayfinder.Core.Constants;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Results;
using Wayfinder.Infrastructure.DTOs;

namespace Wayfinder.Infrastructure.Mappers;

public class ClassDomainMapper
{
    public ClassMapperResult Map(ClassYamlDto dto)
    {
        var result = new ClassMapperResult();

        // 1. Critical Base Validation
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            result.Errors.Add("A class was found with no Name. It has been skipped.");
            return result;
        }

        // 2. Map Core Progressions
        BabProgressionRate bab = default;
        SaveProgressionRate fort = default, reflex = default, will = default;
        try
        {
            bab = PathfinderEnumMapper.ToBabProgression(dto.BabRate);
            fort = PathfinderEnumMapper.ToSaveProgression(dto.FortitudeRate);
            reflex = PathfinderEnumMapper.ToSaveProgression(dto.ReflexRate);
            will = PathfinderEnumMapper.ToSaveProgression(dto.WillRate);
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Class '{dto.Name}' has an invalid core progression: {ex.Message}");
            return result;
        }

        // 3. Map Levels and Feature Pointers
        var mappedLevels = new Dictionary<int, LevelDefinition>();
        if (dto.Levels != null)
        {
            // Validate the Level Bounds
            if (dto.Levels.Keys.Any(level => level < 1 || level > 20))
            {
                result.Errors.Add($"Class '{dto.Name}' has levels outside the 1-20 range.");
                return result;
            }

            foreach (var kvp in dto.Levels)
            {
                int levelNum = kvp.Key;
                var levelDto = kvp.Value;

                // Use your specific domain model for the features
                var mappedFeatures = new List<ClassFeatureDefinition>();

                if (levelDto.ClassFeatures != null)
                {
                    foreach (var fDto in levelDto.ClassFeatures)
                    {
                        if (string.IsNullOrWhiteSpace(fDto.Name))
                        {
                            result.Warnings.Add($"Class '{dto.Name}' has a feature at level {levelNum} with no name. Skipped.");
                            continue;
                        }

                        // Map the pointer data directly (Id, Name, Rank)
                        mappedFeatures.Add(new ClassFeatureDefinition
                        {
                            Id = SetId(fDto.Id, fDto.Name),
                            Name = fDto.Name,
                            Rank = fDto.Rank
                        });
                    }
                }

                mappedLevels[levelNum] = new LevelDefinition
                {
                    ClassFeatures = mappedFeatures
                };
            }
        }

        // 4. Map Favored Class Options
        List<RacialFavoredClassBonus>? fcbOptions = null;
        if (dto.RacialFcbOptions != null)
        {
            fcbOptions = dto.RacialFcbOptions.Select(fcb => new RacialFavoredClassBonus
            {
                RaceName = fcb.RaceName,
                Description = fcb.Description
            }).ToList();
        }

        // 5. Assemble the final Definition
        if (result.IsValid)
        {
            result.HydratedClass = new ClassDefinition
            {
                Name = dto.Name,
                BabRate = bab,
                HitDie = dto.HitDie,
                SkillPointsPerLevel = dto.SkillPointsPerLevel,
                FortitudeRate = fort,
                ReflexRate = reflex,
                WillRate = will,
                ClassSkills = dto.ClassSkills ?? new List<string>(),
                Levels = mappedLevels,
                RacialFcbOptions = fcbOptions ?? new List<RacialFavoredClassBonus>()
            };
        }

        return result;
    }

    private string SetId(string? existingId, string name)
    {
        return string.IsNullOrWhiteSpace(existingId) ? name.Replace(" ", "").ToLower() : existingId;
    }
}
