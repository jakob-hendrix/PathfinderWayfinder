namespace Wayfinder.Core.Models.Characters;

public class RacialTrait
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<ActiveEffect> GrantedEffects { get; init; } = new();
}
