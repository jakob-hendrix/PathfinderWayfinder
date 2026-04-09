using Wayfinder.Core.Constants;

namespace Wayfinder.Core.Models.Items;

public class ArmorItem : BaseItem
{
    public override ItemType Type => ItemType.Armor;
    public override EquipmentSlot? Slot => EquipmentSlot.Armor;

    public ArmorType ArmorType { get; set; }
    public int ArmorBonus { get; set; }
    public int? MaxDexBonus { get; set; }
    public int ArmorCheckPenalty { get; set; } = 0;
    public int ArcaneSpellFailureChance { get; set; }

    public int SpeedForBase30 { get; set; } = 0;
    public int SpeedForBase20 { get; set; } = 0;
}
