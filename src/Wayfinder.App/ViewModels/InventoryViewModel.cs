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
    private readonly IEquipmentEngine _equipmentEngine;

    // The active tab state (0 = Inventory, 1 = Gear)
    public int ActiveTab { get; set; } = 0;

    public InventoryViewModel(CharacterStateService stateService, IItemLibrary itemLibrary, IItemFactory itemFactory, IEquipmentEngine equipmentEngine)
    {
        _stateService = stateService;
        _itemLibrary = itemLibrary;
        _itemFactory = itemFactory;
        _equipmentEngine = equipmentEngine;
    }

    public bool HasActiveCharacter => _stateService.ActiveSheet != null;
    private CharacterSheet? Sheet => _stateService.ActiveSheet;

    // --- Core Inventory Access ---
    public IReadOnlyList<ItemInstance> AllItems => Sheet?.Inventory ?? new List<ItemInstance>();

    // Hides innate items (like fists) from standard inventory lists like "Backpack & Pockets"
    public IEnumerable<ItemInstance> Weapons => _equipmentEngine.GetItemsByType(AllItems, ItemType.Weapon).Where(i => !i.BaseStats.IsInnate);

    // Provides ALL weapons, including innate ones, to the Combat Loadout dropdowns
    public IEnumerable<ItemInstance> EquippableWeapons => _equipmentEngine.GetItemsByType(AllItems, ItemType.Weapon);

    public IEnumerable<ItemInstance> Armor => _equipmentEngine.GetItemsByType(AllItems, ItemType.Armor);
    public IEnumerable<ItemInstance> Shields => _equipmentEngine.GetItemsByType(AllItems, ItemType.Shield);
    public IEnumerable<ItemInstance> AdventuringGear => _equipmentEngine.GetItemsByType(AllItems, ItemType.AdventuringGear);

    public IReadOnlyList<AttackLoadout> Loadouts => Sheet?.Loadouts ?? Array.Empty<AttackLoadout>();

    public IEnumerable<ItemInstance> Containers =>
        AllItems.Where(i => i.BaseStats.Type == ItemType.AdventuringGear &&
                            i.BaseStats.Name.Contains("Backpack", StringComparison.OrdinalIgnoreCase));

    // --- UI Layout Helpers ---
    public EquipmentSlot[] StandardBodySlots => _equipmentEngine.StandardBodySlots;
    public EquipmentSlot[] ArmorSlots => _equipmentEngine.ArmorSlots;
    public EquipmentSlot[] RingSlots => _equipmentEngine.RingSlots;
    public EquipmentSlot[] WondrousSlots => _equipmentEngine.WondrousSlots;

    public ItemInstance? GetItemInSlot(EquipmentSlot slot) =>
        _equipmentEngine.GetItemInSlot(AllItems, slot);

    public IEnumerable<ItemInstance> GetEquippableItemsForSlot(EquipmentSlot targetSlot) =>
        _equipmentEngine.GetEquippableItemsForSlot(AllItems, targetSlot);

    // --- Actions ---

    public void EquipItem(ItemInstance item, EquipmentSlot slot)
    {
        if (Sheet == null) return;
        _equipmentEngine.EquipItem(Sheet, item, slot);
        _stateService.RefreshDomain();
    }

    public void UnequipSlot(EquipmentSlot slot)
    {
        if (Sheet == null) return;
        _equipmentEngine.UnequipSlot(Sheet, slot);
        _stateService.RefreshDomain();
    }

    public void ChangeItemState(ItemInstance item, ItemState newState)
    {
        if (Sheet == null) return;

        if (newState == ItemState.Equipped)
        {
            // If they just blindly select "Equipped" from a dropdown, try to auto-equip to its default slot
            if (item.BaseStats.Slot.HasValue)
                _equipmentEngine.EquipItem(Sheet, item, item.BaseStats.Slot.Value);
        }
        else
        {
            _equipmentEngine.UnequipItem(Sheet, item);
            item.State = newState; // Handle 'Dropped' or 'Stored' states
        }
        _stateService.RefreshDomain();
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

    public void RemoveItem(ItemInstance item)
    {
        if (Sheet == null) return;

        // The sheet manages removing it from both the rich list and the save entity
        Sheet.RemoveItem(item);

        // Recalculate encumbrance and update the UI everywhere
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
            .Where(d => !(d.Properties.TryGetValue("IsInnate", out var innateStr) && bool.TryParse(innateStr, out var isInnate) && isInnate))
            .OrderBy(d => d.Name);

        return _itemLibrary.GetAllDefinitions()
                           .Where(d => d.ItemType.Equals(typeString, StringComparison.OrdinalIgnoreCase))
                           .OrderBy(d => d.Name);
    }
}
