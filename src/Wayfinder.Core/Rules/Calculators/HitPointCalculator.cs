using Wayfinder.Core.Constants;
using Wayfinder.Core.Models.Characters;

namespace Wayfinder.Core.Rules.Calculators;

public static class HitPointCalculator
{
    public static int CalculateMaxHp(IEnumerable<HydratedClassLevel>? levels, int currentConstitution)
    {
        if (levels == null || !levels.Any()) return 0;

        int maxHp = 0;
        int conModifier = AbilityScoreCalculator.CalculateModifier(currentConstitution);

        // Apply hp gains from levels
        foreach (var level in levels)
        {
            int levelHp = level.HpGained + conModifier;
            if (levelHp < 1) levelHp = 1; // Minimum of 1 HP per level

            if (level.AppliedFavoredClassBonus == FavoredClassBonus.HitPoint)
                levelHp += 1; // Add 1 HP for favored class bonus

            maxHp += levelHp;
        }

        return maxHp;
    }
}
