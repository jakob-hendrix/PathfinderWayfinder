using Wayfinder.Core.Data.Definitions;
using Wayfinder.Core.DomainModels.Characters.Race;

namespace Wayfinder.Core.Rules.Services
{
    /// <summary>
    /// Assemble a validated Race object from provided definitions
    /// </summary>

    public static class RacialTraitEngine
    {
        public static RaceResolutionResult Resolve(
            RaceDefinition baseRace,
            Subrace? chosenSubrace,
            List<AlternativeRacialTrait> chosenAlternatives)
        {
            var result = new RaceResolutionResult();
            var activeDefaults = baseRace.DefaultRacialTraits.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);
            var replacedTraitNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var finalTraits = new List<RacialTrait>();
            var validAlternatives = new List<AlternativeRacialTrait>();

            if (chosenSubrace != null)
                ApplyReplacements(chosenSubrace.Traits, activeDefaults, replacedTraitNames, finalTraits, validAlternatives, result);

            ApplyReplacements(chosenAlternatives, activeDefaults, replacedTraitNames, finalTraits, validAlternatives, result);

            if (result.IsValid)
            {
                finalTraits.AddRange(activeDefaults.Values);
                result.HydratedRace = new Race
                {
                    RaceDefinition = baseRace,
                    Subrace = chosenSubrace,
                    ActiveAlternativeRacialTraits = validAlternatives,
                    SelectedRacialTraits = finalTraits
                };
            }

            return result;
        }

        /// <summary>
        /// Calculates which alternative traits are still legally selectable based on the currently applied traits.
        /// </summary>
        public static List<AlternativeRacialTrait> GetAvailableAlternatives(
            RaceDefinition baseRace,
            Subrace? chosenSubrace,
            List<AlternativeRacialTrait> currentlyChosen)
        {
            // 1. Resolve the current state to see what defaults survived
            var currentResolution = Resolve(baseRace, chosenSubrace, currentlyChosen);

            if (!currentResolution.IsValid || currentResolution.HydratedRace == null)
                return new List<AlternativeRacialTrait>();

            // 2. Map the names of traits that are currently active
            var activeTraitNames = currentResolution.HydratedRace.SelectedRacialTraits
                .Select(t => t.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var availableAlts = new List<AlternativeRacialTrait>();

            // 3. Check the remaining alternative traits in the base definition
            foreach (var alt in baseRace.AlternativeRacialTraits)
            {
                // Skip if the user has already selected this exact alternative trait
                if (currentlyChosen.Any(c => c.Id == alt.Id)) continue;

                // An alternative is valid ONLY if every trait it replaces is currently active in the pool
                if (alt.ReplacesTraitNames.All(name => activeTraitNames.Contains(name)))
                {
                    availableAlts.Add(alt);
                }
            }

            return availableAlts;
        }

        private static void ApplyReplacements(
            IEnumerable<AlternativeRacialTrait> traitsToAdd,
            Dictionary<string, RacialTrait> activeDefaults,
            HashSet<string> replacedTraitNames,
            List<RacialTrait> finalTraits,
            List<AlternativeRacialTrait> validAlternatives,
            RaceResolutionResult result)
        {
            foreach (var alt in traitsToAdd)
            {
                bool canApply = true;

                foreach (var replaceName in alt.ReplacesTraitNames)
                {
                    if (replacedTraitNames.Contains(replaceName))
                    {
                        result.Errors.Add($"Conflict: '{alt.Name}' tried to replace '{replaceName}', but it was already replaced.");
                        canApply = false;
                    }
                    else if (!activeDefaults.ContainsKey(replaceName))
                    {
                        result.Errors.Add($"Invalid: '{alt.Name}' tried to replace '{replaceName}', which is not a currently available base trait.");
                        canApply = false;
                    }
                }

                if (canApply)
                {
                    foreach (var replaceName in alt.ReplacesTraitNames)
                    {
                        activeDefaults.Remove(replaceName);
                        replacedTraitNames.Add(replaceName);
                    }

                    finalTraits.Add(alt); // Direct addition since it inherits RacialTrait
                    validAlternatives.Add(alt);
                }
            }
        }
    }
}
