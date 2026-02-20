namespace Wayfinder.Core.Domain.Models
{
    public class CharacterEntity
    {
        public string? Name { get; set; } = string.Empty;
        public string? Gender { get; set; }

        public Race? Race { get; set; }

        // Classes
        public List<ClassLevel> ClassLevels { get; set; } = new();

        public int BaseStrength { get; set; } = 10;
        public int BaseDexterity { get; set; } = 10;
        public int BaseConstitution { get; set; } = 10;
        public int BaseIntelligence { get; set; } = 10;
        public int BaseWisdom { get; set; } = 10;
        public int BaseCharisma { get; set; } = 10;
    }
}
