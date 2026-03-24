namespace Wayfinder.Core.Logic;

using System.Collections.Generic;
using System.Linq;
using Wayfinder.Core.DomainModels.Stats;
using Wayfinder.Core.Enums;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Rules.Calculators;

public static class SaveCalculator
{
    /// <summary>
    /// Calculates the raw base save from class levels using Pathfinder 1e math.
    /// Publicly exposed for rigorous unit testing.
    /// </summary>
    public static int CalculateBaseSave(IEnumerable<HydratedClassLevel> classLevels, StatType statType)
    {
        EnsureValidSaveType(statType);

        // 1. Group levels by Class Definition to calculate the math PER CLASS.
        // E.g., A Fighter 2 / Rogue 1 will have two groups here.
        var levelsByClass = classLevels.GroupBy(l => l.ClassDefinition);

        int totalBaseSave = 0;

        foreach (var classGroup in levelsByClass)
        {
            var classDef = classGroup.Key;

            // The total number of levels the character has in THIS specific class
            int classLevel = classGroup.Count();

            // Figure out if this class has a Fast or Slow progression for this specific save
            SaveProgressionRate progression = statType switch
            {
                StatType.Fortitude => classDef.FortitudeRate,
                StatType.Reflex => classDef.ReflexRate,
                StatType.Will => classDef.WillRate,
                _ => throw new InvalidOperationException("Unreachable code reached.")
            };

            // Apply PF1e Math
            if (progression == SaveProgressionRate.Fast)
            {
                // Fast: Base +2, plus 1 for every 2 levels
                totalBaseSave += 2 + (classLevel / 2);
            }
            else
            {
                // Slow: 1 for every 3 levels
                totalBaseSave += classLevel / 3;
            }
        }

        return totalBaseSave;
    }

    /// <summary>
    /// Builds the final ModifiableStat, combining the base class math with Ability Scores and Active Effects.
    /// </summary>
    public static ModifiableStat CalculateSave(
        string saveName,
        StatType statType,
        IEnumerable<HydratedClassLevel> classLevels,
        int abilityScore,
        string abilityName,
        IEnumerable<ActiveEffect> globalEffects)
    {
        EnsureValidSaveType(statType);

        // 1. Get the PF1e Base Save
        int baseSaveValue = CalculateBaseSave(classLevels, statType);

        // 2. Calculate the Ability Modifier
        int abilityModValue = AbilityScoreCalculator.CalculateModifier(abilityScore);

        var baseModifiers = new[]
        {
            new StatModifier(abilityName, abilityModValue, ModifierType.Ability, true)
        };

        // 3. Let the universal pipeline handle the final math and audit log
        return StatCalculator.Calculate(
            statName: saveName,
            targetStat: statType,
            baseValue: baseSaveValue,
            globalEffects: globalEffects,
            baseModifiers: baseModifiers
        );
    }

    /// <summary>
    /// Guard clause to ensure the provided StatType is a valid Saving Throw.
    /// </summary>
    private static void EnsureValidSaveType(StatType statType)
    {
        // C# 9+ Pattern Matching makes this incredibly readable
        if (statType is not (StatType.Fortitude or StatType.Reflex or StatType.Will))
        {
            throw new ArgumentException($"StatType '{statType}' is not a valid saving throw. Expected Fortitude, Reflex, or Will.", nameof(statType));
        }
    }
}
