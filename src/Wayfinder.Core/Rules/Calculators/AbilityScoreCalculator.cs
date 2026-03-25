namespace Wayfinder.Core.Rules.Calculators;

using System;
using System.Collections.Generic;
using System.Linq;
using Wayfinder.Core.Constants; // Assuming AbilityScore enum lives here
using Wayfinder.Core.DomainModels.Stats;
using Wayfinder.Core.Logic;
using Wayfinder.Core.Models.Characters;

public static class AbilityScoreCalculator
{
    // The core Pathfinder modifier math (-1 for 8, +0 for 10, +1 for 12, etc.)
    public static int CalculateModifier(int score)
    {
        return (int)Math.Floor((score - 10) / 2.0);
    }

    public static int CalculateModifier(ModifiableStat? stat)
    {
        int score = stat?.Total ?? 10;
        return CalculateModifier(score);
    }

    /// <summary>
    /// Builds the ModifiableStat for an Ability Score, accounting for base values, 
    /// level advancements, and all global magic/buff effects.
    /// </summary>
    public static ModifiableStat CalculateAbilityScore(
        string statName,
        int baseScore,
        IEnumerable<HydratedClassLevel>? levels,
        IEnumerable<ActiveEffect> globalEffects)
    {
        var baseModifiers = new List<StatModifier>();
        AbilityScore scoreType = PathfinderEnumMapper.ToAbilityScore(statName);

        // 1. Calculate and log level-up bumps (4th, 8th, 12th, 16th, 20th)
        if (levels != null)
        {
            int levelBumps = levels.Count(l =>
                l.GrantsAbilityScoreIncrease &&
                l.IncreasedAbilityScore == scoreType);

            if (levelBumps > 0)
            {
                // We add this as an Untyped modifier so it permanently stacks with everything
                baseModifiers.Add(new StatModifier("Level Advancement", levelBumps, ModifierType.Untyped, true));
            }
        }

        // 2. Let the universal pipeline handle the rest (racial bonuses, belts, spells)
        return StatCalculator.Calculate(
            statName: statName,
            baseValue: baseScore,
            globalEffects: globalEffects,
            baseModifiers: baseModifiers
        );
    }
}
