namespace Wayfinder.Core.Domain.Models
{
    // See: https://www.d20pfsrd.com/basics-ability-scores/glossary/
    public enum BonusType
    {
        Alchemical = 0,
        Armor = 1,
        BAB = 2,
        Circumstance = 3,   // stacks
        Competence = 4,
        Deflection = 5,
        Dodge = 6,          // stacks
        Enhancement = 7,
        Inherent = 8,
        Insight = 9,
        Luck = 10,
        Morale = 11,
        NaturalArmor = 12,
        Profane = 13,
        Racial = 14,
        Resistance = 15,
        Sacred = 16,
        Shield = 17,
        Size = 18,
        Trait = 19,
        Untyped = 99       // stacks
    }
}
