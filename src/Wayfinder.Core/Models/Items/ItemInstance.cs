using Wayfinder.Core.Constants;
using Wayfinder.Core.Models.Characters;

namespace Wayfinder.Core.Models.Items;

public class ItemInstance
{
    // The underlying save data
    public ItemEntity Entity { get; }

    // The rich rulebook data (from the library)
    public BaseItem BaseStats { get; }

    public ItemInstance(ItemEntity entity, BaseItem baseStats)
    {
        Entity = entity;
        BaseStats = baseStats;
    }

    // --- PASS-THROUGH PROPERTIES (Live Sync to Save File) ---
    public Guid Id => Entity.Id;
    public string? TemplateId => Entity.TemplateId;

    public string? CustomName
    {
        get => Entity.CustomName;
        set => Entity.CustomName = value;
    }

    public int Quantity
    {
        get => Entity.Quantity;
        set => Entity.Quantity = value;
    }

    public ItemState State
    {
        get => Entity.State;
        set => Entity.State = value;
    }

    public Guid? ContainerId
    {
        get => Entity.ContainerId;
        set => Entity.ContainerId = value;
    }

    public EquipmentSlot? EquippedSlot
    {
        get => Entity.EquippedSlot;
        set => Entity.EquippedSlot = value;
    }

    // --- HELPERS ---
    public string DisplayName => !string.IsNullOrWhiteSpace(CustomName) ? CustomName : BaseStats.Name;
    public double TotalWeight => BaseStats.Weight * Quantity;
}
