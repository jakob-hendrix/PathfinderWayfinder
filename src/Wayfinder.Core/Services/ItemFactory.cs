using Wayfinder.Core.Constants;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Items;

namespace Wayfinder.Core.Services
{
    public class ItemFactory : IItemFactory
    {
        private readonly IItemLibrary _itemLibrary;

        public ItemFactory(IItemLibrary itemLibrary) => _itemLibrary = itemLibrary;

        /// <summary>
        /// Creates a brand new item from the library to add to a character's inventory.
        /// </summary>
        public ItemInstance CreateItem(string templateId)
        {
            var definition = _itemLibrary.GetItemDefinition(templateId);
            if (definition == null)
                throw new KeyNotFoundException($"No item definition found for template ID: {templateId}");

            var templateStats = MapDefinitionToDomain(definition);

            // 1. Create the lightweight save entity
            var entity = new ItemEntity
            {
                Id = Guid.NewGuid(),
                TemplateId = templateId,
                Quantity = 1,
                State = ItemState.Carried,
                ContainerId = null,
                EquippedSlot = null
            };

            // 2. Wrap it in the rich domain instance
            return new ItemInstance(entity, templateStats);
        }

        /// <summary>
        /// Creates a brand new custom item entirely defined by the user.
        /// </summary>
        public ItemInstance CreateCustomItem(BaseItem customStats)
        {
            var entity = new ItemEntity
            {
                Id = Guid.NewGuid(),
                TemplateId = null, // Null template flags it as custom
                Quantity = 1,
                State = ItemState.Carried,
                ContainerId = null,
                EquippedSlot = null
            };

            return new ItemInstance(entity, customStats);
        }

        /// <summary>
        /// Rebuilds the rich ItemInstance from a saved ItemEntity when loading a character.
        /// </summary>
        public ItemInstance RehydrateItem(ItemEntity savedEntity)
        {
            BaseItem baseStats;

            if (!string.IsNullOrEmpty(savedEntity.TemplateId))
            {
                // Fetch the rulebook stats from the library using the saved ID
                var definition = _itemLibrary.GetItemDefinition(savedEntity.TemplateId);
                if (definition == null)
                    throw new KeyNotFoundException($"No item definition found for template ID: {savedEntity.TemplateId}");

                baseStats = MapDefinitionToDomain(definition);
            }
            else
            {
                // It's a custom item. 
                // TODO: When you implement custom item serialization, you'll deserialize the custom JSON here.
                // For now, we stub it out so it doesn't crash on load.
                baseStats = new AdventuringGearItem
                {
                    Name = savedEntity.CustomName ?? "Unknown Custom Item",
                    Weight = 0,
                    Cost = 0
                };
            }

            return new ItemInstance(savedEntity, baseStats);
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
