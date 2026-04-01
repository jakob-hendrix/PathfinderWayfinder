using Wayfinder.App.Services;
using Wayfinder.Core.Constants;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Items;

namespace Wayfinder.UI.ViewModels;

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

    // Placeholder: Will need to expand based on how you define 'Container' properties
    public IEnumerable<ItemInstance> Containers =>
        Sheet?.Inventory.Where(i => i.BaseStats.Type == ItemType.AdventuringGear &&
                                    i.BaseStats.Name.Contains("Backpack", StringComparison.OrdinalIgnoreCase))
        ?? Enumerable.Empty<ItemInstance>();

    private IEnumerable<ItemInstance> GetItemsByType(ItemType type)
    {
        return Sheet?.Inventory.Where(i => i.BaseStats.Type == type) ?? Enumerable.Empty<ItemInstance>();
    }

    // --- Actions ---

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
}
