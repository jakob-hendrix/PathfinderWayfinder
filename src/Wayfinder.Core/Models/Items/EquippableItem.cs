using Wayfinder.Core.Constants;

namespace Wayfinder.Core.Models.Items;

public abstract class EquippableItem : BaseItem
{
    public override bool IsEquippable => true;
    public abstract EquipmentSlot Slot { get; }
}
