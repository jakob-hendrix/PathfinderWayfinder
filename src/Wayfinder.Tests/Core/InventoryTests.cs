using NUnit.Framework;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Rules.Engines;
using Wayfinder.Core.Services;

namespace Wayfinder.Tests.Core
{
    [TestFixture]
    public class InventoryTests
    {
        private CharacterEntity _character;
        private IEquipmentManager _equipmentManager;
        private IItemFactory _itemFactory;
        private IItemLibrary _itemLibrary;

        [SetUp]
        public void Setup()
        {

            _itemLibrary = new ItemLibrary();
            _itemLibrary.Register(new ItemDefinition { Id = "item1", Name = "Item 1", Weight = 1.0, ItemType = "AdventuringGear" });
            _itemLibrary.Register(new ItemDefinition { Id = "item2", Name = "Item 2", Weight = 2.0, ItemType = "AdventuringGear" });
            _itemLibrary.Register(new ItemDefinition { Id = "item3", Name = "Item 3", Weight = 3.0, ItemType = "AdventuringGear" });
            _itemLibrary.Register(new ItemDefinition { Id = "container1", Name = "container 1", Weight = 10.0, ItemType = "AdventuringGear" });

            _itemFactory = new ItemFactory(_itemLibrary);
            _equipmentManager = new EquipmentManager();
            _character = new CharacterEntity
            {
                BaseStrength = 10,
            };
        }

        [Test]
        public void GetTotalCarriedWeight_ShouldReturn0_WhenNoItems()
        {
            _character.Inventory.Clear();
            var totalWeight = _equipmentManager.GetTotalCarriedWeight(_character);
            Assert.That(totalWeight, Is.EqualTo(0));
        }

        [Test]
        public void GetTotalCarriedWeight_ShouldReturnCorrectWeight_WhenItemsPresent()
        {
            _character.Inventory.Clear();

            _character.Inventory.Add(_itemFactory.CreateItem("item1"));
            _character.Inventory.Add(_itemFactory.CreateItem("item1"));
            _character.Inventory.Add(_itemFactory.CreateItem("item1"));

            Assert.That(_equipmentManager.GetTotalCarriedWeight(_character), Is.EqualTo(3));
        }

        [Test]
        public void GetTotalCarriedWeight_ShouldNotIncludeDroppedItems()
        {
            _character.Inventory.Clear();

            var item1 = _itemFactory.CreateItem("item1");
            var item2 = _itemFactory.CreateItem("item1");

            _character.Inventory.Add(item1);
            _character.Inventory.Add(item2);

            Assert.That(_equipmentManager.GetTotalCarriedWeight(_character), Is.EqualTo(2));

            _character.Inventory[0].IsCarried = false;

            Assert.That(_equipmentManager.GetTotalCarriedWeight(_character), Is.EqualTo(1));
        }

        [Test]
        public void GetTotalCarriedWeight_ShouldIncludeItemsInContainers()
        {
            _character.Inventory.Clear();

            // weighs 10
            var container = _itemFactory.CreateItem("container1");
            container.IsCarried = true;

            // weighs 1
            var containedItem = _itemFactory.CreateItem("item1");
            containedItem.ContainerId = container.Id;

            _character.Inventory.Add(container);
            _character.Inventory.Add(containedItem);

            Assert.That(_equipmentManager.GetTotalCarriedWeight(_character), Is.EqualTo(11));
        }

        [Test]
        public void GetTotalCarriedWeight_ShouldExcludeItemsInContainersThatAreDropped()
        {
            // weighs 10
            var container = _itemFactory.CreateItem("container1");
            container.IsCarried = true;

            // weighs 1
            var containedItem = _itemFactory.CreateItem("item1");
            containedItem.ContainerId = container.Id;

            _character.Inventory.Add(container);
            _character.Inventory.Add(containedItem);

            _character.Inventory[0].IsCarried = false;

            Assert.That(_equipmentManager.GetTotalCarriedWeight(_character), Is.EqualTo(0));
        }
    }
}
