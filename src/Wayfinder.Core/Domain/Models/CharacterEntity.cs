using Wayfinder.Core.Domain.Constants;

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

        public Dictionary<AbilityScore, int> BaseAbilityScores { get; set; } = new()
        {
            { AbilityScore.Strength, 10 },
            { AbilityScore.Dexterity, 10 },
            { AbilityScore.Constitution, 10 },
            { AbilityScore.Intelligence, 10 },
            { AbilityScore.Wisdom, 10 },
            { AbilityScore.Charisma, 10 }
        };
    }
}
