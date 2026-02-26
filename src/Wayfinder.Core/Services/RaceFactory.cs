using Wayfinder.Core.Data.Definitions;
using Wayfinder.Core.DomainModels.Characters.RaceModels;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Rules.Services;
using Wayfinder.Core.Services;

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
        RaceDefinition baseDef = null;

        try
        {
            baseDef = _raceLibrary.GetRaceDefinition(choices.RaceName);
        }
        catch (KeyNotFoundException ex)
        {
            return new RaceResolutionResult { Errors = { $"Race definition '{choices.RaceName}' not found." } };
        }

        //if (baseDef == null)
        //{
        //    return new RaceResolutionResult { Errors = { $"Race definition '{choices.RaceDefinitionId}' not found." } };
        //}

        // 2. Resolve the Subrace (if selected)
        Subrace? subDef = null;
        if (!string.IsNullOrWhiteSpace(choices.SubraceName))
        {
            subDef = baseDef.Subraces.FirstOrDefault(s => s.Id == choices.SubraceName);
        }

        // 3. Resolve Alternative Traits
        var selectedAlts = baseDef.AlternativeRacialTraits
            .Where(alt => choices.SelectedAlternativeTraitIds.Contains(alt.Id))
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
                SelectedRacialTraits = resolution.ActiveTraits
            };
        }

        return resolution;
    }
}
