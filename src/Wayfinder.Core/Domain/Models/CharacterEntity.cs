namespace Wayfinder.Core.Domain.Models
{
    public class CharacterEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Gender { get; set; }

        // Race. Foreign Key for EF
        public Guid RaceId { get; set; }
        public Race? Race { get; set; }

        // Classes TODO:

        public int BaseStrength { get; set; } = 10;
        public int BaseDexterity { get; set; } = 10;
        public int BaseConstitution { get; set; } = 10;
        public int BaseIntelligence { get; set; } = 10;
        public int BaseWisdom { get; set; } = 10;
        public int BaseCharisma { get; set; } = 10;
    }
}
