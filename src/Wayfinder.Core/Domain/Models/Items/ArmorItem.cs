using Wayfinder.Core.Domain.Constants;

namespace Wayfinder.Core.Domain.Models.Items
{
    public class ArmorItem : EquippableItem
    {
        public override EquipmentSlot Slot => EquipmentSlot.Armor;
        public ArmorType ArmorType { get; set; }
        public int ArmorBonus { get; set; }
        // null represents no limit, otherwise this is the maximum dex bonus allowed by the armor
        public int? MaxDexBonus { get; set; }
        public int ArmorCheckPenalty { get; set; } = 0;
        // This is a percentage chance that a spell will fail. 25 represents a 25% chance
        public int ArcaneSpellFailureChance { get; set; }

        // Armor speed penalties are different based on the base speed of the character
        public int SpeedPenaltyForBase30 { get; set; } = 0;
        public int SpeedPenaltyForBase20 { get; set; } = 0;
    }
}
