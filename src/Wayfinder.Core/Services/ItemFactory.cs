using Wayfinder.Core.Data.Definitions;
using Wayfinder.Core.DomainModels.Items;
using Wayfinder.Core.Enums;

namespace Wayfinder.Core.Services
{
    public interface IItemFactory
    {
        ItemInstance CreateItem(string templateId);
    }

    public class ItemFactory : IItemFactory
    {
        private readonly IItemLibrary _itemLibrary;

        public ItemFactory(IItemLibrary itemLibrary) => _itemLibrary = itemLibrary;

        public ItemInstance CreateItem(string templateId)
        {
            var definition = _itemLibrary.GetItemDefinition(templateId);
            if (definition == null)
                throw new ArgumentException($"No item definition found for template ID: {templateId}");

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
                    MaxDexBonus = int.Parse(definition.Properties["MaxDexBonus"]),
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
