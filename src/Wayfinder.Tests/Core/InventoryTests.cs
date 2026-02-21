using NUnit.Framework;
using Wayfinder.Core.Domain.Models.Characters;
using Wayfinder.Core.Domain.Models.Items;
using Wayfinder.Core.Rules.Services;

namespace Wayfinder.Tests.Core
{
    [TestFixture]
    public class InventoryTests
    {
        private CharacterEntity _character;
        private IEquipmentManager _equipmentManager;

        [SetUp]
        public void Setup()
        {
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

            var itemTemplate = new AdventuringGearItem
            {
                Name = "Generic Item",
                Weight = 1
            };

            _character.Inventory.Add(new ItemInstance { Template = itemTemplate });
            _character.Inventory.Add(new ItemInstance { Template = itemTemplate });
            _character.Inventory.Add(new ItemInstance { Template = itemTemplate });

            Assert.That(_equipmentManager.GetTotalCarriedWeight(_character), Is.EqualTo(3));
        }

        [Test]
        public void GetTotalCarriedWeight_ShouldNotIncludeDroppedItems()
        {
            _character.Inventory.Clear();
            var itemTemplate = new AdventuringGearItem
            {
                Name = "Generic Item",
                Weight = 1
            };
            _character.Inventory.Add(new ItemInstance { Template = itemTemplate, IsCarried = true });
            _character.Inventory.Add(new ItemInstance { Template = itemTemplate, IsCarried = false });

            Assert.That(_equipmentManager.GetTotalCarriedWeight(_character), Is.EqualTo(1));
        }

        [Test]
        public void GetTotalCarriedWeight_ShouldIncludeItemsInContainers()
        {
            _character.Inventory.Clear();
            var backpackTemplate = new AdventuringGearItem
            {
                Name = "Backpack",
                Weight = 2
            };
            var itemTemplate = new AdventuringGearItem
            {
                Name = "Generic Item",
                Weight = 1
            };

            var backpackInstance = new ItemInstance
            {
                Template = backpackTemplate,
                IsCarried = true
            };

            _character.Inventory.Add(backpackInstance);

            var containedItem1 = new ItemInstance { Template = itemTemplate, IsCarried = true, ContainerId = backpackInstance.Id };
            var containedItem2 = new ItemInstance { Template = itemTemplate, IsCarried = true, ContainerId = backpackInstance.Id };

            _character.Inventory.Add(containedItem1);
            _character.Inventory.Add(containedItem2);

            Assert.That(_equipmentManager.GetTotalCarriedWeight(_character), Is.EqualTo(4));
        }

        [Test]
        public void GetTotalCarriedWeight_ShouldExcludeItemsInContainersThatAreDropped()
        {
            _character.Inventory.Clear();
            var backpackTemplate = new AdventuringGearItem
            {
                Name = "Backpack",
                Weight = 2
            };
            var itemTemplate = new AdventuringGearItem
            {
                Name = "Generic Item",
                Weight = 1
            };

            var backpackInstance = new ItemInstance
            {
                Template = backpackTemplate,
                IsCarried = false
            };

            _character.Inventory.Add(backpackInstance);

            var containedItem1 = new ItemInstance { Template = itemTemplate, IsCarried = true, ContainerId = backpackInstance.Id };
            var containedItem2 = new ItemInstance { Template = itemTemplate, IsCarried = true, ContainerId = backpackInstance.Id };

            _character.Inventory.Add(containedItem1);
            _character.Inventory.Add(containedItem2);

            Assert.That(_equipmentManager.GetTotalCarriedWeight(_character), Is.EqualTo(0));
        }
    }
}
