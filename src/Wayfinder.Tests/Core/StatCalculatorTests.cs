using NUnit.Framework;
using Wayfinder.Core.Domain.Constants;
using Wayfinder.Core.Domain.Models;
using Wayfinder.Core.Rules.Services;

namespace Wayfinder.Tests.Core
{
    [TestFixture]
    public class StatCalculatorTests
    {
        private IStatCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = new StatCalculator();
        }

        [Test]
        public void CalculateStat_ShouldApplyHighestBonus_WhenUnstackableTypeIsTheSame()
        {
            var baseStat = 10;
            var bonuses = new List<Bonus>
            {
                new Bonus { Type = BonusType.Enhancement, Value = 2 },
                new Bonus { Type = BonusType.Enhancement, Value = 4 }
            };
            Assert.That(_calculator.CalculateStat(baseStat, bonuses), Is.EqualTo(14));
        }

        [Test]
        public void CalculateStat_ShouldApplyOnce_WhenUnstackableBonusesMatch()
        {
            var baseStat = 10;
            var bonuses = new List<Bonus>
            {
                new Bonus { Type = BonusType.Enhancement, Value = 5 },
                new Bonus { Type = BonusType.Enhancement, Value = 5 }
            };
            Assert.That(_calculator.CalculateStat(baseStat, bonuses), Is.EqualTo(15));
        }

        [Test]
        public void CalculateStat_ShouldSumBonuses_WhenStackableTypeIsTheSame()
        {
            var baseStat = 10;
            var bonuses = new List<Bonus>
            {
                new Bonus { Type = BonusType.Dodge, Value = 2 },
                new Bonus { Type = BonusType.Dodge, Value = 4 }
            };
            Assert.That(_calculator.CalculateStat(baseStat, bonuses), Is.EqualTo(16));
        }

        [Test]
        public void CalculateStat_ShouldSumBonuses_WhenStackableAndNonBonusesAreMixed()
        {
            var baseStat = 10;
            var bonuses = new List<Bonus>
            {
                new Bonus { Type = BonusType.Dodge, Value = 1 },
                new Bonus { Type = BonusType.Dodge, Value = 2 },
                new Bonus { Type = BonusType.Enhancement, Value = 1 },
                new Bonus { Type = BonusType.Enhancement, Value = 2 }
            };
            Assert.That(_calculator.CalculateStat(baseStat, bonuses), Is.EqualTo(15));
        }
    }
}
