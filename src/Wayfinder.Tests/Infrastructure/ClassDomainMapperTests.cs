using NUnit.Framework;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Infrastructure.DTOs;
using Wayfinder.Infrastructure.Mappers;

namespace Wayfinder.Tests.Infrastructure;

[TestFixture]
public class ClassDomainMapperTests
{
    private ClassDomainMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new ClassDomainMapper();
    }

    // --- HELPER METHOD ---
    private ClassYamlDto CreateValidClassDto()
    {
        return new ClassYamlDto
        {
            Name = "Rogue",
            HitDie = 8,
            SkillPointsPerLevel = 8,
            BabRate = "medium",       // Assuming PathfinderEnumMapper handles case-insensitivity
            FortitudeRate = "slow",
            ReflexRate = "fast",
            WillRate = "slow",
            ClassSkills = new List<string> { "Acrobatics", "Stealth" },
            Levels = new Dictionary<int, LevelDefinition>
            {
                {
                    1, new LevelDefinition
                    {
                        ClassFeatures = new List<ClassFeatureDefinition>
                        {
                            new ClassFeatureDefinition { Id = "sneakattack", Name = "Sneak Attack", Rank = 1 }
                        }
                    }
                }
            },
            RacialFcbOptions = new List<RacialFavoredClassBonusDto> // Using your specific DTO name
            {
                new RacialFavoredClassBonusDto { RaceName = "Human", Description = "Add +1/6 of a new rogue talent." }
            }
        };
    }

    // ==========================================
    // SUCCESS PATHS
    // ==========================================

    [Test]
    public void Map_ValidDto_ReturnsFullyHydratedClass()
    {
        // Arrange
        var dto = CreateValidClassDto();

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.Warnings, Is.Empty);
            Assert.That(result.HydratedClass, Is.Not.Null);

            var hydrated = result.HydratedClass!;
            Assert.That(hydrated.Name, Is.EqualTo("Rogue"));
            Assert.That(hydrated.HitDie, Is.EqualTo(8));
            Assert.That(hydrated.ClassSkills, Contains.Item("Stealth"));

            // Verify Level & Feature Pointer mapping survived the passthrough
            Assert.That(hydrated.Levels.ContainsKey(1), Is.True);
            var feature = hydrated.Levels[1].ClassFeatures.First();
            Assert.That(feature.Name, Is.EqualTo("Sneak Attack"));
            Assert.That(feature.Rank, Is.EqualTo(1));

            // Verify FCB mapping
            Assert.That(hydrated.RacialFcbOptions, Has.Count.EqualTo(1));
            Assert.That(hydrated.RacialFcbOptions[0].RaceName, Is.EqualTo("Human"));
        });
    }

    // ==========================================
    // FATAL ERRORS (Fails the Result)
    // ==========================================

    [Test]
    public void Map_MissingClassName_ReturnsInvalidResult()
    {
        // Arrange
        var dto = CreateValidClassDto();
        dto.Name = ""; // Invalid!

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Count.EqualTo(1));
            Assert.That(result.Errors[0], Does.Contain("no Name"));
            Assert.That(result.HydratedClass, Is.Null);
        });
    }

    [Test]
    public void Map_InvalidProgressionRate_ReturnsInvalidResult()
    {
        // Arrange
        var dto = CreateValidClassDto();
        dto.BabRate = "SuperFast"; // This will cause PathfinderEnumMapper to throw

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Count.EqualTo(1));
            Assert.That(result.Errors[0], Does.Contain("invalid core progression"));
            Assert.That(result.HydratedClass, Is.Null);
        });
    }

    [Test]
    public void Map_LevelOutOfBounds_ReturnsInvalidResult()
    {
        // Arrange
        var dto = CreateValidClassDto();
        dto.Levels.Add(21, new LevelDefinition()); // Max level is 20

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Count.EqualTo(1));
            Assert.That(result.Errors[0], Does.Contain("outside the 1-20 range"));
            Assert.That(result.HydratedClass, Is.Null);
        });
    }

    // ==========================================
    // WARNINGS (Graceful Degradation)
    // ==========================================

    [Test]
    public void Map_FeatureMissingName_SkipsFeatureAndLogsWarning()
    {
        // Arrange
        var dto = CreateValidClassDto();

        // Add a nameless feature directly to the LevelDefinition object
        dto.Levels[1].ClassFeatures.Add(new ClassFeatureDefinition { Rank = 2, Name = "" });

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.True, "The class itself should survive.");
            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.Warnings, Has.Count.EqualTo(1));
            Assert.That(result.Warnings[0], Does.Contain("no name. Skipped."));

            var hydrated = result.HydratedClass!;

            // The valid "Sneak Attack" should survive, but the sanitizer should strip the nameless one
            Assert.That(hydrated.Levels[1].ClassFeatures, Has.Count.EqualTo(1));
            Assert.That(hydrated.Levels[1].ClassFeatures[0].Name, Is.EqualTo("Sneak Attack"));
        });
    }
}
