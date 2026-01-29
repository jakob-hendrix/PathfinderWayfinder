using Wayfinder.Core.Domain.Constants;

namespace Wayfinder.Core.Domain.Models
{
    public class Race
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public CreatureSize Size { get; set; } = CreatureSize.Medium;
        public int BaseSpeed { get; set; } = 30;
        public string CreatureType { get; set; } = string.Empty;
        public List<string> SubTypes { get; set; } = new();
        public List<AbilityModifier> AbilityModifiers { get; set; } = new();
    }

    public class AbilityModifier
    {
        public AbilityScore Ability { get; set; }
        public int Value { get; set; }
    }
}
