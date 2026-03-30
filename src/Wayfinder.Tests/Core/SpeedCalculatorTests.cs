using NUnit.Framework;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DomainModels.Stats;
using Wayfinder.Core.Rules.Calculators;

namespace Wayfinder.Tests.Core.Rules.Calculators;

[TestFixture]
public class SpeedCalculatorTests
{
    private List<ActiveEffect> GetBaseSpeedEffects(int baseSpeed)
    {
        return new List<ActiveEffect>
        {
            new() { TargetStatName = StatNames.LandSpeed, Value = baseSpeed, Type = ModifierType.Base, SourceName = "Base Race" }
        };
    }

    // --- SPEED REDUCTION MATH ---

    [TestCase(30, EncumbranceLevel.Light, 30)] // No penalty
    [TestCase(30, EncumbranceLevel.Medium, 20)] // 30 * 2/3 = 20
    [TestCase(20, EncumbranceLevel.Heavy, 15)] // 20 * 2/3 = 13.3 -> rounds UP to 15
    [TestCase(40, EncumbranceLevel.Medium, 30)] // 40 * 2/3 = 26.6 -> rounds UP to 30
    [TestCase(10, EncumbranceLevel.Heavy, 10)] // 10 * 2/3 = 6.6 -> rounds UP to 10
    [TestCase(30, EncumbranceLevel.Overloaded, 5)] // Overloaded caps at 5
    public void CalculateLandSpeed_WithEncumbrance_CalculatesCorrectFinalSpeed(int baseSpeed, EncumbranceLevel encumbrance, int expectedTotal)
    {
        var effects = GetBaseSpeedEffects(baseSpeed);
        var result = SpeedCalculator.CalculateLandSpeed(effects, encumbrance);

        Assert.That(result.Total, Is.EqualTo(expectedTotal));
    }

    [Test]
    public void CalculateLandSpeed_WithPenalty_AppendsModifierCorrectly()
    {
        var effects = GetBaseSpeedEffects(30); // Base 30

        // Act: Apply Heavy Encumbrance (should reduce by 10)
        var result = SpeedCalculator.CalculateLandSpeed(effects, EncumbranceLevel.Heavy);

        Assert.Multiple(() =>
        {
            Assert.That(result.Total, Is.EqualTo(20));
            Assert.That(result.Modifiers, Has.Count.EqualTo(2)); // 1 Base, 1 Penalty

            var penaltyMod = result.Modifiers.FirstOrDefault(m => m.Type == ModifierType.Penalty);
            Assert.That(penaltyMod, Is.Not.Null);
            Assert.That(penaltyMod!.Value, Is.EqualTo(-10));
            Assert.That(penaltyMod.SourceName, Is.EqualTo("Encumbrance"));
        });
    }

    [Test]
    public void CalculateLandSpeed_LightEncumbrance_DoesNotAddPenalty()
    {
        var effects = GetBaseSpeedEffects(30);
        var result = SpeedCalculator.CalculateLandSpeed(effects, EncumbranceLevel.Light);

        // Should only have the Base modifier, no penalty modifier added
        Assert.That(result.Modifiers, Has.Count.EqualTo(1));
        Assert.That(result.Total, Is.EqualTo(30));
    }

    // --- RUN MULTIPLIER ---

    [TestCase(EncumbranceLevel.Light, 4)]
    [TestCase(EncumbranceLevel.Medium, 4)]
    [TestCase(EncumbranceLevel.Heavy, 3)]
    [TestCase(EncumbranceLevel.Overloaded, 3)]
    public void CalculateRunMultiplier_ReturnsCorrectValue(EncumbranceLevel encumbrance, int expectedMultiplier)
    {
        var result = SpeedCalculator.CalculateRunMultiplier(encumbrance);
        Assert.That(result, Is.EqualTo(expectedMultiplier));
    }
}
