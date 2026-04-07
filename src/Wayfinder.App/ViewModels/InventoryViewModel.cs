using Wayfinder.App.Services;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Items;

namespace Wayfinder.App.ViewModels;

public class InventoryViewModel
{
    private readonly CharacterStateService _stateService;
    private readonly IItemLibrary _itemLibrary;
    private readonly IItemFactory _itemFactory;

    // The active tab state (0 = Inventory, 1 = Gear)
    public int ActiveTab { get; set; } = 0;

    public InventoryViewModel(CharacterStateService stateService, IItemLibrary itemLibrary, IItemFactory itemFactory)
    {
        _stateService = stateService;
        _itemLibrary = itemLibrary;
        _itemFactory = itemFactory;
    }

    // --- UI State Helpers ---
    public bool HasActiveCharacter => _stateService.ActiveSheet != null;

    // Internal helper to avoid typing _stateService.ActiveSheet everywhere
    private CharacterSheet? Sheet => _stateService.ActiveSheet;

    // --- Core Inventory Access ---
    public IReadOnlyList<ItemInstance> AllItems => Sheet?.Inventory ?? new List<ItemInstance>();

    public IEnumerable<ItemInstance> Weapons => GetItemsByType(ItemType.Weapon);
    public IEnumerable<ItemInstance> Armor => GetItemsByType(ItemType.Armor);
    public IEnumerable<ItemInstance> AdventuringGear => GetItemsByType(ItemType.AdventuringGear);

    public IReadOnlyList<AttackLoadout> Loadouts => Sheet?.Loadouts ?? Array.Empty<AttackLoadout>();

    // Placeholder: Will need to expand based on how you define 'Container' properties
    public IEnumerable<ItemInstance> Containers =>
        Sheet?.Inventory.Where(i => i.BaseStats.Type == ItemType.AdventuringGear &&
                                    i.BaseStats.Name.Contains("Backpack", StringComparison.OrdinalIgnoreCase))
        ?? Enumerable.Empty<ItemInstance>();

    private IEnumerable<ItemInstance> GetItemsByType(ItemType type)
    {
        return Sheet?.Inventory.Where(i => i.BaseStats.Type == type) ?? Enumerable.Empty<ItemInstance>();
    }

    // A list of the standard static body slots (excluding hands, which are handled by Loadouts)
    public static readonly EquipmentSlot[] StandardBodySlots =
    {
        EquipmentSlot.Armor, EquipmentSlot.Head, EquipmentSlot.Headband, EquipmentSlot.Eyes,
        EquipmentSlot.Shoulders, EquipmentSlot.Neck, EquipmentSlot.Chest, EquipmentSlot.Body,
        EquipmentSlot.Belt, EquipmentSlot.Wrists, EquipmentSlot.Hands,
        EquipmentSlot.Ring2, EquipmentSlot.Ring1, EquipmentSlot.Feet
    };

    // Helper to get whatever is currently in a specific slot
    public ItemInstance? GetItemInSlot(EquipmentSlot slot)
    {
        return Sheet?.Inventory.FirstOrDefault(i => i.EquippedSlot == slot);
    }

    // Helper for the dropdowns: Gets items that COULD go in this slot
    // (In the future, you'll filter this by item type, e.g., only Armor in the Armor slot)
    public IEnumerable<ItemInstance> GetEquippableItemsForSlot(EquipmentSlot slot)
    {
        if (Sheet == null) return Enumerable.Empty<ItemInstance>();

        // Return items that are currently in this slot, OR items that are just carried
        return Sheet.Inventory.Where(i => i.EquippedSlot == slot || i.State == ItemState.Carried);
    }

    // --- Actions ---

    // --- Loadout Actions ---
    public void SetActiveLoadout(Guid loadoutId)
    {
        if (Sheet == null) return;
        foreach (var l in Sheet.Loadouts)
        {
            l.IsActive = (l.Id == loadoutId);
        }
        _stateService.RefreshDomain(); // Triggers AC and Attack Bonus recalculations!
    }

    public void AddNewLoadout()
    {
        Sheet?.AddLoadout(new AttackLoadout { Name = $"Loadout {(Sheet.Loadouts.Count + 1)}" });
        _stateService.RefreshDomain();
    }

    public void ChangeItemContainer(ItemInstance item, Guid? newContainerId)
    {
        if (Sheet == null) return;
        item.ContainerId = newContainerId;
        _stateService.RefreshDomain();
    }

    public void ChangeItemQuantity(ItemInstance item, int newQuantity)
    {
        if (Sheet == null) return;
        item.Quantity = Math.Max(1, newQuantity); // Don't allow 0 or negative
        _stateService.RefreshDomain();
    }

    public void AddItemFromLibrary(string templateId)
    {
        if (Sheet == null) return;

        // The factory should now build the ItemEntity AND wrap it in the ItemInstance
        var newItem = _itemFactory.CreateItem(templateId);

        // Use our new API method
        Sheet.AddItem(newItem);

        _stateService.RefreshDomain();
    }

    public void ChangeItemState(ItemInstance item, ItemState newState)
    {
        if (Sheet == null) return;

        item.State = newState;

        if (newState != ItemState.Equipped)
        {
            item.EquippedSlot = null;
        }

        _stateService.RefreshDomain();
    }

    public void RemoveItem(ItemInstance item)
    {
        if (Sheet == null) return;

        // The sheet manages removing it from both the rich list and the save entity
        Sheet.RemoveItem(item);

        // Recalculate encumbrance and update the UI everywhere
        _stateService.RefreshDomain();
    }

    public void EquipItem(ItemInstance item, EquipmentSlot slot)
    {
        if (Sheet == null) return;

        var existingItem = Sheet.Inventory.FirstOrDefault(i => i.EquippedSlot == slot);
        if (existingItem != null)
        {
            // Modifying this automatically modifies existingItem.Entity.State!
            existingItem.State = ItemState.Carried;
            existingItem.EquippedSlot = null;
        }

        item.State = ItemState.Equipped;
        item.EquippedSlot = slot;
        item.ContainerId = null;

        _stateService.RefreshDomain();
    }

    // Add this method to query the library for the picker
    public IEnumerable<ItemDefinition> GetLibraryTemplates(ItemType categoryType)
    {
        // Assuming your library has a way to get all definitions. 
        // We filter it by the string mapping of the enum.
        var typeString = categoryType.ToString();

        return _itemLibrary.GetAllDefinitions()
                           .Where(d => d.ItemType.Equals(typeString, StringComparison.OrdinalIgnoreCase))
                           .OrderBy(d => d.Name);
    }
}
