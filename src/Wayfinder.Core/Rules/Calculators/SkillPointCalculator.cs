using Wayfinder.Core.Enums;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Rules.Calculators;

namespace Wayfinder.Core.Logic;

public static class SkillPointCalculator
{
    /// <summary>
    /// Calculates the standard skill points gained for a specific level.
    /// </summary>
    public static int CalculateStandardSkillPoints(HydratedClassLevel level, int intScore)
    {
        if (level?.ClassDefinition == null) return 0;

        int intModifier = AbilityScoreCalculator.CalculateModifier(intScore);

        // 1. Base + Int Mod
        int points = level.ClassDefinition.SkillPointsPerLevel + intModifier;

        // 2. Minimum 1 point per level rule (applied before FCB or racial bonuses)
        if (points < 1)
        {
            points = 1;
        }

        // 3. Add Favored Class Bonus if applicable
        if (level.AppliedFavoredClassBonus == FavoredClassBonus.SkillPoint)
        {
            points += 1;
        }

        // TODO: add racial bonus

        return points;
    }

    /// <summary>
    /// Calculates the background skill points gained for a specific level.
    /// </summary>
    public static int CalculateBackgroundSkillPoints()
    {
        // Under the Background Skills alternate rules, this is always a flat 2.
        // It is unaffected by Intelligence or Favored Class Bonuses.
        return 2;
    }
}
