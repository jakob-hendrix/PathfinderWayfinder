using Wayfinder.Core.Constants;

namespace Wayfinder.Core.Models.Items;

public class WeaponItem : BaseItem
{
    public override ItemType Type => ItemType.Weapon;

    public WeaponProficiency Proficiency { get; set; }
    public WeaponCategory Category { get; set; }

    public string DamageSmall { get; set; } = string.Empty;
    public string DamageMedium { get; set; } = string.Empty;

    public string CritRange { get; set; } = "20";
    public int CritMultiplier { get; set; } = 2;

    // Null indicates a melee weapon with no range increment
    public int? RangeIncrement { get; set; }

    // e.g., ["B", "P", "S"]
    public List<WeaponDamageType> DamageTypes { get; set; } = new();

    // e.g., ["brace", "reach", "trip"]
    public List<string> SpecialTraits { get; set; } = new();

    // Storing conditional effects for the combat engine to parse later
    public string? ConditionalEffect { get; set; }
}
