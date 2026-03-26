using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Results;
using Wayfinder.Core.Rules.Engines;

namespace Wayfinder.Core.Factories;

public class RaceFactory : IRaceFactory
{
    private readonly IRaceLibrary _raceLibrary;

    public RaceFactory(IRaceLibrary raceLibrary)
    {
        _raceLibrary = raceLibrary;
    }

    public RaceResolutionResult BuildRace(RaceChoices choices)
    {
        var result = new RaceResolutionResult();

        RaceDefinition? baseDef = null;


        if (string.IsNullOrWhiteSpace(choices.RaceName))
        {
            result.Errors.Add($"No race name provided");
            return result;
        }

        baseDef = _raceLibrary.GetRaceDefinition(choices.RaceName);
        if (baseDef == null)
        {
            result.Errors.Add($"Race definition '{choices.RaceName}' not found.");
            return result;
        }

        // 2. Resolve the Subrace (if selected)
        Subrace? subDef = null;
        if (!string.IsNullOrWhiteSpace(choices.SubraceName))
        {
            subDef = baseDef.Subraces.FirstOrDefault(s => s.Name == choices.SubraceName);
            if (subDef == null)
            {
                result.Errors.Add($"Subrace definition '{choices.SubraceName}' not found.");
                return result;
            }
        }

        // 3. Resolve Alternative RacialTraits
        var selectedAlts = baseDef.AlternativeRacialTraits
            .Where(alt => choices.SelectedAlternativeTraits.Contains(alt.Name))
            .ToList();

        // 4. Delegate to the Engine for the heavy lifting
        var resolution = RacialTraitEngine.Resolve(baseDef, subDef, selectedAlts);

        // 5. Finalize the Assembled Object
        if (resolution.IsValid)
        {
            resolution.HydratedRace = new HydratedRace
            {
                RaceDefinition = baseDef,
                Subrace = subDef,
                ActiveAlternativeRacialTraits = selectedAlts,
                SelectedRacialTraits = resolution.ActiveRacialTraits
            };
        }

        return resolution;
    }
}
