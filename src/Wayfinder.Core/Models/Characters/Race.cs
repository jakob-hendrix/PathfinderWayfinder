using Wayfinder.Core.DataDefinitions;

namespace Wayfinder.Core.Models.Characters;

public class Race
{
    public RaceDefinition RaceDefinition { get; set; } = null;
    public Subrace? Subrace { get; set; }

    public List<AlternativeRacialTrait> ActiveAlternativeRacialTraits { get; set; } = new();
    public List<RacialTrait> SelectedRacialTraits { get; set; } = new();

}
