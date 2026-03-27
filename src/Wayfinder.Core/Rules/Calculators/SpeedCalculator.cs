namespace Wayfinder.Core.Rules.Calculators;

using System.Collections.Generic;
using Wayfinder.Core.DomainModels.Stats;
using Wayfinder.Core.Logic;

public static class SpeedCalculator
{
    public static ModifiableStat CalculateLandSpeed(IEnumerable<ActiveEffect> globalEffects)
    {
        // 1. Get the base speed and any standard magical buffs (e.g., Haste, Expeditious Retreat)
        var speedStat = StatCalculator.Calculate(StatNames.LandSpeed, 0, globalEffects);

        // 2. TODO: Apply Armor and Encumbrance Penalties
        // If (wearing medium/heavy armor) AND (does not have "Slow and Steady" effect)
        // { 
        //     int penalty = CalculateArmorSpeedPenalty(speedStat.Total);
        //     speedStat.AddModifier(new StatModifier("Armor Encumbrance", penalty, ModifierType.Penalty, true));
        // }

        // 3. Minimum Speed Rule: Speed cannot be reduced below 5ft by armor/encumbrance. At this point
        // you may move 5 ft via full round action
        if (speedStat.Total < 5 && speedStat.Total > 0)
        {
            // Optional: You could add a hard floor adjustment here if you want to strictly enforce it
        }

        return speedStat;
    }

    public static ModifiableStat CalculateFlySpeed(IEnumerable<ActiveEffect> globalEffects)
        => StatCalculator.Calculate(StatNames.FlySpeed, 0, globalEffects);

    public static ModifiableStat CalculateClimbSpeed(IEnumerable<ActiveEffect> globalEffects)
        => StatCalculator.Calculate(StatNames.ClimbSpeed, 0, globalEffects);

    public static ModifiableStat CalculateSwimSpeed(IEnumerable<ActiveEffect> globalEffects)
        => StatCalculator.Calculate(StatNames.SwimSpeed, 0, globalEffects);
}
