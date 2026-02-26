using NUnit.Framework;
using Wayfinder.Core.Data.Definitions;
using Wayfinder.Core.DomainModels.Characters.RaceModels;
using Wayfinder.Infrastructure.DataValidators;

namespace Wayfinder.Tests.Infrastructure.Validation;

[TestFixture]
public class RaceSeedValidatorTests
{
    [Test]
    public void Validate_WithValidRace_ReturnsTrueAndNoErrors()
    {
        // Arrange
        var race = new RaceDefinition
        {
            Id = "human",
            Name = "Human",
            DefaultRacialTraits = new List<RacialTrait> { new RacialTrait { Name = "Bonus Feat" } }
        };

        // Act
        var (isValid, errors) = RaceSeedValidator.Validate(race);

        // Assert
        Assert.That(isValid, Is.True);
        Assert.That(errors, Is.Empty);
    }

    [Test]
    public void Validate_WithDuplicateTraits_ReturnsFalseAndError()
    {
        // Arrange
        var race = new RaceDefinition
        {
            Id = "elf",
            Name = "Elf",
            DefaultRacialTraits = new List<RacialTrait> { new RacialTrait { Name = "Low-Light Vision" } },
            AlternativeRacialTraits = new List<AlternativeRacialTrait>
        {
            new AlternativeRacialTrait
            {
                Name = "low-light vision", // Case-insensitive collision with default trait
                ReplacesRacialTraits = new List<string> { "Elven Immunities" }
            }
        }
        };

        // Act
        var (isValid, errors) = RaceSeedValidator.Validate(race);

        // Assert
        Assert.That(isValid, Is.False);
        Assert.That(errors, Has.Count.EqualTo(1));
        Assert.That(errors[0], Does.Contain("Duplicate alternative trait name found: 'low-light vision'"));
    }

    [Test]
    public void Validate_WithMissingReplacementData_ReturnsFalseAndError()
    {
        // Arrange
        var race = new RaceDefinition
        {
            Id = "dwarf",
            Name = "Dwarf",
            AlternativeRacialTraits = new List<AlternativeRacialTrait>
        {
            new AlternativeRacialTrait { Name = "Relentless" } // Missing what it replaces
        }
        };

        // Act
        var (isValid, errors) = RaceSeedValidator.Validate(race);

        // Assert
        Assert.That(isValid, Is.False);
        Assert.That(errors[0], Does.Contain("must specify at least one trait it replaces"));
    }
}
