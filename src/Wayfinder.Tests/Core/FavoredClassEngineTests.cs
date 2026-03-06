using NUnit.Framework;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Rules.Engines;

namespace Wayfinder.Tests.Logic;

[TestFixture]
public class FavoredClassBonusEngineTests
{
    [Test]
    public void IsFavoredClass_AtLevelOne_AlwaysReturnsTrue()
    {
        // Act
        bool result = FavoredClassBonusEngine.IsFavoredClass("Fighter", 1, null);

        // Assert
        Assert.That(result, Is.True, "Any class picked at level 1 is automatically the favored class.");
    }

    [Test]
    public void IsFavoredClass_MatchingLevelOneClass_ReturnsTrue()
    {
        // Arrange
        var history = new List<HydratedClassLevel>
        {
            new HydratedClassLevel { CharacterLevel = 1, ClassDefinition = new ClassDefinition { Name = "Wizard" } }
        };

        // Act - Taking a second level of Wizard at Character Level 2
        bool result = FavoredClassBonusEngine.IsFavoredClass("Wizard", 2, history);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsFavoredClass_DifferentFromLevelOneClass_ReturnsFalse()
    {
        // Arrange
        var history = new List<HydratedClassLevel>
        {
            new HydratedClassLevel { CharacterLevel = 1, ClassDefinition = new ClassDefinition { Name = "Wizard" } }
        };

        // Act - Taking a level of Fighter at Character Level 2
        bool result = FavoredClassBonusEngine.IsFavoredClass("Fighter", 2, history);

        // Assert
        Assert.That(result, Is.False, "Fighter does not match the Level 1 favored class (Wizard).");
    }

    [Test]
    public void IsFavoredClass_MissingHistoryForHigherLevels_ReturnsFalse()
    {
        // Act - Trying to evaluate level 2 without knowing what happened at level 1
        bool result = FavoredClassBonusEngine.IsFavoredClass("Rogue", 2, new List<HydratedClassLevel>());

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void GetAlternateRacialFcbDescription_WithMatchingRace_ReturnsDescription()
    {
        // Arrange
        var classDef = new ClassDefinition
        {
            Name = "Fighter",
            RacialFcbOptions = new List<RacialFavoredClassBonus>
            {
                new RacialFavoredClassBonus { RaceName = "Human", Description = "Add +1 to CMD." },
                new RacialFavoredClassBonus { RaceName = "Dwarf", Description = "Add +1 to CMD against bull rush." }
            }
        };

        // Act
        string? description = FavoredClassBonusEngine.GetAlternateRacialFcbDescription(classDef, "Human");

        // Assert
        Assert.That(description, Is.EqualTo("Add +1 to CMD."));
    }

    [Test]
    public void GetAlternateRacialFcbDescription_WithNoMatchingRace_ReturnsNull()
    {
        // Arrange
        var classDef = new ClassDefinition
        {
            Name = "Fighter",
            RacialFcbOptions = new List<RacialFavoredClassBonus>
            {
                new RacialFavoredClassBonus { RaceName = "Human", Description = "Add +1 to CMD." }
            }
        };

        // Act
        string? description = FavoredClassBonusEngine.GetAlternateRacialFcbDescription(classDef, "Elf");

        // Assert
        Assert.That(description, Is.Null, "Elves do not have a registered alternate FCB for this class.");
    }

    [Test]
    public void GetAlternateRacialFcbDescription_WithNullInputs_SafelyReturnsNull()
    {
        // Act & Assert
        Assert.That(FavoredClassBonusEngine.GetAlternateRacialFcbDescription(null, "Human"), Is.Null);
        Assert.That(FavoredClassBonusEngine.GetAlternateRacialFcbDescription(new ClassDefinition(), null), Is.Null);
    }
}
