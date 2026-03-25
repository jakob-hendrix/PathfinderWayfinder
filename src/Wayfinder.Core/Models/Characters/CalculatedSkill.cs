using Wayfinder.Core.Constants;
using Wayfinder.Core.DomainModels.Stats;

namespace Wayfinder.Core.DomainModels.Skills;

public class CalculatedSkill
{
    public string Name { get; init; } = string.Empty;
    public AbilityScore KeyAbility { get; init; }
    public bool IsClassSkill { get; init; }
    public bool IsTrainedOnly { get; init; }
    public bool IsBackground { get; init; }
    public int TotalRanks { get; init; }

    public ModifiableStat Score { get; init; } = default!;

    public int TotalBonus => Score?.Total ?? 0;

    public int AbilityModifier => Score?.Modifiers
            .FirstOrDefault(m => m.Type == ModifierType.Ability && m.IsApplied)?.Value ?? 0;

    public int ClassSkillBonus => Score?.Modifiers
        .FirstOrDefault(m => m.SourceName == "Class Skill" && m.IsApplied)?.Value ?? 0;

    public int MiscBonus => TotalBonus - TotalRanks - AbilityModifier - ClassSkillBonus;

    // Helper for the UI to know if it should gray out an untrained skill
    public bool IsUsable => !IsTrainedOnly || TotalRanks > 0;
}
