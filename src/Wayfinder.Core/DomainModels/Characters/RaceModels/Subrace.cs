namespace Wayfinder.Core.DomainModels.Characters.RaceModels
{
    public class Subrace
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<AlternativeRacialTrait> Traits { get; init; } = new();
    }
}
