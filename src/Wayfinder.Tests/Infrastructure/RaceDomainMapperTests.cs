using NUnit.Framework;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DomainModels.Stats;
using Wayfinder.Infrastructure.DTOs;
using Wayfinder.Infrastructure.Mappers;

namespace Wayfinder.Tests.Infrastructure;

[TestFixture]
public class RaceDomainMapperTests
{
    private RaceDomainMapper _mapper;
    private const string ExpectedWarning = "[System Warning: The mathematical effects for this trait failed to load. It is currently descriptive-only.]";

    [SetUp]
    public void SetUp()
    {
        _mapper = new RaceDomainMapper();
    }

    // --- HELPER METHOD ---
    private RaceYamlDto CreateBaseRaceDto()
    {
        return new RaceYamlDto
        {
            Id = "testrace",
            Name = "Test Race",
            DefaultRacialTraits = new List<RacialTraitYamlDto>() // Assuming this is your DTO name
        };
    }

    [Test]
    public void Map_WithValidEffects_MapsCorrectlyAndReturnsNoErrors()
    {
        // Arrange
        var dto = CreateBaseRaceDto();
        dto.DefaultRacialTraits.Add(new RacialTraitYamlDto
        {
            Name = "Healthy",
            Description = "A healthy trait.",
            GrantedEffects = new List<EffectDto>
            {
                new EffectDto { Target = StatNames.Constitution, Value = "2", Type = ModifierType.Racial.ToString() }
            }
        });

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Errors, Is.Empty);

            var mappedTrait = result.HydratedRace!.DefaultRacialTraits.First();
            Assert.That(mappedTrait.GrantedEffects, Has.Count.EqualTo(1));
            Assert.That(mappedTrait.GrantedEffects[0].Type, Is.EqualTo(ModifierType.Racial));
            Assert.That(mappedTrait.Description, Does.Not.Contain("System Warning"));
        });
    }

    [Test]
    public void Map_WithInvalidEffectType_DegradesGracefullyAndLogsError()
    {
        // Arrange
        var dto = CreateBaseRaceDto();
        dto.DefaultRacialTraits.Add(new RacialTraitYamlDto
        {
            Name = "Broken Trait",
            Description = "Base description.",
            GrantedEffects = new List<EffectDto>
            {
                new EffectDto { Target = StatNames.Strength, Value = "2", Type = "TypoType" }
            }
        });

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.True, "The race itself should still be considered valid.");
            Assert.That(result.Warnings, Has.Count.EqualTo(1), "Should log exactly one error for the bad mapping.");
            Assert.That(result.Warnings[0], Does.Contain("failed to map effect"));

            var mappedTrait = result.HydratedRace!.DefaultRacialTraits.First();
            Assert.That(mappedTrait.GrantedEffects, Is.Empty, "Effects should be completely stripped.");
            Assert.That(mappedTrait.Description, Does.Contain(ExpectedWarning), "Warning should be appended to description.");
        });
    }

    [Test]
    public void Map_WithMissingTarget_DegradesGracefullyAndLogsError()
    {
        // Arrange
        var dto = CreateBaseRaceDto();
        dto.DefaultRacialTraits.Add(new RacialTraitYamlDto
        {
            Name = "Nameless Target Trait",
            Description = "Base description.",
            GrantedEffects = new List<EffectDto>
            {
                new EffectDto { Target = "", Value = "2", Type = "Racial" }
            }
        });

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Warnings, Has.Count.EqualTo(1));
            Assert.That(result.Warnings[0], Does.Contain("no Target"));

            var mappedTrait = result.HydratedRace!.DefaultRacialTraits.First();
            Assert.That(mappedTrait.GrantedEffects, Is.Empty);
            Assert.That(mappedTrait.Description, Does.Contain(ExpectedWarning));
        });
    }

    [Test]
    public void Map_PartialFailureInTrait_StripsAllMathFromThatTrait()
    {
        // Arrange
        var dto = CreateBaseRaceDto();
        dto.DefaultRacialTraits.Add(new RacialTraitYamlDto
        {
            Name = "Half Broken Trait",
            Description = "Base description.",
            GrantedEffects = new List<EffectDto>
            {
                new EffectDto { Target = StatNames.Dexterity, Value = "2", Type = "Racial" }, // GOOD
                new EffectDto { Target = StatNames.Strength, Value = "2", Type = "BadType" }  // BAD
            }
        });

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            var mappedTrait = result.HydratedRace!.DefaultRacialTraits.First();

            // Even though the Dexterity effect was valid, the whole trait must be stripped 
            // to prevent the player from getting an unbalanced, partial trait.
            Assert.That(mappedTrait.GrantedEffects, Is.Empty);
            Assert.That(mappedTrait.Description, Does.Contain(ExpectedWarning));
            Assert.That(result.Warnings, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void Map_WithOneBrokenTraitAndOneGoodTrait_IsolatesTheFailure()
    {
        // Arrange
        var dto = CreateBaseRaceDto();
        dto.DefaultRacialTraits.Add(new RacialTraitYamlDto
        {
            Name = "Broken Trait",
            Description = "Bad.",
            GrantedEffects = new List<EffectDto> { new EffectDto { Target = "STR", Value = "2", Type = "Nope" } }
        });

        dto.DefaultRacialTraits.Add(new RacialTraitYamlDto
        {
            Name = "Good Trait",
            Description = "Good.",
            GrantedEffects = new List<EffectDto> { new EffectDto { Target = StatNames.Constitution, Value = "2", Type = "Racial" } }
        });

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            var brokenTrait = result.HydratedRace!.DefaultRacialTraits.First(t => t.Name == "Broken Trait");
            var goodTrait = result.HydratedRace!.DefaultRacialTraits.First(t => t.Name == "Good Trait");

            // The broken trait degrades
            Assert.That(brokenTrait.GrantedEffects, Is.Empty);
            Assert.That(brokenTrait.Description, Does.Contain(ExpectedWarning));

            // The good trait survives perfectly unharmed
            Assert.That(goodTrait.GrantedEffects, Has.Count.EqualTo(1));
            Assert.That(goodTrait.Description, Does.Not.Contain(ExpectedWarning));
        });
    }
}
