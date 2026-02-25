namespace Wayfinder.Core.DomainModels.Characters.RaceModels
{
    public class AlternativeRacialTrait : RacialTrait
    {
        public List<string> ReplacesTraitNames { get; init; } = new();
    }
}
