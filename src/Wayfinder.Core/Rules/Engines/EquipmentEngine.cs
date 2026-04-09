using Wayfinder.Core.Constants;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Items;

namespace Wayfinder.Core.Engines;

public class EquipmentEngine : IEquipmentEngine
{
    public EquipmentSlot[] StandardBodySlots { get; } =
    {
        EquipmentSlot.Armor, EquipmentSlot.Head, EquipmentSlot.Headband, EquipmentSlot.Eyes,
        EquipmentSlot.Shoulders, EquipmentSlot.Neck, EquipmentSlot.Chest, EquipmentSlot.Body,
        EquipmentSlot.Belt, EquipmentSlot.Wrists, EquipmentSlot.Hands,
        EquipmentSlot.Ring1, EquipmentSlot.Ring2, EquipmentSlot.Feet
    };

    public IEnumerable<ItemInstance> GetItemsByType(IEnumerable<ItemInstance> inventory, ItemType type)
    {
        return inventory.Where(i => i.BaseStats.Type == type);
    }

    public ItemInstance? GetItemInSlot(IEnumerable<ItemInstance> inventory, EquipmentSlot slot)
    {
        return inventory.FirstOrDefault(i => i.EquippedSlot == slot);
    }

    public IEnumerable<ItemInstance> GetEquippableItemsForSlot(IEnumerable<ItemInstance> inventory, EquipmentSlot targetSlot)
    {
        return inventory.Where(i =>
            (i.EquippedSlot == targetSlot || i.State == ItemState.Carried) &&
            IsSlotCompatible(i.BaseStats.Slot, targetSlot)
        );
    }

    public bool IsSlotCompatible(EquipmentSlot? itemSlot, EquipmentSlot targetSlot)
    {
        if (itemSlot == null) return false;
        if (itemSlot == targetSlot) return true;

        if ((itemSlot == EquipmentSlot.Ring1 || itemSlot == EquipmentSlot.Ring2) &&
            (targetSlot == EquipmentSlot.Ring1 || targetSlot == EquipmentSlot.Ring2)) return true;

        if ((itemSlot == EquipmentSlot.MainHand || itemSlot == EquipmentSlot.OffHand) &&
            (targetSlot == EquipmentSlot.MainHand || targetSlot == EquipmentSlot.OffHand)) return true;

        return false;
    }

    public void EquipItem(CharacterSheet sheet, ItemInstance item, EquipmentSlot slot)
    {
        // Unequip whatever is currently in that slot
        var existingItem = GetItemInSlot(sheet.Inventory, slot);
        if (existingItem != null)
        {
            UnequipItem(existingItem);
        }

        // Equip the new item
        item.State = ItemState.Equipped;
        item.EquippedSlot = slot;
        item.ContainerId = null; // Removing it from a backpack if it was in one
    }

    public void UnequipItem(ItemInstance item)
    {
        item.State = ItemState.Carried;
        item.EquippedSlot = null;
    }

    public void UnequipSlot(CharacterSheet sheet, EquipmentSlot slot)
    {
        var currentItem = GetItemInSlot(sheet.Inventory, slot);
        if (currentItem != null)
        {
            UnequipItem(currentItem);
        }
    }
}
