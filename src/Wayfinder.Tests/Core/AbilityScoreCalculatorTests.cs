using NUnit.Framework;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DomainModels.Stats;
using Wayfinder.Core.Logic;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Rules.Calculators;

namespace Wayfinder.Tests.Core;

[TestFixture]
public class AbilityScoreCalculatorTests
{
    #region Helper Methods
    private HydratedClassLevel CreateLevelBump(int level, AbilityScore boostedScore)
    {
        return new HydratedClassLevel
        {
            ClassLevel = level,
            GrantsAbilityScoreIncrease = true,
            IncreasedAbilityScore = boostedScore
        };
    }

    private HydratedClassLevel CreateNormalLevel(int level)
    {
        return new HydratedClassLevel
        {
            ClassLevel = level,
            GrantsAbilityScoreIncrease = false
        };
    }
    #endregion

    [TestCase(10, 0)]
    [TestCase(11, 0)]
    [TestCase(12, 1)]
    [TestCase(13, 1)]
    [TestCase(20, 5)]
    [TestCase(9, -1)]
    [TestCase(8, -1)]
    [TestCase(1, -5)]
    [TestCase(0, -5)]
    public void CalculateModifier_FromInt_ReturnsCorrectModifier(int score, int expectedModifier)
    {
        // Act
        int result = AbilityScoreCalculator.CalculateModifier(score);

        // Assert
        Assert.That(result, Is.EqualTo(expectedModifier));
    }

    [Test]
    public void CalculateModifier_FromModifiableStat_ExtractsTotalAndCalculates()
    {
        // Arrange
        var stat = StatCalculator.Calculate(
                    statName: "Strength",
                    baseValue: 14,
                    globalEffects: Array.Empty<ActiveEffect>()
                );

        // A total of 14 should yield a +2

        // Act
        int result = AbilityScoreCalculator.CalculateModifier(stat);

        // Assert
        Assert.That(result, Is.EqualTo(2));
    }

    [Test]
    public void CalculateModifier_FromNullStat_SafelyDefaultsToZeroModifier()
    {
        // Arrange
        ModifiableStat? nullStat = null;

        // Act
        int result = AbilityScoreCalculator.CalculateModifier(nullStat);

        // Assert
        // A null stat defaults to a score of 10, which means the modifier is 0.
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void CalculateAbilityScore_BaseScoreOnly_ReturnsExactBase()
    {
        // Arrange
        int baseScore = 15;
        var levels = new List<HydratedClassLevel>();
        var effects = new List<ActiveEffect>();

        // Act
        var result = AbilityScoreCalculator.CalculateAbilityScore("Strength", baseScore, levels, effects);

        // Assert
        Assert.That(result.Total, Is.EqualTo(15));
        Assert.That(result.Modifiers.Count(m => m.Type == ModifierType.Base), Is.EqualTo(1));
    }

    [Test]
    public void CalculateAbilityScore_WithMatchingLevelBumps_AddsUntypedModifier()
    {
        // Arrange
        int baseScore = 15;
        var levels = new List<HydratedClassLevel>
    {
        CreateNormalLevel(1),
        CreateNormalLevel(2),
        CreateNormalLevel(3),
        CreateLevelBump(4, AbilityScore.Strength), // Match!
        CreateLevelBump(8, AbilityScore.Dexterity), // Ignored (wrong stat)
        CreateLevelBump(12, AbilityScore.Strength) // Match!
    };
        var effects = new List<ActiveEffect>();

        // Act
        var result = AbilityScoreCalculator.CalculateAbilityScore("Strength", baseScore, levels, effects);

        // Assert (15 Base + 2 Strength Bumps = 17)
        Assert.That(result.Total, Is.EqualTo(17));

        var levelBumpLog = result.Modifiers.FirstOrDefault(m => m.SourceName == "Level Advancement");
        Assert.That(levelBumpLog, Is.Not.Null, "Should log the level advancements.");
        Assert.That(levelBumpLog!.Value, Is.EqualTo(2));
        Assert.That(levelBumpLog.Type, Is.EqualTo(ModifierType.Untyped));
    }

    [Test]
    public void CalculateAbilityScore_IntegratesGlobalEffectsBus()
    {
        // Arrange
        int baseScore = 14;
        var levels = new List<HydratedClassLevel>
    {
        CreateLevelBump(4, AbilityScore.Dexterity) // 1 bump
    };

        var effects = new List<ActiveEffect>
    {
        // A matching effect
        new ActiveEffect { TargetStatName = "Dexterity", SourceName = "Belt of Incredible Dexterity", Value = 4, Type = ModifierType.Enhancement },
        
        // An ignored effect (wrong target)
        new ActiveEffect { TargetStatName = "Strength", SourceName = "Bull's Strength", Value = 4, Type = ModifierType.Enhancement }
    };

        // Act
        var result = AbilityScoreCalculator.CalculateAbilityScore("Dexterity", baseScore, levels, effects);

        // Assert (14 Base + 1 Bump + 4 Belt = 19)
        Assert.That(result.Total, Is.EqualTo(19));

        // Audit Log verification
        Assert.That(result.Modifiers.Any(m => m.SourceName == "Level Advancement" && m.Value == 1), Is.True);
        Assert.That(result.Modifiers.Any(m => m.SourceName == "Belt of Incredible Dexterity" && m.Value == 4), Is.True);
    }
}
