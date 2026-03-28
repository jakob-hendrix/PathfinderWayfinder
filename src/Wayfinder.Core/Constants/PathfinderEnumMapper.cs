namespace Wayfinder.Core.Constants;

public static class PathfinderEnumMapper
{
    public static BabProgressionRate ToBabProgression(string value) => value.ToLower() switch
    {
        "fast" => BabProgressionRate.Fast,
        "medium" => BabProgressionRate.Medium,
        "slow" => BabProgressionRate.Slow,
        _ => throw new ArgumentException($"Invalid BAB progression rate: {value}")
    };

    public static SaveProgressionRate ToSaveProgression(string value) => value.ToLower() switch
    {
        "fast" => SaveProgressionRate.Fast,
        "slow" => SaveProgressionRate.Slow,
        _ => throw new ArgumentException($"Invalid save progression rate: {value}")
    };

    public static ArmorType ToArmorType(string value) => value.ToLower() switch
    {
        "light" => ArmorType.Light,
        "medium" => ArmorType.Medium,
        "heavy" => ArmorType.Heavy,
        _ => throw new ArgumentException($"Invalid armor type: {value}")
    };

    public static ItemType ToItemType(string value) => value.ToLower() switch
    {
        "weapon" => ItemType.Weapon,
        "armor" => ItemType.Armor,
        "shield" => ItemType.Shield,
        "wondrousitem" => ItemType.WondrousItem,
        "potion" => ItemType.Potion,
        "adventuringgear" => ItemType.AdventuringGear,
        _ => throw new ArgumentException($"Invalid item type: {value}")
    };

    public static AbilityScore ToAbilityScore(string value) => value.ToLower() switch
    {
        "strength" => AbilityScore.Strength,
        "dexterity" => AbilityScore.Dexterity,
        "constitution" => AbilityScore.Constitution,
        "intelligence" => AbilityScore.Intelligence,
        "wisdom" => AbilityScore.Wisdom,
        "charisma" => AbilityScore.Charisma,
        _ => throw new ArgumentException($"Invalid ability score: {value}")
    };

    public static ModifierType ToModifierType(string type) => type.ToLower() switch
    {
        "base" => ModifierType.Base,
        "racial" => ModifierType.Racial,
        "enhancement" => ModifierType.Enhancement,
        "trait" => ModifierType.Trait,
        "untyped" => ModifierType.Untyped,
        _ => throw new ArgumentException($"Invalid modifier type: {type}")
    };
}
