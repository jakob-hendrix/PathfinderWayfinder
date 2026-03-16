using NUnit.Framework;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Enums;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Results;
using Wayfinder.Core.Rules.Engines;

namespace Wayfinder.Tests.Logic;

[TestFixture]
public class SkillEngineTests
{
    private List<SkillDefinition> _availableSkills;

    [SetUp]
    public void Setup()
    {
        // Mock the combined library (defaults + custom)
        _availableSkills = new List<SkillDefinition>
        {
            new SkillDefinition { Name = "Acrobatics", DefaultAbility = AbilityScore.Dexterity, IsBackground = false },
            new SkillDefinition { Name = "Lore (Brewing)", DefaultAbility = AbilityScore.Intelligence, IsBackground = true },
            new SkillDefinition { Name = "Perform (Kazoo)", DefaultAbility = AbilityScore.Charisma, IsBackground = true } // Custom!
        };
    }

    [Test]
    public void ValidateSkillRanks_ValidPurchase_CalculatesSpentRanksAccurately()
    {
        var history = new List<SkillRankChoice>();
        var proposed = new List<SkillRankChoice>
        {
            new SkillRankChoice { SkillName = "Acrobatics", CharacterLevel = 1, Ranks = 1 },
            new SkillRankChoice { SkillName = "Perform (Kazoo)", CharacterLevel = 1, Ranks = 1 },
            new SkillRankChoice { SkillName = "Lore (Brewing)", CharacterLevel = 1, Ranks = 1 }
        };

        // Act
        SkillValidationResult result = SkillEngine.ValidateSkillRanksForLevel(1, proposed, history, _availableSkills);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.StandardRanksSpent, Is.EqualTo(1));
        Assert.That(result.BackgroundRanksSpent, Is.EqualTo(2));
    }

    [Test]
    public void ValidateSkillRanks_ExceedsRankCap_ReturnsError()
    {
        var history = new List<SkillRankChoice>
        {
            new SkillRankChoice { SkillName = "Acrobatics", CharacterLevel = 1, Ranks = 1 }
        };

        // Trying to add a second rank at level 1
        var proposed = new List<SkillRankChoice>
        {
            new SkillRankChoice { SkillName = "Acrobatics", CharacterLevel = 1, Ranks = 1 }
        };

        // Act
        SkillValidationResult result = SkillEngine.ValidateSkillRanksForLevel(1, proposed, history, _availableSkills);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors[0], Does.Contain("Max ranks cannot exceed character level"));
    }

    [Test]
    public void ValidateSkillRanks_ExceedsRankCapAcrossLevels_ReturnsError()
    {
        var history = new List<SkillRankChoice>
        {
            new SkillRankChoice { SkillName = "Acrobatics", CharacterLevel = 1, Ranks = 1 }
        };

        // Trying to add a 2 ranks second rank at level 2
        var proposed = new List<SkillRankChoice>
        {
            new SkillRankChoice { SkillName = "Acrobatics", CharacterLevel = 2, Ranks = 2 }
        };

        // Act
        SkillValidationResult result = SkillEngine.ValidateSkillRanksForLevel(2, proposed, history, _availableSkills);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors[0], Does.Contain("Max ranks cannot exceed character level"));
    }

    [Test]
    public void ValidateSkillRanks_WithUnknownSkill_ReturnsError()
    {
        var proposed = new List<SkillRankChoice>
        {
            new SkillRankChoice { SkillName = "MadeUpSkill", CharacterLevel = 1, Ranks = 1 }
        };

        // Act
        SkillValidationResult result = SkillEngine.ValidateSkillRanksForLevel(1, proposed, new List<SkillRankChoice>(), _availableSkills);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors[0], Does.Contain("not a recognized skill"));
    }

    [Test]
    public void ValidateSkillRanksForLevel_WithFutureHistory_IgnoresFutureLevels()
    {
        // Arrange: Validating Level 2.
        // History contains a rank bought at Level 3 (the "future").
        var history = new List<SkillRankChoice>
        {
            new SkillRankChoice { SkillName = "Acrobatics", CharacterLevel = 1, Ranks = 1 },
            new SkillRankChoice { SkillName = "Acrobatics", CharacterLevel = 3, Ranks = 1 } // Future!
        };

        var proposed = new List<SkillRankChoice>
        {
            // Adding 1 rank at level 2. 
            // If the engine counts the future level 3 rank, total would be 3 (which exceeds the level 2 cap) and fail.
            new SkillRankChoice { SkillName = "Acrobatics", CharacterLevel = 2, Ranks = 1 }
        };

        // Act
        SkillValidationResult result = SkillEngine.ValidateSkillRanksForLevel(2, proposed, history, _availableSkills);

        // Assert
        Assert.That(result.IsValid, Is.True, "Should ignore history from levels > targetLevel.");
    }

    [Test]
    public void ValidateSkillRanksForLevel_ProposedLevelMismatch_ReturnsError()
    {
        // Arrange: Validating Level 2, but accidentally passing a choice tagged as Level 3.
        var history = new List<SkillRankChoice>();
        var proposed = new List<SkillRankChoice>
        {
            new SkillRankChoice { SkillName = "Acrobatics", CharacterLevel = 3, Ranks = 1 }
        };

        // Act
        SkillValidationResult result = SkillEngine.ValidateSkillRanksForLevel(2, proposed, history, _availableSkills);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors[0], Does.Contain("tagged as Level 3, but we are validating Level 2"));
    }
}
