using Wayfinder.Core.DataDefinitions;

namespace Wayfinder.Core.Models.Characters;

public class HydratedRace
{
    public string Name => RaceDefinition.Name;

    public required RaceDefinition RaceDefinition { get; set; }
    public Subrace? Subrace { get; set; }

    public List<AlternativeRacialTrait> ActiveAlternativeRacialTraits { get; set; } = new();
    public List<RacialTrait> SelectedRacialTraits { get; set; } = new();
    public List<ActiveEffect> AddedActiveEffects { get; set; } = new();

}
