using Wayfinder.Core.Constants;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Items;

namespace Wayfinder.Core.Interfaces;

public interface IEquipmentEngine
{
    EquipmentSlot[] StandardBodySlots { get; }
    EquipmentSlot[] ArmorSlots { get; }
    EquipmentSlot[] RingSlots { get; }
    EquipmentSlot[] WondrousSlots { get; }

    bool IsSlotCompatible(EquipmentSlot? itemSlot, EquipmentSlot targetSlot);

    IEnumerable<ItemInstance> GetEquippableItemsForSlot(IEnumerable<ItemInstance> inventory, EquipmentSlot targetSlot);
    IEnumerable<ItemInstance> GetItemsByType(IEnumerable<ItemInstance> inventory, ItemType type);

    ItemInstance? GetItemInSlot(IEnumerable<ItemInstance> inventory, EquipmentSlot slot);

    void EquipItem(CharacterSheet sheet, ItemInstance item, EquipmentSlot slot);
    void UnequipItem(ItemInstance item);
    void UnequipSlot(CharacterSheet sheet, EquipmentSlot slot);
}
