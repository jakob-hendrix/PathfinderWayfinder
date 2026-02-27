namespace Wayfinder.Core.Models.Characters;

public class AlternativeRacialTrait : RacialTrait
{
    public List<string> ReplacesRacialTraits { get; init; } = new();
}
