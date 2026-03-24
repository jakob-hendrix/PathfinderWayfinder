namespace Wayfinder.Core.DomainModels.Stats;

using System.Collections.Generic;
using System.Linq;

public class CalculatedStat
{
    public string Name { get; init; } = string.Empty;
    public IReadOnlyList<StatModifier> Modifiers { get; init; } = Array.Empty<StatModifier>();

    // The Total is automatically derived from ONLY the modifiers that successfully stacked
    public int Total => Modifiers
        .Where(m => m.IsApplied)
        .Sum(m => m.Value);
}
