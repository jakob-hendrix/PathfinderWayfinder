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
        // 1. Resolve the Base Definition
        RaceDefinition? baseDef = null;

        try
        {
            baseDef = _raceLibrary.GetRaceDefinition(choices.RaceName);
        }
        catch (KeyNotFoundException ex)
        {
            return new RaceResolutionResult { Errors = { $"Race definition '{choices.RaceName}' not found." } };
        }

        // 2. Resolve the Subrace (if selected)
        Subrace? subDef = null;
        if (!string.IsNullOrWhiteSpace(choices.SubraceName))
        {
            try
            {
                subDef = baseDef.Subraces.FirstOrDefault(s => s.Name == choices.SubraceName);
            }
            catch (KeyNotFoundException ex)
            {

                return new RaceResolutionResult { Errors = { $"Subrace definition '{choices.SubraceName}' not found." } };
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
            resolution.HydratedRace = new Race
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
