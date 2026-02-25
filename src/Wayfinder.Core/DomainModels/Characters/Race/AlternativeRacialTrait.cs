namespace Wayfinder.Core.DomainModels.Characters.Race
{
    public class AlternativeRacialTrait : RacialTrait
    {
        public List<string> ReplacesTraitNames { get; init; } = new();
    }
}
