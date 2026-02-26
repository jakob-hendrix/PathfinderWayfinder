using Wayfinder.Core.Data.Definitions;

namespace Wayfinder.Infrastructure.DataValidators
{
    /// <summary>
    /// Validation rules mapping the Race YAML definitions to the ClassDefinition library
    /// </summary>
    public static class RaceSeedValidator
    {
        public static (bool IsValid, List<string> Errors) Validate(RaceDefinition definition)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(definition.Name)) errors.Add("Race Name is required.");

            // Track trait names across both Default and Alternative lists for this specific race
            var seenTraitNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 1. Validate Default Racial Traits
            if (definition.DefaultRacialTraits != null)
            {
                foreach (var trait in definition.DefaultRacialTraits)
                {
                    if (string.IsNullOrWhiteSpace(trait.Name))
                        errors.Add("A default racial trait is missing a name.");
                    else if (!seenTraitNames.Add(trait.Name))
                        errors.Add($"Duplicate trait name found: '{trait.Name}'. Trait names must be unique within a race.");
                }
            }

            // 2. Validate Alternative Racial Traits
            if (definition.AlternativeRacialTraits != null)
            {
                foreach (var alt in definition.AlternativeRacialTraits)
                {
                    if (string.IsNullOrWhiteSpace(alt.Name))
                        errors.Add("An alternative racial trait is missing a name.");
                    else if (!seenTraitNames.Add(alt.Name))
                        errors.Add($"Duplicate alternative trait name found: '{alt.Name}'. Trait names must be unique within a race.");

                    if (alt.ReplacesRacialTraits == null || alt.ReplacesRacialTraits.Count == 0)
                        errors.Add($"Alternative trait '{alt.Name}' must specify at least one trait it replaces.");
                }
            }

            // 3. Validate Subraces
            if (definition.Subraces != null)
            {
                var seenSubraceNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var subrace in definition.Subraces)
                {
                    if (string.IsNullOrWhiteSpace(subrace.Name))
                        errors.Add("A subrace is missing a name.");
                    else if (!seenSubraceNames.Add(subrace.Name))
                        errors.Add($"Duplicate subrace name found: '{subrace.Name}'.");

                    // Validate traits inside the subrace
                    if (subrace.RacialTraits != null)
                    {
                        var seenSubraceTraitNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        foreach (var subTrait in subrace.RacialTraits)
                        {
                            if (string.IsNullOrWhiteSpace(subTrait.Name))
                                errors.Add($"A trait in subrace '{subrace.Name}' is missing a name.");
                            else if (!seenSubraceTraitNames.Add(subTrait.Name))
                                errors.Add($"Duplicate trait '{subTrait.Name}' found in subrace '{subrace.Name}'.");
                        }
                    }
                }
            }

            return (errors.Count == 0, errors);
        }
    }
}
