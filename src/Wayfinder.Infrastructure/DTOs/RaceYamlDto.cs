namespace Wayfinder.Infrastructure.DTOs
{
    public class RaceYamlDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CreatureType { get; set; } = "Humanoid";
        public List<string> Subtypes { get; set; } = new();

        public List<RacialTraitYamlDto> DefaultRacialTraits { get; set; } = new();
        public List<AlternativeRacialTraitYamlDto> AlternativeRacialTraits { get; set; } = new();
        public List<SubraceYamlDto> Subraces { get; set; } = new();
    }
}
