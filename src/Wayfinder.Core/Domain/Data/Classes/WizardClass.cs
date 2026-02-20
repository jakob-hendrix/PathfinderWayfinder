using Wayfinder.Core.Domain.Constants;
using Wayfinder.Core.Domain.Models;

namespace Wayfinder.Core.Domain.Data.Classes
{
    public class WizardClass : BaseCharacterClass
    {
        public override string Name => "Wizard";

        public override string Description => string.Empty;

        public override int HitDie => 6;

        public override int SkillPointsPerLevel => 2;

        public override BabProgressionRate BabRate => BabProgressionRate.Slow;

        public override SaveProgressionRate FortitudeRate => SaveProgressionRate.Slow;

        public override SaveProgressionRate WillRate => SaveProgressionRate.Fast;

        public override SaveProgressionRate ReflexRate => SaveProgressionRate.Slow;

        public override void ApplyClassFeature(int level, CharacterSheet sheet, List<Bonus> bonuses)
        {
            // https://www.d20pfsrd.com/classes/core-classes/wizard/
        }
    }
}
