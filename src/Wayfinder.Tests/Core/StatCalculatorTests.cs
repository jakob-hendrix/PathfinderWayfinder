using NUnit.Framework;
using Wayfinder.Core.Models.Common;
using Wayfinder.Core.Enums;
using Wayfinder.Core.Rules.Calculators;

namespace Wayfinder.Tests.Core
{
    [TestFixture]
    public class StatCalculatorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CalculateStat_ShouldApplyHighestBonus_WhenUnstackableTypeIsTheSame()
        {
            var baseStat = 10;
            var bonuses = new List<Bonus>
            {
                new Bonus { Type = ModifierType.Enhancement, Value = 2 },
                new Bonus { Type = ModifierType.Enhancement, Value = 4 }
            };
            Assert.That(StatCalculator.CalculateStat(baseStat, bonuses), Is.EqualTo(14));
        }

        [Test]
        public void CalculateStat_ShouldApplyOnce_WhenUnstackableBonusesMatch()
        {
            var baseStat = 10;
            var bonuses = new List<Bonus>
            {
                new Bonus { Type = ModifierType.Enhancement, Value = 5 },
                new Bonus { Type = ModifierType.Enhancement, Value = 5 }
            };
            Assert.That(StatCalculator.CalculateStat(baseStat, bonuses), Is.EqualTo(15));
        }

        [Test]
        public void CalculateStat_ShouldSumBonuses_WhenStackableTypeIsTheSame()
        {
            var baseStat = 10;
            var bonuses = new List<Bonus>
            {
                new Bonus { Type = ModifierType.Dodge, Value = 2 },
                new Bonus { Type = ModifierType.Dodge, Value = 4 }
            };
            Assert.That(StatCalculator.CalculateStat(baseStat, bonuses), Is.EqualTo(16));
        }

        [Test]
        public void CalculateStat_ShouldSumBonuses_WhenStackableAndNonBonusesAreMixed()
        {
            var baseStat = 10;
            var bonuses = new List<Bonus>
            {
                new Bonus { Type = ModifierType.Dodge, Value = 1 },
                new Bonus { Type = ModifierType.Dodge, Value = 2 },
                new Bonus { Type = ModifierType.Enhancement, Value = 1 },
                new Bonus { Type = ModifierType.Enhancement, Value = 2 }
            };
            Assert.That(StatCalculator.CalculateStat(baseStat, bonuses), Is.EqualTo(15));
        }
    }
}
