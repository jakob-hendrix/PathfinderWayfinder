using NUnit.Framework;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Enums;
using Wayfinder.Core.Logic;
using Wayfinder.Core.Models.Characters;

namespace Wayfinder.Tests.Logic;

[TestFixture]
public class SkillPointCalculatorTests
{
    private ClassDefinition _rogueDef;
    private ClassDefinition _fighterDef;

    [SetUp]
    public void Setup()
    {
        _rogueDef = new ClassDefinition { Name = "Rogue", SkillPointsPerLevel = 8 };
        _fighterDef = new ClassDefinition { Name = "Fighter", SkillPointsPerLevel = 2 };
    }

    [Test]
    public void CalculateStandard_WithPositiveIntelligence_AddsModifier()
    {
        var level = new HydratedClassLevel { ClassDefinition = _rogueDef };

        // Int 14 (+2 mod). 8 + 2 = 10.
        int points = SkillPointCalculator.CalculateStandardSkillPoints(level, 14);

        Assert.That(points, Is.EqualTo(10));
    }

    [Test]
    public void CalculateStandard_WithNegativeIntelligence_SubtractsModifier()
    {
        var level = new HydratedClassLevel { ClassDefinition = _rogueDef };

        // Int 8 (-1 mod). 8 - 1 = 7.
        int points = SkillPointCalculator.CalculateStandardSkillPoints(level, 8);

        Assert.That(points, Is.EqualTo(7));
    }

    [Test]
    public void CalculateStandard_WithExtremeNegativeIntelligence_EnforcesMinimumOfOne()
    {
        var level = new HydratedClassLevel { ClassDefinition = _fighterDef };

        // Int 5 (-3 mod). 2 - 3 = -1. Should floor to 1.
        int points = SkillPointCalculator.CalculateStandardSkillPoints(level, 5);

        Assert.That(points, Is.EqualTo(1), "Should always grant at least 1 point before FCB.");
    }

    [Test]
    public void CalculateStandard_WithSkillPointFCB_AddsOneToTotal()
    {
        var level = new HydratedClassLevel
        {
            ClassDefinition = _fighterDef,
            AppliedFavoredClassBonus = FavoredClassBonus.SkillPoint
        };

        // Int 10 (+0 mod). Base 2 + 1 FCB = 3.
        int points = SkillPointCalculator.CalculateStandardSkillPoints(level, 10);

        Assert.That(points, Is.EqualTo(3));
    }

    [Test]
    public void CalculateStandard_ExtremeNegativeIntPlusFCB_AddsFCBAfterMinimum()
    {
        var level = new HydratedClassLevel
        {
            ClassDefinition = _fighterDef,
            AppliedFavoredClassBonus = FavoredClassBonus.SkillPoint
        };

        // Int 5 (-3 mod). Base 2 - 3 = -1 -> floored to 1. 
        // Then add 1 for FCB. Total = 2.
        int points = SkillPointCalculator.CalculateStandardSkillPoints(level, 5);

        Assert.That(points, Is.EqualTo(2), "FCB should be applied on top of the minimum 1 point.");
    }
}
