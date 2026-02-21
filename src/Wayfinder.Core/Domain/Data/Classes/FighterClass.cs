using Wayfinder.Core.Domain.Constants;
using Wayfinder.Core.Domain.Models.Characters;
using Wayfinder.Core.Domain.Models.Common;

namespace Wayfinder.Core.Domain.Data.Classes
{
    public class FighterClass : BaseCharacterClass
    {
        public override string Name => "Fighter";

        public override string Description => "TODO";

        public override int HitDie => 10;

        public override int SkillPointsPerLevel => 2;

        public override BabProgressionRate BabRate => BabProgressionRate.Fast;

        public override SaveProgressionRate FortitudeRate => SaveProgressionRate.Fast;

        public override SaveProgressionRate WillRate => SaveProgressionRate.Slow;

        public override SaveProgressionRate ReflexRate => SaveProgressionRate.Slow;

        public override void ApplyClassFeature(int level, CharacterSheet sheet, List<Bonus> bonuses)
        {
            // https://www.d20pfsrd.com/classes/core-classes/fighter/
            // 1 - Bonus Feat
            // 2 - Bonus Feat, Bravery +1

            if (level >= 2)
            {
                int braveryBonus = 1 + ((level - 2) / 4);
                // TODO: need to flesh out 
            }

            // 3 - Armor Training
            // 4 - Bonus Feat
            // 5 - Weapon Training
            // 6 - Bonus Feat, Bravery +2
            // 7 - Armor Training
            // 8 - Bonus Feat
            // 9 - Weapon Training or Advanced Weapon Training
            // 10 - Bonus Feat, Bravery +3
            // 11 - Armor Training
            // 12 - Bonus Feat
            // 13 - Weapon Training or Advanced Weapon Training
            // 14 - Bonus Feat, Bravery +4
            // 15 - Armor Training
            // 16 - Bonus Feat
            // 17 - Weapon Training or Advanced Weapon Training
            // 18 - Bonus Feat, Bravery +5
            // 19 - Armor Mastery
            // 20 - Bonus Feat, Capstone (Weapon Mastery)

        }
    }
}
