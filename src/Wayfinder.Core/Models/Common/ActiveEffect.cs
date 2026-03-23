using Wayfinder.Core.Enums;

public class ActiveEffect
{
    public required string SourceName { get; init; } = string.Empty; // e.g., "Ring of Protection +2"
    public EffectCategory Category { get; set; }
    public StatType TargetStat { get; init; }
    public int Value { get; init; }         // e.g., 2
    public BonusType Type { get; init; } = BonusType.Untyped; // e.g., ModifierType.Deflection
    public bool IsConditional { get; init; } = false; // e.g., "Only when tracking"
    public string? ConditionDescription { get; init; }
}
