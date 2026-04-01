using Wayfinder.Core.Constants;
using Wayfinder.Core.DomainModels.Stats;

namespace Wayfinder.Core.Rules.Calculators;

public static class CarryCapacityCalculator
{
    // The official Pathfinder Heavy Load limits for STR 0-19
    private static readonly int[] HeavyLoadBase =
    {
        0,   10,  20,  30,  40,  50,  60,  70,  80,  90,  // 0-9
        100, 115, 130, 150, 175, 200, 230, 260, 300, 350  // 10-19
    };

    public static int GetMaxCarryingCapacity(int strengthScore, SizeCategory size = SizeCategory.Medium, int legs = 2)
    {
        if (strengthScore <= 0) return 0;

        int baseCapacity;
        if (strengthScore < 20)
        {
            baseCapacity = HeavyLoadBase[strengthScore];
        }
        else
        {
            // For scores 20+, find the base value from the 10-19 range and apply the x4 multiplier
            int baseIndex = 10 + (strengthScore % 10);
            int multiplierPower = (strengthScore / 10) - 1;
            baseCapacity = HeavyLoadBase[baseIndex] * (int)Math.Pow(4, multiplierPower);
        }

        double sizeLegMultiplier = GetSizeAndLegMultiplier(size, legs);

        // Truncate decimals as per standard Pathfinder rounding rules for weights
        return (int)(baseCapacity * sizeLegMultiplier);
    }

    private static double GetSizeAndLegMultiplier(SizeCategory size, int legs)
    {
        // TODO - what if you have 8 legs? 1000?
        bool isQuadruped = legs >= 4;

        return size switch
        {
            SizeCategory.Fine => isQuadruped ? 0.25 : 0.125,
            SizeCategory.Diminutive => isQuadruped ? 0.5 : 0.25,
            SizeCategory.Tiny => isQuadruped ? 0.75 : 0.5,
            SizeCategory.Small => isQuadruped ? 1.0 : 0.75,
            SizeCategory.Medium => isQuadruped ? 1.5 : 1.0,
            SizeCategory.Large => isQuadruped ? 3.0 : 2.0,
            SizeCategory.Huge => isQuadruped ? 6.0 : 4.0,
            SizeCategory.Gargantuan => isQuadruped ? 12.0 : 8.0,
            SizeCategory.Colossal => isQuadruped ? 24.0 : 16.0,
            _ => 1.0
        };
    }

    public static EncumbranceLevel GetEncumbranceLevel(int maxCapacity, double totalWeight)
    {
        double lightLimit = Math.Floor(maxCapacity / 3.0);
        double mediumLimit = Math.Floor((maxCapacity * 2.0) / 3.0);

        if (totalWeight <= lightLimit) return EncumbranceLevel.Light;
        if (totalWeight <= mediumLimit) return EncumbranceLevel.Medium;
        if (totalWeight <= maxCapacity) return EncumbranceLevel.Heavy;

        return EncumbranceLevel.Overloaded;
    }

    /// <summary>
    /// Generates only the static numeric penalties (Max Dex and ACP). 
    /// Speed and Run multipliers are handled dynamically by the SpeedCalculator.
    /// </summary>
    public static IEnumerable<ActiveEffect> GenerateEncumbranceEffects(EncumbranceLevel level)
    {
        var effects = new List<ActiveEffect>();

        if (level == EncumbranceLevel.Medium)
        {
            effects.Add(new ActiveEffect { TargetStatName = StatNames.MaxDexBonus, Value = 3, Type = ModifierType.Untyped, Category = EffectCategory.Encumbrance, SourceName = "Medium Load" });
            effects.Add(new ActiveEffect { TargetStatName = StatNames.ACP, Value = -3, Type = ModifierType.Penalty, Category = EffectCategory.Encumbrance, SourceName = "Medium Load" });
        }
        else if (level == EncumbranceLevel.Heavy || level == EncumbranceLevel.Overloaded)
        {
            effects.Add(new ActiveEffect { TargetStatName = StatNames.MaxDexBonus, Value = 1, Type = ModifierType.Untyped, Category = EffectCategory.Encumbrance, SourceName = "Heavy Load" });
            effects.Add(new ActiveEffect { TargetStatName = StatNames.ACP, Value = -6, Type = ModifierType.Penalty, Category = EffectCategory.Encumbrance, SourceName = "Heavy Load" });
        }

        return effects;
    }
}
