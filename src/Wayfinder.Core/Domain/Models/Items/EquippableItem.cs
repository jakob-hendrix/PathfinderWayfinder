using Wayfinder.Core.Domain.Constants;

namespace Wayfinder.Core.Domain.Models.Items
{
    public abstract class EquippableItem : BaseItem
    {
        public override bool IsEquippable => true;
        public abstract EquipmentSlot Slot { get; }
    }
}
