using NUnit.Framework;
using Wayfinder.Core.Constants;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Rules.Calculators;

namespace Wayfinder.Tests.Core;

[TestFixture]
public class HitPointCalculatorTests
{
    [Test]
    public void CalculateMaxHp_NoLevels_ReturnsZero()
    {
        Assert.That(HitPointCalculator.CalculateMaxHp(null, 10), Is.EqualTo(0));
        Assert.That(HitPointCalculator.CalculateMaxHp(new List<HydratedClassLevel>(), 10), Is.EqualTo(0));
    }

    [Test]
    public void CalculateMaxHp_WithPositiveCon_AddsModifierPerLevel()
    {
        var levels = new List<HydratedClassLevel>
        {
            new HydratedClassLevel { CharacterLevel = 1, HpGained = 10 },
            new HydratedClassLevel { CharacterLevel = 2, HpGained = 5 },
            new HydratedClassLevel { CharacterLevel = 3, HpGained = 5 }
        };

        var result = HitPointCalculator.CalculateMaxHp(levels, 14); // +2 Con modifier

        Assert.That(result, Is.EqualTo(26));
    }

    [Test]
    public void CalculateMaxHp_WithNegativeCon_SubtractsModifierPerLevel()
    {
        var levels = new List<HydratedClassLevel>
        {
            new HydratedClassLevel { CharacterLevel = 1, HpGained = 10 },
            new HydratedClassLevel { CharacterLevel = 2, HpGained = 5 },
            new HydratedClassLevel { CharacterLevel = 3, HpGained = 5 }
        };

        var result = HitPointCalculator.CalculateMaxHp(levels, 8); // -1 Con modifier

        Assert.That(result, Is.EqualTo(17)); // 9 + 4 + 4 = 17
    }

    [Test]
    public void CalculateMaxHp_WithNegativeCon_EnforcesMinOnePerLevel()
    {
        var levels = new List<HydratedClassLevel>
        {
            new HydratedClassLevel { CharacterLevel = 1, HpGained = 10 },
            new HydratedClassLevel { CharacterLevel = 2, HpGained = 1 }
        };

        var result = HitPointCalculator.CalculateMaxHp(levels, 2);

        Assert.That(result, Is.EqualTo(7)); //10 - 4 + 1 = 7
    }

    [Test]
    public void CalculateMaxHp_AppliesFCB()
    {
        var levels = new List<HydratedClassLevel>
        {
            new HydratedClassLevel {
                CharacterLevel = 1,
                HpGained = 10,
                AppliedFavoredClassBonus = FavoredClassBonus.HitPoint
            },
            new HydratedClassLevel {
                CharacterLevel = 2,
                HpGained = 10,
                AppliedFavoredClassBonus = FavoredClassBonus.SkillPoint
            }
        };

        var result = HitPointCalculator.CalculateMaxHp(levels, 10); // +0

        Assert.That(result, Is.EqualTo(21));    // 10+10+1
    }
}
