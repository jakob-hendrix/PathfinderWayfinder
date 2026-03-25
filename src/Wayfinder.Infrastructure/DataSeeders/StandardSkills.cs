namespace Wayfinder.Core.Data;

using System.Collections.Generic;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DataDefinitions;

public static class StandardSkills
{
    public static List<SkillDefinition> GetCoreSkills()
    {
        return new List<SkillDefinition>
        {
            new() { Name = "Acrobatics", DefaultAbility = AbilityScore.Dexterity },
            new() { Name = "Appraise", DefaultAbility = AbilityScore.Intelligence },
            new() { Name = "Bluff", DefaultAbility = AbilityScore.Charisma },
            new() { Name = "Climb", DefaultAbility = AbilityScore.Strength },
            new() { Name = "Craft", DefaultAbility = AbilityScore.Intelligence },
            new() { Name = "Diplomacy", DefaultAbility = AbilityScore.Charisma },
            new() { Name = "Disable Device", DefaultAbility = AbilityScore.Dexterity, IsTrainedOnly = true },
            new() { Name = "Disguise", DefaultAbility = AbilityScore.Charisma },
            new() { Name = "Escape Artist", DefaultAbility = AbilityScore.Dexterity },
            new() { Name = "Fly", DefaultAbility = AbilityScore.Dexterity },
            new() { Name = "Handle Animal", DefaultAbility = AbilityScore.Charisma, IsTrainedOnly = true },
            new() { Name = "Heal", DefaultAbility = AbilityScore.Wisdom },
            new() { Name = "Intimidate", DefaultAbility = AbilityScore.Charisma },
            new() { Name = "Knowledge (Arcana)", DefaultAbility = AbilityScore.Intelligence, IsTrainedOnly = true },
            new() { Name = "Knowledge (Dungeoneering)", DefaultAbility = AbilityScore.Intelligence, IsTrainedOnly = true },
            new() { Name = "Knowledge (Engineering)", DefaultAbility = AbilityScore.Intelligence, IsTrainedOnly = true },
            new() { Name = "Knowledge (Geography)", DefaultAbility = AbilityScore.Intelligence, IsTrainedOnly = true },
            new() { Name = "Knowledge (History)", DefaultAbility = AbilityScore.Intelligence, IsTrainedOnly = true },
            new() { Name = "Knowledge (Local)", DefaultAbility = AbilityScore.Intelligence, IsTrainedOnly = true },
            new() { Name = "Knowledge (Nature)", DefaultAbility = AbilityScore.Intelligence, IsTrainedOnly = true },
            new() { Name = "Knowledge (Nobility)", DefaultAbility = AbilityScore.Intelligence, IsTrainedOnly = true },
            new() { Name = "Knowledge (Planes)", DefaultAbility = AbilityScore.Intelligence, IsTrainedOnly = true },
            new() { Name = "Knowledge (Religion)", DefaultAbility = AbilityScore.Intelligence, IsTrainedOnly = true },
            new() { Name = "Linguistics", DefaultAbility = AbilityScore.Intelligence, IsTrainedOnly = true },
            new() { Name = "Perception", DefaultAbility = AbilityScore.Wisdom },
            new() { Name = "Perform", DefaultAbility = AbilityScore.Charisma },
            new() { Name = "Profession", DefaultAbility = AbilityScore.Wisdom, IsTrainedOnly = true },
            new() { Name = "Ride", DefaultAbility = AbilityScore.Dexterity },
            new() { Name = "Sense Motive", DefaultAbility = AbilityScore.Wisdom },
            new() { Name = "Sleight of Hand", DefaultAbility = AbilityScore.Dexterity, IsTrainedOnly = true },
            new() { Name = "Spellcraft", DefaultAbility = AbilityScore.Intelligence, IsTrainedOnly = true },
            new() { Name = "Stealth", DefaultAbility = AbilityScore.Dexterity },
            new() { Name = "Survival", DefaultAbility = AbilityScore.Wisdom },
            new() { Name = "Swim", DefaultAbility = AbilityScore.Strength },
            new() { Name = "Use Magic Device", DefaultAbility = AbilityScore.Charisma, IsTrainedOnly = true }
        };
    }
}
