using NUnit.Framework;
using Wayfinder.Core.Enums;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Rules.Calculators;

namespace Wayfinder.Tests.Core
{
    [TestFixture]
    public class AbilityScoreCalculatorTests
    {
        [TestFixture]
        public class PathfinderMathExtensionsTests
        {
            [TestCase(0, -5)]
            [TestCase(1, -5)]
            [TestCase(2, -4)]
            [TestCase(3, -4)]
            [TestCase(4, -3)]
            [TestCase(5, -3)]
            [TestCase(6, -2)]
            [TestCase(7, -2)]
            [TestCase(8, -1)]
            [TestCase(9, -1)]
            [TestCase(10, 0)]
            [TestCase(11, 0)]
            [TestCase(12, 1)]
            [TestCase(13, 1)]
            [TestCase(14, 2)]
            [TestCase(15, 2)]
            [TestCase(16, 3)]
            [TestCase(17, 3)]
            [TestCase(18, 4)]
            [TestCase(19, 4)]
            [TestCase(20, 5)]
            [TestCase(33, 11)]
            public void CalculateModifier_ShouldReturnCorrectModifier(int score, int expected)
            {
                var result = AbilityScoreCalculator.CalculateModifier(score);
                Assert.That(result, Is.EqualTo(expected));
            }
        }

        [Test]
        public void CalculateCurrentValue_WithNullLevels_ReturnsBaseScore()
        {
            // Act
            int result = AbilityScoreCalculator.CalculateCurrentValue(15, AbilityScore.Strength, null);

            // Assert
            Assert.That(result, Is.EqualTo(15), "If levels is null, it should safely return the base score.");
        }

        [Test]
        public void CalculateCurrentValue_WithEmptyLevels_ReturnsBaseScore()
        {
            // Act
            int result = AbilityScoreCalculator.CalculateCurrentValue(12, AbilityScore.Dexterity, new List<HydratedClassLevel>());

            // Assert
            Assert.That(result, Is.EqualTo(12));
        }

        [Test]
        public void CalculateCurrentValue_WithMatchingAbilityBumps_AddsToBaseScore()
        {
            // Arrange
            var levels = new List<HydratedClassLevel>
        {
            // Two bumps specifically to Strength
            new HydratedClassLevel { GrantsAbilityScoreIncrease = true, IncreasedAbilityScore = AbilityScore.Strength },
            new HydratedClassLevel { GrantsAbilityScoreIncrease = true, IncreasedAbilityScore = AbilityScore.Strength }
        };

            // Act
            int result = AbilityScoreCalculator.CalculateCurrentValue(14, AbilityScore.Strength, levels);

            // Assert
            Assert.That(result, Is.EqualTo(16), "Should add +2 to the base score of 14.");
        }

        [Test]
        public void CalculateCurrentValue_WithDifferentAbilityBumps_IgnoresThem()
        {
            // Arrange
            var levels = new List<HydratedClassLevel>
        {
            // A bump to Dexterity shouldn't affect Strength
            new HydratedClassLevel { GrantsAbilityScoreIncrease = true, IncreasedAbilityScore = AbilityScore.Dexterity }
        };

            // Act
            int result = AbilityScoreCalculator.CalculateCurrentValue(14, AbilityScore.Strength, levels);

            // Assert
            Assert.That(result, Is.EqualTo(14), "Should ignore bumps assigned to other ability scores.");
        }

        [Test]
        public void CalculateCurrentValue_WithMixedLevels_CalculatesCorrectly()
        {
            // Arrange
            var levels = new List<HydratedClassLevel>
        {
            new HydratedClassLevel { GrantsAbilityScoreIncrease = false }, // e.g., Level 1 (no bump)
            new HydratedClassLevel { GrantsAbilityScoreIncrease = true, IncreasedAbilityScore = AbilityScore.Intelligence }, // Level 4
            new HydratedClassLevel { GrantsAbilityScoreIncrease = true, IncreasedAbilityScore = AbilityScore.Wisdom },      // Level 8
            new HydratedClassLevel { GrantsAbilityScoreIncrease = true, IncreasedAbilityScore = AbilityScore.Intelligence } // Level 12
        };

            // Act - Calculate Intelligence
            int intResult = AbilityScoreCalculator.CalculateCurrentValue(10, AbilityScore.Intelligence, levels);

            // Act - Calculate Wisdom
            int wisResult = AbilityScoreCalculator.CalculateCurrentValue(10, AbilityScore.Wisdom, levels);

            // Assert
            Assert.That(intResult, Is.EqualTo(12), "Should add exactly the 2 Intelligence bumps.");
            Assert.That(wisResult, Is.EqualTo(11), "Should add exactly the 1 Wisdom bump.");
        }
    }
}
