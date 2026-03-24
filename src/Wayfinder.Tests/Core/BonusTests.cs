using NUnit.Framework;
using Wayfinder.Core.Models.Common;
using Wayfinder.Core.Enums;

namespace Wayfinder.Tests.Core
{
    [TestFixture]
    public class BonusTests
    {
        [TestCase(ModifierType.Circumstance, true)]
        [TestCase(ModifierType.Dodge, true)]
        [TestCase(ModifierType.Untyped, true)]
        [TestCase(ModifierType.Alchemical, false)]
        [TestCase(ModifierType.Armor, false)]
        [TestCase(ModifierType.BAB, false)]
        [TestCase(ModifierType.Competence, false)]
        [TestCase(ModifierType.Deflection, false)]
        [TestCase(ModifierType.Enhancement, false)]
        [TestCase(ModifierType.Inherent, false)]
        [TestCase(ModifierType.Insight, false)]
        [TestCase(ModifierType.Luck, false)]
        [TestCase(ModifierType.Morale, false)]
        [TestCase(ModifierType.NaturalArmor, false)]
        [TestCase(ModifierType.Profane, false)]
        [TestCase(ModifierType.Racial, false)]
        [TestCase(ModifierType.Resistance, false)]
        [TestCase(ModifierType.Sacred, false)]
        [TestCase(ModifierType.Shield, false)]
        [TestCase(ModifierType.Size, false)]
        [TestCase(ModifierType.Trait, false)]
        public void BonusOfType_IsStackableOrNot(ModifierType bonusType, bool isStackable)
        {
            var bonus = new Bonus
            {
                Type = bonusType
            };
            Assert.That(bonus.IsStackable, Is.EqualTo(isStackable));
        }
    }
}
