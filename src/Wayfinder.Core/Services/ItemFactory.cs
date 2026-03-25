using Wayfinder.Core.Constants;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Items;

namespace Wayfinder.Core.Services
{
    public class ItemFactory : IItemFactory
    {
        private readonly IItemLibrary _itemLibrary;

        public ItemFactory(IItemLibrary itemLibrary) => _itemLibrary = itemLibrary;

        public ItemInstance CreateItem(string templateId)
        {
            var definition = _itemLibrary.GetItemDefinition(templateId);
            if (definition == null)
                throw new KeyNotFoundException($"No item definition found for template ID: {templateId}");

            var templateStats = MapDefinitionToDomain(definition);

            return new ItemInstance
            {
                Id = Guid.NewGuid(),
                TemplateId = templateId,
                IsCarried = true,
                BaseStats = templateStats
            };
        }

        private BaseItem MapDefinitionToDomain(ItemDefinition definition)
        {
            return definition.ItemType switch
            {
                // TODO refactor out some of the shared mapping
                "Armor" => new ArmorItem
                {
                    Name = definition.Name,
                    Weight = definition.Weight,
                    Cost = definition.Cost,
                    ArmorType = PathfinderEnumMapper.ToArmorType(definition.Properties["Category"]),
                    ArmorBonus = int.Parse(definition.Properties["ArmorBonus"]),
                    MaxDexBonus = int.Parse(definition.Properties["MaxDex"]),
                    ArmorCheckPenalty = int.Parse(definition.Properties["ACP"]),
                    ArcaneSpellFailureChance = int.Parse(definition.Properties["SpellFailure"]),
                    SpeedForBase30 = int.Parse(definition.Properties["Speed30"]),
                    SpeedForBase20 = int.Parse(definition.Properties["Speed20"])

                },
                "AdventuringGear" => new AdventuringGearItem
                {
                    Name = definition.Name,
                    Weight = definition.Weight,
                    Cost = definition.Cost
                },
                _ => throw new NotSupportedException($"Unsupported item type: {definition.ItemType}")
            };
        }
    }
}
