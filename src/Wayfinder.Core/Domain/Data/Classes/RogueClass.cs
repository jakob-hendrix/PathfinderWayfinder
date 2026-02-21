using Wayfinder.Core.Domain.Constants;
using Wayfinder.Core.Domain.Models.Characters;
using Wayfinder.Core.Domain.Models.Common;

namespace Wayfinder.Core.Domain.Data.Classes
{
    public class RogueClass : BaseCharacterClass
    {
        public override string Name => "Rogue";

        public override string Description => string.Empty;

        public override int HitDie => 8;

        public override int SkillPointsPerLevel => 8;

        public override BabProgressionRate BabRate => BabProgressionRate.Medium;

        public override SaveProgressionRate FortitudeRate => SaveProgressionRate.Slow;

        public override SaveProgressionRate WillRate => SaveProgressionRate.Slow;

        public override SaveProgressionRate ReflexRate => SaveProgressionRate.Fast;

        public override void ApplyClassFeature(int level, CharacterSheet sheet, List<Bonus> bonuses)
        {
            // https://www.d20pfsrd.com/classes/core-classes/rogue/

        }
    }
}
