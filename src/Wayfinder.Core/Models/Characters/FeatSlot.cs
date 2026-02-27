namespace Wayfinder.Core.Models.Characters;

public class FeatSlot
{
    // This might end up being an id - we might need to add Guids to all factory output
    public string Source { get; set; } = string.Empty;
    // 0 indicates it was granted pre-levels (ie Human Racial Trait Bonus feat)
    public int GrantedAtCharacterLevel { get; set; }
    // null means unspent
    public string? SelectedFeatName { get; set; }
}
