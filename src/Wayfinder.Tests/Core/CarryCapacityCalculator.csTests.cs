using NUnit.Framework;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DomainModels.Stats;
using Wayfinder.Core.Rules.Calculators;

namespace Wayfinder.Tests.Core.Rules.Calculators;

[TestFixture]
public class CarryCapacityCalculatorTests
{
    // --- CARRYING CAPACITY MATH ---

    [TestCase(10, SizeCategory.Medium, 2, 100)] // Standard Base
    [TestCase(18, SizeCategory.Medium, 2, 300)] // Standard High
    [TestCase(25, SizeCategory.Medium, 2, 800)] // Scaling Rule: Base 15 (200) * 4^1 = 800
    [TestCase(30, SizeCategory.Medium, 2, 1600)] // Scaling Rule: Base 10 (100) * 4^2 = 1600
    public void GetMaxCarryingCapacity_BaseStrength_CalculatesCorrectly(int str, SizeCategory size, int legs, int expectedMax)
    {
        var result = CarryCapacityCalculator.GetMaxCarryingCapacity(str, size, legs);
        Assert.That(result, Is.EqualTo(expectedMax));
    }

    [TestCase(10, SizeCategory.Large, 2, 200)]  // Large Biped = x2
    [TestCase(10, SizeCategory.Large, 4, 300)]  // Large Quadruped = x3
    [TestCase(10, SizeCategory.Tiny, 2, 50)]    // Tiny Biped = x0.5
    [TestCase(10, SizeCategory.Small, 4, 100)]  // Small Quadruped = x1 (same as Med Biped)
    public void GetMaxCarryingCapacity_WithSizeAndLegModifiers_CalculatesCorrectly(int str, SizeCategory size, int legs, int expectedMax)
    {
        var result = CarryCapacityCalculator.GetMaxCarryingCapacity(str, size, legs);
        Assert.That(result, Is.EqualTo(expectedMax));
    }

    // --- ENCUMBRANCE BANDS ---

    [TestCase(300, 100, EncumbranceLevel.Light)]    // Exactly 33.3%
    [TestCase(300, 101, EncumbranceLevel.Medium)]   // Over Light limit
    [TestCase(300, 200, EncumbranceLevel.Medium)]   // Exactly 66.6%
    [TestCase(300, 201, EncumbranceLevel.Heavy)]    // Over Medium limit
    [TestCase(300, 300, EncumbranceLevel.Heavy)]    // Exactly Max
    [TestCase(300, 301, EncumbranceLevel.Overloaded)] // Over Max
    public void GetEncumbranceLevel_ReturnsCorrectBand(int maxCapacity, double carriedWeight, EncumbranceLevel expected)
    {
        var result = CarryCapacityCalculator.GetEncumbranceLevel(maxCapacity, carriedWeight);
        Assert.That(result, Is.EqualTo(expected));
    }

    // --- EFFECT GENERATION ---

    [Test]
    public void GenerateEncumbranceEffects_MediumLoad_ReturnsCorrectPenalties()
    {
        var effects = CarryCapacityCalculator.GenerateEncumbranceEffects(EncumbranceLevel.Medium).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(effects, Has.Count.EqualTo(2));
            Assert.That(effects.Any(e => e.TargetStatName == StatNames.MaxDexBonus && e.Value == 3));
            Assert.That(effects.Any(e => e.TargetStatName == StatNames.ACP && e.Value == -3));
        });
    }

    [Test]
    public void GenerateEncumbranceEffects_HeavyLoad_ReturnsCorrectPenalties()
    {
        var effects = CarryCapacityCalculator.GenerateEncumbranceEffects(EncumbranceLevel.Heavy).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(effects, Has.Count.EqualTo(2));
            Assert.That(effects.Any(e => e.TargetStatName == StatNames.MaxDexBonus && e.Value == 1));
            Assert.That(effects.Any(e => e.TargetStatName == StatNames.ACP && e.Value == -6));
        });
    }
}
