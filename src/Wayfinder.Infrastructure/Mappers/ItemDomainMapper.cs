using Wayfinder.Core.Constants;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Extensions;
using Wayfinder.Core.Models.Results;
using Wayfinder.Infrastructure.DTOs;

namespace Wayfinder.Infrastructure.Mappers;

public class ItemDomainMapper
{
    public ItemMapperResult Map(ItemYamlDto dto)
    {
        var result = new ItemMapperResult();

        // 1. Critical Base Validation
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            result.Errors.Add($"An item was found with no Name (ID: '{dto.Id}'). It has been skipped.");
            return result;
        }

        // 2. Validate Enum (Fatal if it fails, as the engine needs to know what this is)
        ItemType mappedType;
        try
        {
            mappedType = PathfinderEnumMapper.ToItemType(dto.ItemType);
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Item '{dto.Name}' has an invalid Type: {ex.Message}");
            return result;
        }

        // 3. Type-Specific Validation
        switch (mappedType)
        {
            case ItemType.Armor:
                ValidateArmorProperties(dto, result);
                break;
            case ItemType.Weapon:
                ValidateWeaponProperties(dto, result);
                break;
            case ItemType.AdventuringGear:
                break;
            default:
                result.Warnings.Add($"No specific property validator implemented for ItemType '{mappedType}' on item '{dto.Name}'.");
                break;
        }

        // If the type-specific validation logged any Errors, we abort.
        if (!result.IsValid)
        {
            return result;
        }

        // 4. Assemble the final Definition
        result.HydratedItem = new ItemDefinition
        {
            Id = !string.IsNullOrWhiteSpace(dto.Id) ? dto.Id : dto.Name.GenerateIdFromName(),
            Name = dto.Name,
            Weight = dto.Weight,
            Cost = dto.Cost,
            ItemType = dto.ItemType,
            Description = dto.Description ?? string.Empty,
            URL = dto.URL ?? string.Empty,
            Properties = dto.Properties ?? new Dictionary<string, string>()
        };

        return result;
    }

    // --- Type-Specific Validators ---

    private void ValidateArmorProperties(ItemYamlDto dto, ItemMapperResult result)
    {
        if (dto.Properties == null)
        {
            result.Errors.Add($"Armor '{dto.Name}' has no Properties defined (Missing ACP, MaxDex, etc).");
            return;
        }

        if (!dto.Properties.ContainsKey("ACP"))
        {
            result.Errors.Add($"Armor '{dto.Name}' is missing the 'ACP' property.");
        }
    }

    private void ValidateWeaponProperties(ItemYamlDto dto, ItemMapperResult result)
    {
        if (dto.Properties == null)
        {
            result.Errors.Add($"Weapon '{dto.Name}' has no Properties defined.");
            return;
        }

        // 1. Validate Category
        if (dto.Properties.TryGetValue("Category", out var categoryStr))
        {
            if (!Enum.TryParse<WeaponCategory>(categoryStr, true, out _))
                result.Errors.Add($"Weapon '{dto.Name}' has an invalid Category: '{categoryStr}'. Expected Light, OneHanded, TwoHanded, or Ranged.");
        }
        else
        {
            result.Errors.Add($"Weapon '{dto.Name}' is missing the 'Category' property.");
        }

        // 2. Validate Proficiency
        if (dto.Properties.TryGetValue("Proficiency", out var profStr))
        {
            if (!Enum.TryParse<WeaponProficiency>(profStr, true, out _))
                result.Errors.Add($"Weapon '{dto.Name}' has an invalid Proficiency: '{profStr}'. Expected Simple, Martial, or Exotic.");
        }

        // 3. Validate Damage Types (Allowing B, P, S abbreviations)
        if (dto.Properties.TryGetValue("DamageType", out var dmgTypeStr))
        {
            var types = dmgTypeStr.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach (var type in types)
            {
                var upperType = type.ToUpper();
                if (upperType != "B" && upperType != "P" && upperType != "S" && !Enum.TryParse<WeaponDamageType>(type, true, out _))
                {
                    result.Errors.Add($"Weapon '{dto.Name}' has an invalid DamageType: '{type}'. Expected B, P, S, Bludgeoning, Piercing, or Slashing.");
                }
            }
        }
    }
}
