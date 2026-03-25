using NUnit.Framework;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DomainModels.Stats;

[TestFixture]
public class ClassFeatureTests
{
    [Test]
    public void BraveryBehavior_HasCorrectFeatureName()
    {
        // Arrange
        var behavior = new BraveryBehavior();

        // Assert
        // This is important to test because the ClassFeatureRegistry relies on this exact string!
        Assert.That(behavior.FeatureName, Is.EqualTo("Bravery"));
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(5)]
    public void BraveryBehavior_GenerateEffects_ReturnsCorrectMathematicalEffect(int rank)
    {
        // Arrange
        var behavior = new BraveryBehavior();

        // Act
        var results = behavior.GenerateEffects(rank).ToList();

        // Assert
        Assert.That(results, Has.Count.EqualTo(1), "Bravery should only generate exactly one effect.");

        var effect = results.First();

        Assert.That(effect.SourceName, Is.EqualTo($"Bravery (Rank {rank})"));
        Assert.That(effect.TargetStatName, Is.EqualTo(StatNames.Will));
        Assert.That(effect.Value, Is.EqualTo(rank), "Bravery bonus should perfectly match its rank.");
        Assert.That(effect.Type, Is.EqualTo(ModifierType.Untyped));
        Assert.That(effect.Category, Is.EqualTo(EffectCategory.ClassFeature));
        Assert.That(effect.IsConditional, Is.True);
        Assert.That(effect.ConditionDescription, Is.EqualTo("vs. Fear"));
    }
}
