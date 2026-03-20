using Wayfinder.Core.Enums;

namespace Wayfinder.Core.DomainModels.Skills;

public class CalculatedSkill
{
    public string Name { get; init; } = string.Empty;
    public AbilityScore KeyAbility { get; init; }
    public bool IsClassSkill { get; init; }
    public bool IsTrainedOnly { get; init; }
    public bool IsBackground { get; init; }

    // The Math
    public int TotalRanks { get; init; }
    public int AbilityModifier { get; init; }

    public int ClassSkillBonus { get; init; }

    public int TotalBonus { get; init; }

    // Helper for the UI to know if it should gray out an untrained skill
    public bool IsUsable => !IsTrainedOnly || TotalRanks > 0;
}
