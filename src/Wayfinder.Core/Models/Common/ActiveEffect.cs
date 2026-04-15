using Wayfinder.Core.Constants;

public class ActiveEffect
{
    public Guid SourceId { get; init; }
    public required string SourceName { get; init; } = string.Empty; // e.g., "Ring of Protection +2"
    public EffectCategory Category { get; set; }
    public string TargetStatName { get; init; } = string.Empty;
    public int Value { get; init; }         // e.g., 2
    public string? StringValue { get; init; }
    public ModifierType Type { get; init; } = ModifierType.Untyped; // e.g., ModifierType.Deflection
    public bool IsConditional { get; init; } = false; // e.g., "Only when tracking"
    public string ConditionDescription { get; init; } = string.Empty;
}
