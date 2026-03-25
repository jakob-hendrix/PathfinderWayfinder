using Wayfinder.Core.Constants;
using Wayfinder.Core.DataDefinitions;

namespace Wayfinder.Infrastructure.DataValidators
{
    /// <summary>
    /// Validation rules mapping the Item YAML definitions to the 
    /// </summary>
    public static class ItemSeedValidator
    {
        public static (bool IsValid, List<string> Errors) Validate(ItemDefinition definition)
        {
            var errors = new List<string>();

            ValidateBaseItem(definition, errors);
            if (errors.Count > 0)
                return (errors.Count == 0, errors);

            // Ensure our item type is handled
            try
            {
                PathfinderEnumMapper.ToItemType(definition.ItemType);
            }
            catch (ArgumentException ex)
            {
                errors.Add($"Invalid item type {ex.Message}");
            }

            if (errors.Count == 0)
            {
                var type = PathfinderEnumMapper.ToItemType(definition.ItemType);

                switch (type)
                {
                    case ItemType.AdventuringGear:
                        ValidateAdventuringGearItem(definition, errors);
                        break;
                    case ItemType.Armor:
                        ValidateArmorItem(definition, errors);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"No validator created for item type {type}");
                }
            }

            // TODO: validate that armor items have armor type


            return (errors.Count == 0, errors);
        }

        // Validate the basic facts of this item
        private static void ValidateBaseItem(ItemDefinition definition, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(definition.Name))
                errors.Add($"Item with ID '{definition.Id} is missing a Name");

            if (string.IsNullOrWhiteSpace(definition.Name))
                errors.Add($"Item with ID '{definition.Id} is missing a Name");

        }

        private static bool ValidateArmorItem(ItemDefinition definition, List<string> errors)
        {
            // TODO: add more validations
            if (!definition.Properties.ContainsKey("ACP")) errors.Add($"{definition.Name}: Missing ACP");
            return true;
        }

        private static bool ValidateAdventuringGearItem(ItemDefinition definition, List<string> errors)
        {
            // TODO: add more validations
            return true;
        }
    }
}
