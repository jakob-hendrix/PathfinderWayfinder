using Wayfinder.Core.Constants;

namespace Wayfinder.Core.Models.Characters;

/// <summary>
/// The lightweight record of an item stored in a character's save file.
/// </summary>
public class ItemEntity
{
    public Guid Id { get; set; }

    // If this is a standard library item, this is all we need to look up the stats
    public string? TemplateId { get; set; }

    public string? CustomName { get; set; }
    public int Quantity { get; set; }

    // State
    public ItemState State { get; set; }
    public Guid? ContainerId { get; set; }
    public EquipmentSlot? EquippedSlot { get; set; }

    // TODO FOR LATER: If TemplateId is null, this is a fully custom item.
    // We will eventually need a serialized string or dictionary here to save the 
    // custom stats the user entered in the "Item Forge".
    // public string? CustomItemDataJson { get; set; } 
}
