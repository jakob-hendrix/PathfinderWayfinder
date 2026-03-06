using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Models.Characters;

namespace Wayfinder.Core.Rules.Engines;

public static class FavoredClassBonusEngine
{
    /// <summary>
    /// Determines if a specific class selection qualifies as the character's Favored Class.
    /// </summary>
    public static bool IsFavoredClass(
        string proposedClassName,
        int proposedCharacterLevel,
        IEnumerable<HydratedClassLevel>? existingLevels)
    {
        if (string.IsNullOrWhiteSpace(proposedClassName)) return false;

        // Rule: At character level 1, the chosen class establishes the favored class.
        if (proposedCharacterLevel == 1) return true;

        if (existingLevels == null) return false;

        // Rule: For all subsequent levels, it must match the class chosen at level 1.
        var levelOneClass = existingLevels.FirstOrDefault(l => l.CharacterLevel == 1);

        return levelOneClass != null && levelOneClass.ClassDefinition.Name == proposedClassName;
    }

    /// <summary>
    /// Checks if a given class has an Alternate Racial Favored Class Bonus for the character's race.
    /// Returns the description if found, or null if none exists.
    /// </summary>
    public static string? GetAlternateRacialFcbDescription(ClassDefinition? classDef, string? characterRaceName)
    {
        if (classDef == null || string.IsNullOrWhiteSpace(characterRaceName))
            return null;

        var racialFcb = classDef.RacialFcbOptions
            .FirstOrDefault(r => r.RaceName == characterRaceName);

        return racialFcb?.Description;
    }
}
