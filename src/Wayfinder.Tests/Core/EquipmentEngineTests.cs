using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Wayfinder.Core.Constants;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Items;
using Wayfinder.Tests.Core.Fakes; // Assumes your TestRulesContext is here

namespace Wayfinder.Tests.Core.Engines;

[TestFixture]
public class EquipmentEngineTests
{
    private TestRulesContext _context;
    private IEquipmentEngine _engine;

    [SetUp]
    public void SetUp()
    {
        // Spin up the master DI container for this test run
        _context = new TestRulesContext();

        // Resolve the engine we are actually testing
        _engine = _context.Provider.GetRequiredService<IEquipmentEngine>();
    }

    // --- HELPER METHODS ---
    private ItemInstance CreateTestItem(ItemType type, string name, ItemState state = ItemState.Carried, EquipmentSlot? equippedSlot = null)
    {
        BaseItem baseStats = type switch
        {
            ItemType.Armor => new ArmorItem { Name = name, ArmorType = ArmorType.Light },
            ItemType.Weapon => new WeaponItem { Name = name, Category = WeaponCategory.OneHanded },
            ItemType.Shield => new ShieldItem { Name = name, ShieldType = ShieldType.Heavy },
            ItemType.AdventuringGear => new AdventuringGearItem { Name = name },
            _ => throw new NotImplementedException()
        };

        var entity = new ItemEntity
        {
            Id = Guid.NewGuid(),
            State = state,
            EquippedSlot = equippedSlot
        };

        return new ItemInstance(entity, baseStats);
    }

    // ==========================================
    // ISSLOTCOMPATIBLE TESTS
    // ==========================================

    [Test]
    public void IsSlotCompatible_ExactMatch_ReturnsTrue()
    {
        Assert.That(_engine.IsSlotCompatible(EquipmentSlot.Head, EquipmentSlot.Head), Is.True);
        Assert.That(_engine.IsSlotCompatible(EquipmentSlot.Chest, EquipmentSlot.Chest), Is.True);
    }

    [Test]
    public void IsSlotCompatible_NullItemSlot_ReturnsFalse()
    {
        // Items like Backpacks have a null BaseStats.Slot
        Assert.That(_engine.IsSlotCompatible(null, EquipmentSlot.Body), Is.False);
    }

    [Test]
    public void IsSlotCompatible_MismatchedSlots_ReturnsFalse()
    {
        Assert.That(_engine.IsSlotCompatible(EquipmentSlot.Head, EquipmentSlot.Feet), Is.False);
    }

    [Test]
    public void IsSlotCompatible_RingsAreInterchangeable_ReturnsTrue()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_engine.IsSlotCompatible(EquipmentSlot.Ring1, EquipmentSlot.Ring2), Is.True);
            Assert.That(_engine.IsSlotCompatible(EquipmentSlot.Ring2, EquipmentSlot.Ring1), Is.True);
        });
    }

    [Test]
    public void IsSlotCompatible_HandsAreInterchangeable_ReturnsTrue()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_engine.IsSlotCompatible(EquipmentSlot.MainHand, EquipmentSlot.OffHand), Is.True);
            Assert.That(_engine.IsSlotCompatible(EquipmentSlot.OffHand, EquipmentSlot.MainHand), Is.True);
        });
    }

    // ==========================================
    // GET EQUIPPABLE ITEMS TESTS
    // ==========================================

    [Test]
    public void GetEquippableItemsForSlot_FiltersCorrectly()
    {
        // Arrange
        var inventory = new List<ItemInstance>
        {
            CreateTestItem(ItemType.Armor, "Leather Armor", ItemState.Carried), // Valid
            CreateTestItem(ItemType.Armor, "Chain Shirt", ItemState.Equipped, EquipmentSlot.Armor), // Valid (already in this slot)
            CreateTestItem(ItemType.Armor, "Stored Plate", ItemState.Stored), // Invalid (Not carried)
            CreateTestItem(ItemType.Weapon, "Sword", ItemState.Carried), // Invalid (Wrong slot)
            CreateTestItem(ItemType.AdventuringGear, "Backpack", ItemState.Carried) // Invalid (No slot)
        };

        // Act
        var result = _engine.GetEquippableItemsForSlot(inventory, EquipmentSlot.Armor).ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.Any(i => i.BaseStats.Name == "Leather Armor"), Is.True);
            Assert.That(result.Any(i => i.BaseStats.Name == "Chain Shirt"), Is.True);
        });
    }

    // ==========================================
    // EQUIP / UNEQUIP TESTS
    // ==========================================

    [Test]
    public void EquipItem_EmptySlot_SetsStateAndSlot()
    {
        // Arrange
        var entity = new CharacterEntity();

        // SUCCESS: We use the fully wired-up engine from the DI container
        var sheet = new CharacterSheet(entity, _context.Engine);
        var newArmor = CreateTestItem(ItemType.Armor, "Breastplate", ItemState.Carried);

        sheet.LoadHydratedInventory(new[] { newArmor });

        // Act
        _engine.EquipItem(sheet, newArmor, EquipmentSlot.Armor);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(newArmor.State, Is.EqualTo(ItemState.Equipped));
            Assert.That(newArmor.EquippedSlot, Is.EqualTo(EquipmentSlot.Armor));
            Assert.That(newArmor.ContainerId, Is.Null);
        });
    }

    [Test]
    public void EquipItem_OccupiedSlot_ReplacesExistingItem()
    {
        // Arrange
        var entity = new CharacterEntity();
        var sheet = new CharacterSheet(entity, _context.Engine);

        var oldArmor = CreateTestItem(ItemType.Armor, "Leather Armor", ItemState.Equipped, EquipmentSlot.Armor);
        var newArmor = CreateTestItem(ItemType.Armor, "Breastplate", ItemState.Carried);

        sheet.LoadHydratedInventory(new[] { oldArmor, newArmor });

        // Act
        _engine.EquipItem(sheet, newArmor, EquipmentSlot.Armor);

        // Assert
        Assert.Multiple(() =>
        {
            // Old armor should be booted back to inventory
            Assert.That(oldArmor.State, Is.EqualTo(ItemState.Carried));
            Assert.That(oldArmor.EquippedSlot, Is.Null);

            // New armor should take its place
            Assert.That(newArmor.State, Is.EqualTo(ItemState.Equipped));
            Assert.That(newArmor.EquippedSlot, Is.EqualTo(EquipmentSlot.Armor));
        });
    }

    [Test]
    public void UnequipSlot_RemovesItemFromSlot()
    {
        // Arrange
        var entity = new CharacterEntity();
        var sheet = new CharacterSheet(entity, _context.Engine);
        var equippedArmor = CreateTestItem(ItemType.Armor, "Leather Armor", ItemState.Equipped, EquipmentSlot.Armor);

        sheet.LoadHydratedInventory(new[] { equippedArmor });

        // Act
        _engine.UnequipSlot(sheet, EquipmentSlot.Armor);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(equippedArmor.State, Is.EqualTo(ItemState.Carried));
            Assert.That(equippedArmor.EquippedSlot, Is.Null);
            Assert.That(_engine.GetItemInSlot(sheet.Inventory, EquipmentSlot.Armor), Is.Null);
        });
    }
}
