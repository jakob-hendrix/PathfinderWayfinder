using Wayfinder.Core.Constants;

namespace Wayfinder.Core.Models.Items;

public class ItemInstance
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // If null, this is a Custom Item created entirely by the user
    public string? TemplateId { get; set; }

    // User overrides (e.g., naming a sword "Orc-Cleaver")
    public string? CustomName { get; set; }

    // The core stats of the item (cloned from Library or custom built)
    public BaseItem BaseStats { get; set; } = default!;

    public int Quantity { get; set; } = 1;

    // --- LOCATION TRACKING ---
    public ItemState State { get; set; } = ItemState.Carried;

    // If Guid.Empty or null, it is loose in the character's general inventory
    public Guid? ContainerId { get; set; }

    // If State == Equipped, where is it?
    public EquipmentSlot? EquippedSlot { get; set; }

    // --- HELPERS ---
    public string DisplayName => !string.IsNullOrWhiteSpace(CustomName) ? CustomName : BaseStats.Name;
    public double TotalWeight => BaseStats.Weight * Quantity;
}
