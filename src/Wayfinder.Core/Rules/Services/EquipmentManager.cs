using Wayfinder.Core.DomainModels.Characters;
using Wayfinder.Core.DomainModels.Items;
using Wayfinder.Core.Enums;
using Wayfinder.Core.Interfaces;

namespace Wayfinder.Core.Rules.Services
{
    public class EquipmentManager : IEquipmentManager
    {
        public EncumbranceLevel GetEncumbrance(int totalCarriedWeight, int strength)
        {
            // TODO:
            // https://www.d20pfsrd.com/alignment-description/carrying-capacity/
            // There is a lookup table we need to implement here that returns light, med, heavy load
            // values based on the provided strength

            throw new NotImplementedException();
        }

        public double GetTotalCarriedWeight(CharacterEntity entity)
        {
            // 1. Filter for items being carried
            // 2. Sum from the template
            // 3. Check if a carried item is a container for other items

            double weight = 0;

            var topLevelCarried = entity.Inventory
                .Where(i => i.IsCarried && i.ContainerId == Guid.Empty);

            foreach (var item in topLevelCarried)
            {
                weight += CalculateWeightOfItemsContainedTherein(item, entity.Inventory);
            }

            return weight;
        }

        private double CalculateWeightOfItemsContainedTherein(ItemInstance baseItem, List<ItemInstance> inventory)
        {
            var weight = baseItem.BaseStats.Weight;

            // Recursively find items inside this item
            var heldItems = inventory.Where(i => i.ContainerId == baseItem.Id);
            foreach (var heldItem in heldItems)
            {
                weight += CalculateWeightOfItemsContainedTherein(heldItem, inventory);
            }

            return weight;
        }
    }
}
