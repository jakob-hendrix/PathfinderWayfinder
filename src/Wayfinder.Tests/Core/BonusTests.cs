using NUnit.Framework;
using Wayfinder.Core.Domain.Constants;
using Wayfinder.Core.Domain.Models;

namespace Wayfinder.Tests.Core
{
    [TestFixture]
    public class BonusTests
    {
        [TestCase(BonusType.Circumstance, true)]
        [TestCase(BonusType.Dodge, true)]
        [TestCase(BonusType.Untyped, true)]
        [TestCase(BonusType.Alchemical, false)]
        [TestCase(BonusType.Armor, false)]
        [TestCase(BonusType.BAB, false)]
        [TestCase(BonusType.Competence, false)]
        [TestCase(BonusType.Deflection, false)]
        [TestCase(BonusType.Enhancement, false)]
        [TestCase(BonusType.Inherent, false)]
        [TestCase(BonusType.Insight, false)]
        [TestCase(BonusType.Luck, false)]
        [TestCase(BonusType.Morale, false)]
        [TestCase(BonusType.NaturalArmor, false)]
        [TestCase(BonusType.Profane, false)]
        [TestCase(BonusType.Racial, false)]
        [TestCase(BonusType.Resistance, false)]
        [TestCase(BonusType.Sacred, false)]
        [TestCase(BonusType.Shield, false)]
        [TestCase(BonusType.Size, false)]
        [TestCase(BonusType.Trait, false)]
        public void BonusOfType_IsStackableOrNot(BonusType bonusType, bool isStackable)
        {
            var bonus = new Bonus
            {
                Type = bonusType
            };
            Assert.That(bonus.IsStackable, Is.EqualTo(isStackable));
        }
    }
}
