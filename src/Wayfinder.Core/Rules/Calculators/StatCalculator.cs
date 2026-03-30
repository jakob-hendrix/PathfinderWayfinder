namespace Wayfinder.Core.Logic;

using System.Collections.Generic;
using System.Linq;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DomainModels.Stats;

public static class StatCalculator
{
    private static readonly ModifierType[] StackingTypes =
    {
        ModifierType.Untyped,
        ModifierType.Dodge,
        ModifierType.Circumstance
    };

    public static ModifiableStat Calculate(
        string statName,
        int baseValue,
        IEnumerable<ActiveEffect> globalEffects,
        IEnumerable<StatModifier>? baseModifiers = null)
    {
        var auditLog = new List<StatModifier>();

        // 1. Filter the bus FIRST so we can inspect what's coming in
        var relevantEffects = globalEffects
            .Where(e => e.TargetStatName.Equals(statName, StringComparison.OrdinalIgnoreCase) && !e.IsConditional)
            .ToList();

        // 2. Only apply the fallback base value if the effects bus didn't explicitly provide a Base modifier
        bool busHasBaseValue = relevantEffects.Any(e => e.Type == ModifierType.Base);

        if (!busHasBaseValue)
        {
            auditLog.Add(new StatModifier("Base", baseValue, ModifierType.Base, true));
        }

        if (baseModifiers != null)
        {
            auditLog.AddRange(baseModifiers);
        }

        var groupedEffects = relevantEffects.GroupBy(e => e.Type);

        foreach (var group in groupedEffects)
        {
            ModifierType type = group.Key;
            var typeEffects = group.ToList();

            if (StackingTypes.Contains(type))
            {
                // STACKING RULES: They stack, EXCEPT when from the exact same source.
                // We group by SourceName to enforce this.
                var effectsBySource = typeEffects.GroupBy(e => e.SourceName);

                foreach (var sourceGroup in effectsBySource)
                {
                    // Evaluate bonuses and penalties from this specific source independently
                    var bonuses = sourceGroup.Where(e => e.Value >= 0).ToList();
                    var penalties = sourceGroup.Where(e => e.Value < 0).ToList();

                    if (bonuses.Any())
                    {
                        var highestBonus = bonuses.OrderByDescending(e => e.Value).First();
                        foreach (var b in bonuses)
                            auditLog.Add(new StatModifier(b.SourceName, b.Value, type, b == highestBonus));
                    }

                    if (penalties.Any())
                    {
                        // OrderBy (ascending) puts -4 before -2, giving us the worst penalty
                        var worstPenalty = penalties.OrderBy(e => e.Value).First();
                        foreach (var p in penalties)
                            auditLog.Add(new StatModifier(p.SourceName, p.Value, type, p == worstPenalty));
                    }
                }
            }
            else
            {
                // NON-STACKING RULES: Take the single highest bonus and single worst penalty across all sources.
                var bonuses = typeEffects.Where(e => e.Value >= 0).ToList();
                var penalties = typeEffects.Where(e => e.Value < 0).ToList();

                ActiveEffect? highestBonus = bonuses.Any() ? bonuses.OrderByDescending(e => e.Value).First() : null;
                ActiveEffect? worstPenalty = penalties.Any() ? penalties.OrderBy(e => e.Value).First() : null;

                foreach (var effect in typeEffects)
                {
                    bool isApplied = false;

                    if (effect.Value >= 0 && effect == highestBonus) isApplied = true;
                    if (effect.Value < 0 && effect == worstPenalty) isApplied = true;

                    auditLog.Add(new StatModifier(effect.SourceName, effect.Value, type, isApplied));
                }
            }
        }

        return new ModifiableStat
        {
            Name = statName,
            Modifiers = auditLog
        };
    }
}
