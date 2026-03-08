namespace Wayfinder.Core.Models.Characters;

public class GrantedFeatSlot
{
    public string Source { get; set; } = string.Empty;
    public int GrantedAtCharacterLevel { get; set; }
    // Null means unrestricted
    public string? Category { get; set; }
}
