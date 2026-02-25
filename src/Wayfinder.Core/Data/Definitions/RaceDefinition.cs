using Wayfinder.Core.DomainModels.Characters.RaceModels;

namespace Wayfinder.Core.Data.Definitions
{
    public class RaceDefinition
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string CreatureType { get; init; } = "Humanoid";
        public List<string> Subtypes { get; init; } = new();
        public List<Subrace> Subraces { get; init; } = new();
        public List<RacialTrait> DefaultRacialTraits { get; init; } = new();
        public List<AlternativeRacialTrait> AlternativeRacialTraits { get; init; } = new();
    }
}
