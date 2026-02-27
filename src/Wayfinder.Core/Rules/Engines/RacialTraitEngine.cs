using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Extensions;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Results;

namespace Wayfinder.Core.Rules.Engines;

/// <summary>
/// Assemble a validated Race object from provided definitions
/// </summary>

public static class RacialTraitEngine
{
    public static RaceResolutionResult Resolve(
            RaceDefinition baseRace,
            Subrace? activeSubrace,
            List<AlternativeRacialTrait> selectedAlternatives)
    {
        var result = new RaceResolutionResult();

        // Use a dictionary for fast, case-insensitive lookups and replacements
        var activeRacialTraits = baseRace.DefaultRacialTraits
            .ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);

        // 1. Apply Subrace first (Subraces are "Bundles" of replacements)
        if (activeSubrace != null)
        {
            ApplySubrace(activeRacialTraits, activeSubrace, result);
        }

        // 2. Apply individual alternative racial traits
        if (selectedAlternatives.Any())
        {
            ApplyAlternativeRacialTraits(activeRacialTraits, selectedAlternatives, result);
        }

        // 3. Output the final assembled list
        result.ActiveRacialTraits = activeRacialTraits.Values.ToList();
        return result;
    }

    /// <summary>
    /// Extracted for isolated Unit Testing. Modifies the active traits dictionary.
    /// </summary>
    public static void ApplySubrace(Dictionary<string, RacialTrait> activeRacialTraits, Subrace subrace, RaceResolutionResult result) => ApplyReplacements(activeRacialTraits, subrace.RacialTraits, result, subrace.Name);

    /// <summary>
    /// Extracted for isolated Unit Testing.
    /// </summary>
    public static void ApplyAlternativeRacialTraits(Dictionary<string, RacialTrait> activeRacialTraits, List<AlternativeRacialTrait> alternatives, RaceResolutionResult result) => ApplyReplacements(activeRacialTraits, alternatives, result, "Manual Selection");

    // Shared internal logic for doing the actual swapping
    private static void ApplyReplacements(
        Dictionary<string, RacialTrait> activeRacialTraits,
        IEnumerable<AlternativeRacialTrait> incomingAlternatives,
        RaceResolutionResult result,
        string sourceName)
    {
        foreach (var altTrait in incomingAlternatives)
        {
            // Verify we have all prerequisites to replace
            bool canReplace = true;
            foreach (var reqName in altTrait.ReplacesRacialTraits)
            {
                var key = activeRacialTraits.Keys.FirstOrDefault(k => k.EqualsIgnoreCase(reqName));
                if (key == null)
                {
                    result.Errors.Add($"[{sourceName}] Cannot apply '{altTrait.Name}'. It requires replacing '{reqName}', which is no longer available.");
                    canReplace = false;
                }
            }

            // If valid, remove the old traits and add the new one
            if (canReplace)
            {
                foreach (var reqName in altTrait.ReplacesRacialTraits)
                {
                    var key = activeRacialTraits.Keys.First(k => k.EqualsIgnoreCase(reqName));
                    activeRacialTraits.Remove(key);
                }
                activeRacialTraits[altTrait.Name] = altTrait;
            }
        }
    }

    /// <summary>
    /// Calculates which alternative traits are still legally selectable based on the currently applied traits.
    /// </summary>
    public static IEnumerable<AlternativeRacialTrait> GetAvailableAlternatives(RaceDefinition baseRace, Subrace? activeSubrace, List<AlternativeRacialTrait> selectedAlternatives)
    {
        // 1. Calculate the current state of traits based ONLY on the provided inputs
        var currentResolution = Resolve(baseRace, activeSubrace, selectedAlternatives);

        // Create a fast lookup list of the currently active trait names
        var activeTraitNames = currentResolution.ActiveRacialTraits
            .Select(t => t.Name)
            .ToList();

        var validOptions = new List<AlternativeRacialTrait>();

        // 2. Evaluate what remaining alternative traits we can still afford
        foreach (var alt in baseRace.AlternativeRacialTraits)
        {
            // Skip traits that are already selected (the UI will handle displaying them separately)
            if (selectedAlternatives.Any(s => s.Name.EqualsIgnoreCase(alt.Name)))
                continue;

            // An alternative is valid ONLY IF every trait it requires replacing is currently active
            bool canAfford = alt.ReplacesRacialTraits.All(req =>
                activeTraitNames.Any(active => active.EqualsIgnoreCase(req)));

            if (canAfford)
            {
                validOptions.Add(alt);
            }
        }

        return validOptions;
    }
}
