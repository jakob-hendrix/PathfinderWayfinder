using Wayfinder.Core.Constants;

namespace Wayfinder.Core.Models.Items;

public class ShieldItem : BaseItem
{
    public override ItemType Type => ItemType.Shield;

    public ShieldType ShieldType { get; set; }

    // --- STATS ---
    public int ShieldBonus { get; set; }
    public int? MaxDexBonus { get; set; }
    public int ArmorCheckPenalty { get; set; } = 0;
    public int ArcaneSpellFailureChance { get; set; }
}
