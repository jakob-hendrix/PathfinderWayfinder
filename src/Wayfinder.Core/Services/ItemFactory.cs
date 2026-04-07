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
                "Weapon" => new WeaponItem
                {
                    Name = definition.Name,
                    Weight = definition.Weight,
                    Cost = definition.Cost,
                    Proficiency = Enum.Parse<WeaponProficiency>(definition.Properties.GetValueOrDefault("Proficiency", "Simple")),
                    Category = Enum.Parse<WeaponCategory>(definition.Properties.GetValueOrDefault("Category", "OneHanded")),
                    DamageSmall = definition.Properties.GetValueOrDefault("DamageSmall", "1d4"),
                    DamageMedium = definition.Properties.GetValueOrDefault("DamageMedium", "1d6"),
                    CritRange = definition.Properties.GetValueOrDefault("CritRange", "20"),
                    CritMultiplier = int.Parse(definition.Properties.GetValueOrDefault("CritMultiplier", "2")),
                    RangeIncrement = definition.Properties.ContainsKey("RangeIncrement") ? int.Parse(definition.Properties["RangeIncrement"]) : null,
                    DamageTypes = definition.Properties.GetValueOrDefault("DamageType", "").Split(',').Select(ParseDamageType).ToList(),
                    SpecialTraits = definition.Properties.GetValueOrDefault("Special", "").Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList(),
                    ConditionalEffect = definition.Properties.GetValueOrDefault("ConditionalEffect")
                },
                "Armor" => new ArmorItem
                {
                    Name = definition.Name,
                    Weight = definition.Weight,
                    Cost = definition.Cost,
                    ArmorType = PathfinderEnumMapper.ToArmorType(definition.Properties.GetValueOrDefault("Category", "Light")),
                    ArmorBonus = definition.Properties.TryGetValue("AC", out var abStr) && int.TryParse(abStr, out var ab) ? ab : 0,
                    MaxDexBonus = definition.Properties.TryGetValue("MaxDex", out var maxDexStr) && int.TryParse(maxDexStr, out var md) ? md : null,
                    ArmorCheckPenalty = definition.Properties.TryGetValue("ACP", out var acpStr) && int.TryParse(acpStr, out var acp) ? acp : 0,
                    ArcaneSpellFailureChance = definition.Properties.TryGetValue("SpellFailure", out var sfStr) && int.TryParse(sfStr, out var sf) ? sf : 0,
                    SpeedForBase30 = definition.Properties.TryGetValue("Speed30", out var spd30Str) && int.TryParse(spd30Str, out var spd30) ? spd30 : 0,
                    SpeedForBase20 = definition.Properties.TryGetValue("Speed20", out var spd20Str) && int.TryParse(spd20Str, out var spd20) ? spd20 : 0
                },
                "AdventuringGear" => new AdventuringGearItem
                {
                    Name = definition.Name,
                    Weight = definition.Weight,
                    Cost = definition.Cost
                },
                "Shield" => new ShieldItem
                {
                    Name = definition.Name,
                    Weight = definition.Weight,
                    Cost = definition.Cost,
                    ShieldType = PathfinderEnumMapper.ToShieldType(definition.Properties.GetValueOrDefault("Category", "Heavy")),
                    ShieldBonus = definition.Properties.TryGetValue("AC", out var sbStr) && int.TryParse(sbStr, out var sb) ? sb : 0,
                    MaxDexBonus = definition.Properties.TryGetValue("MaxDex", out var maxDexStr) && int.TryParse(maxDexStr, out var md) ? md : null,
                    ArmorCheckPenalty = definition.Properties.TryGetValue("ACP", out var acpStr) && int.TryParse(acpStr, out var acp) ? acp : 0,
                    ArcaneSpellFailureChance = definition.Properties.TryGetValue("SpellFailure", out var sfStr) && int.TryParse(sfStr, out var sf) ? sf : 0
                },
                _ => throw new NotSupportedException($"Unsupported item type: {definition.ItemType}")
            };
        }

        // Add this helper to ItemFactory.cs or PathfinderEnumMapper.cs
        private WeaponDamageType ParseDamageType(string typeString)
        {
            return typeString.ToUpper() switch
            {
                "B" => WeaponDamageType.Bludgeoning,
                "P" => WeaponDamageType.Piercing,
                "S" => WeaponDamageType.Slashing,
                _ => Enum.Parse<WeaponDamageType>(typeString, true)
            };
        }
    }
}
