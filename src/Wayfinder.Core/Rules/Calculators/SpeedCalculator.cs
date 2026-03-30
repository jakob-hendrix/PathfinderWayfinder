namespace Wayfinder.Core.Rules.Calculators;

using System.Collections.Generic;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DomainModels.Stats;
using Wayfinder.Core.Logic;

public static class SpeedCalculator
{
    public static ModifiableStat CalculateLandSpeed(IEnumerable<ActiveEffect> globalEffects, EncumbranceLevel encumbrance)
    {
        // 1. Calculate the Raw Speed (Race + Spells + Feats)
        var rawStat = StatCalculator.Calculate(StatNames.LandSpeed, 0, globalEffects);
        int rawSpeed = rawStat.Total;

        int penalty = 0;
        string penaltyName = string.Empty;

        // 2. Apply Encumbrance Math (2/3 rounded up to the nearest 5)
        if (encumbrance == EncumbranceLevel.Medium || encumbrance == EncumbranceLevel.Heavy)
        {
            int encumberedSpeed = CalculateEncumberedSpeed(rawSpeed);
            penalty = encumberedSpeed - rawSpeed; // e.g., 20 - 30 = -10
            penaltyName = "Encumbrance";
        }
        else if (encumbrance == EncumbranceLevel.Overloaded)
        {
            // Overloaded characters can only stagger 5ft as a full-round action
            penalty = 5 - rawSpeed;
            penaltyName = "Overloaded";
        }

        // 3. Return a new immutable ModifiableStat if a penalty applies
        if (penalty < 0)
        {
            var updatedModifiers = rawStat.Modifiers.ToList();

            // Note: Assuming your StatModifier constructor takes (SourceName, Value, Type, IsApplied)
            updatedModifiers.Add(new StatModifier(penaltyName, penalty, ModifierType.Penalty, true));

            return new ModifiableStat
            {
                Name = rawStat.Name,
                Modifiers = updatedModifiers
            };
        }

        return rawStat;
    }

    /// <summary>
    /// Calculates speed reduction: 2/3 of base speed, rounded UP to the nearest 5ft multiple.
    /// Minimum speed is 5ft unless raw speed was 0.
    /// </summary>
    private static int CalculateEncumberedSpeed(int rawSpeed)
    {
        if (rawSpeed <= 0) return 0;

        double reduced = rawSpeed * (2.0 / 3.0);
        int roundedToNearest5 = (int)(Math.Ceiling(reduced / 5.0) * 5.0);

        return Math.Max(5, roundedToNearest5);
    }

    public static int CalculateRunMultiplier(EncumbranceLevel encumbrance)
    {
        // Standard run is 4x. Heavy/Overloaded drops it to 3x.
        // Note: If you add the "Run" feat later (5x), you'll pass that in here to modify the base.
        if (encumbrance == EncumbranceLevel.Heavy || encumbrance == EncumbranceLevel.Overloaded)
        {
            return 3;
        }
        return 4;
    }

    public static ModifiableStat CalculateFlySpeed(IEnumerable<ActiveEffect> globalEffects)
        => StatCalculator.Calculate(StatNames.FlySpeed, 0, globalEffects);

    public static ModifiableStat CalculateClimbSpeed(IEnumerable<ActiveEffect> globalEffects)
        => StatCalculator.Calculate(StatNames.ClimbSpeed, 0, globalEffects);

    public static ModifiableStat CalculateSwimSpeed(IEnumerable<ActiveEffect> globalEffects)
        => StatCalculator.Calculate(StatNames.SwimSpeed, 0, globalEffects);
}
