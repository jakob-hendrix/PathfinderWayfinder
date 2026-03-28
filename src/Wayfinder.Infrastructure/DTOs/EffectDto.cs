namespace Wayfinder.Infrastructure.DTOs;

public class EffectDto
{
    public string Target { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Type { get; set; } = "Untyped";
    public bool IsConditional { get; set; }
    public string? Condition { get; set; }
}
