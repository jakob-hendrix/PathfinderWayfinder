using Wayfinder.Core.Domain.Constants;

namespace Wayfinder.Core.Domain.Models
{
    public abstract class BaseCharacterClass
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract int HitDie { get; }
        public abstract int SkillPointsPerLevel { get; }
        public abstract BabProgressionRate BabRate { get; }
        public abstract SaveProgressionRate FortitudeRate { get; }
        public abstract SaveProgressionRate WillRate { get; }
        public abstract SaveProgressionRate ReflexRate { get; }

        // Hook for behavioral features
        public virtual void ApplyClassFeature(int level, CharacterSheet sheet, List<Bonus> bonuses)
        {
            // Default, no class features 
        }
    }
}
