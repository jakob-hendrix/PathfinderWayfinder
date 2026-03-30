using Wayfinder.Core.Constants;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Results;
using Wayfinder.Infrastructure.DTOs;

namespace Wayfinder.Infrastructure.Mappers;

public class RaceDomainMapper
{
    public RaceMapperResult Map(RaceYamlDto dto)
    {
        var result = new RaceMapperResult();

        // 1. Critical Validation
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            result.Errors.Add("A race was found with no Name. It has been skipped.");
            return result;
        }

        var seenTraitNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // 2. Map Alternative Traits
        var mappedAltTraits = new List<AlternativeRacialTrait>();
        if (dto.AlternativeRacialTraits != null)
        {
            foreach (var altDto in dto.AlternativeRacialTraits)
            {
                if (string.IsNullOrWhiteSpace(altDto.Name))
                {
                    result.Errors.Add($"Race '{dto.Name}' has an alternative trait with no name. Skipped.");
                    continue;
                }

                if (!seenTraitNames.Add(altDto.Name))
                {
                    result.Errors.Add($"Duplicate trait name '{altDto.Name}' in race '{dto.Name}'. Skipped.");
                    continue;
                }

                if (altDto.ReplacesTraitNames == null || !altDto.ReplacesTraitNames.Any())
                {
                    result.Errors.Add($"Alternative trait '{altDto.Name}' in race '{dto.Name}' does not specify replaced traits.");
                }

                var (effects, effectErrorSuffix) = MapEffects(altDto.GrantedEffects, dto.Name, altDto.Name, result);

                mappedAltTraits.Add(new AlternativeRacialTrait
                {
                    Name = altDto.Name,
                    Description = altDto.Description + effectErrorSuffix, // Append warning if effects failed
                    ReplacesRacialTraits = altDto.ReplacesTraitNames ?? new List<string>(),
                    GrantedEffects = effects
                });
            }
        }

        // 3. Map Default Traits
        var mappedDefaultTraits = new List<RacialTrait>();
        if (dto.DefaultRacialTraits != null)
        {
            foreach (var tDto in dto.DefaultRacialTraits)
            {
                if (string.IsNullOrWhiteSpace(tDto.Name))
                {
                    result.Errors.Add($"Race '{dto.Name}' has a default trait with no name. Skipped.");
                    continue;
                }

                if (!seenTraitNames.Add(tDto.Name))
                {
                    result.Errors.Add($"Duplicate trait name '{tDto.Name}' in race '{dto.Name}'. Skipped.");
                    continue;
                }

                var (effects, effectErrorSuffix) = MapEffects(tDto.GrantedEffects, dto.Name, tDto.Name, result);

                mappedDefaultTraits.Add(new RacialTrait
                {
                    Name = tDto.Name,
                    Description = tDto.Description + effectErrorSuffix, // Append warning if effects failed
                    GrantedEffects = effects
                });
            }
        }

        // 4. Map Subraces safely (No throwing exceptions!)
        var mappedSubraces = new List<Subrace>();
        if (dto.Subraces != null)
        {
            var seenSubraceNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var sDto in dto.Subraces)
            {
                if (string.IsNullOrWhiteSpace(sDto.Name))
                {
                    result.Errors.Add($"Race '{dto.Name}' has a subrace with no name. Skipped.");
                    continue;
                }

                if (!seenSubraceNames.Add(sDto.Name))
                {
                    result.Errors.Add($"Duplicate subrace name '{sDto.Name}' in race '{dto.Name}'. Skipped.");
                    continue;
                }

                var linkedTraits = new List<AlternativeRacialTrait>();
                if (sDto.AlternativeTraitNames != null)
                {
                    foreach (var traitName in sDto.AlternativeTraitNames)
                    {
                        var found = mappedAltTraits.FirstOrDefault(alt => alt.Name.Equals(traitName, StringComparison.OrdinalIgnoreCase));
                        if (found == null)
                        {
                            result.Errors.Add($"Subrace '{sDto.Name}' references missing trait '{traitName}'.");
                        }
                        else
                        {
                            linkedTraits.Add(found);
                        }
                    }
                }

                mappedSubraces.Add(new Subrace
                {
                    Id = SetId(sDto.Id, sDto.Name),
                    Name = sDto.Name,
                    Description = sDto.Description,
                    RacialTraits = linkedTraits
                });
            }
        }

        // 5. Assemble the final Definition
        result.HydratedRace = new RaceDefinition
        {
            Id = SetId(dto.Id, dto.Name),
            Name = dto.Name,
            CreatureType = dto.CreatureType ?? "Humanoid",
            Subtypes = dto.Subtypes ?? new List<string>(),
            DefaultRacialTraits = mappedDefaultTraits,
            AlternativeRacialTraits = mappedAltTraits,
            Subraces = mappedSubraces
        };

        return result;
    }

    // --- The Permissive Effect Mapper ---
    // Returns the list of effects, AND a string suffix to append to the description if things went wrong.
    private (List<ActiveEffect> Effects, string DescriptionSuffix) MapEffects(
        IEnumerable<EffectDto>? effects,
        string raceName,
        string traitName,
        RaceMapperResult result)
    {
        if (effects == null || !effects.Any())
            return (new List<ActiveEffect>(), string.Empty);

        var mapped = new List<ActiveEffect>();
        bool hasErrors = false;

        foreach (var e in effects)
        {
            if (string.IsNullOrWhiteSpace(e.Target))
            {
                result.Warnings.Add($"Trait '{traitName}' in '{raceName}' has an effect with no Target.");
                hasErrors = true;
                continue;
            }

            // Handle values of string or numerics
            int numericValue = 0;
            string? stringValue = null;

            if (int.TryParse(e.Value, out int parsed))
            {
                numericValue = parsed; // It was a number, like 30
            }
            else
            {
                stringValue = e.Value; // It was a string, like "Medium"
            }

            try
            {
                mapped.Add(new ActiveEffect
                {
                    TargetStatName = e.Target,
                    Value = numericValue,
                    StringValue = stringValue,
                    Type = PathfinderEnumMapper.ToModifierType(e.Type), // Safely attempts conversion
                    Category = EffectCategory.RacialTrait,
                    IsConditional = e.IsConditional,
                    SourceName = $"{raceName} ({traitName})"
                });
            }
            catch (Exception ex)
            {
                // We catch the Enum mapping error here instead of letting it blow up the loop
                result.Warnings.Add($"Trait '{traitName}' in '{raceName}' failed to map effect '{e.Target}': {ex.Message}");
                hasErrors = true;
            }
        }

        // The "Degrade Gracefully" mechanic:
        // If ANY effect in this trait is broken, we strip ALL math from it so it doesn't 
        // silently apply half a trait. We then append a warning so the UI shows it's broken.
        if (hasErrors)
        {
            return (new List<ActiveEffect>(), "\n\n[System Warning: The mathematical effects for this trait failed to load. It is currently descriptive-only.]");
        }

        return (mapped, string.Empty);
    }

    private string SetId(string? existingId, string name)
    {
        return string.IsNullOrWhiteSpace(existingId) ? name.Replace(" ", "").ToLower() : existingId;
    }
}
