using Wayfinder.Core.Enums;

namespace Wayfinder.Core.DomainModels.Characters
{
    public class Race
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public CreatureSize Size { get; set; } = CreatureSize.Medium;
        public int BaseSpeed { get; set; } = 30;
        public string CreatureType { get; set; } = string.Empty;
        public List<string> CreatureSubTypes { get; set; } = new();
        public List<AbilityModifier> AbilityModifiers { get; set; } = new();
    }
}
