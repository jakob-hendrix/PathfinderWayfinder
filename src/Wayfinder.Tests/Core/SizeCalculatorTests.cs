using NUnit.Framework;
using Wayfinder.Core.Constants;
using Wayfinder.Core.Rules.Calculators;

namespace Wayfinder.Tests.Core.Rules.Calculators;

[TestFixture]
public class SizeCalculatorTests
{
    [Test]
    public void CalculateSize_NoEffects_DefaultsToMedium()
    {
        var effects = new List<ActiveEffect>();
        var result = SizeCalculator.CalculateSize(effects);
        Assert.That(result, Is.EqualTo(SizeCategory.Medium));
    }

    [Test]
    public void CalculateSize_WithBaseString_ParsesCorrectly()
    {
        var effects = new List<ActiveEffect>
        {
            new() { TargetStatName = "Size", StringValue = "Small", Type = ModifierType.Base, SourceName = "Halfling Race" }
        };

        var result = SizeCalculator.CalculateSize(effects);
        Assert.That(result, Is.EqualTo(SizeCategory.Small));
    }

    [Test]
    public void CalculateSize_WithSizeShifts_CalculatesCorrectFinalSize()
    {
        var effects = new List<ActiveEffect>
        {
            new() { TargetStatName = "Size", StringValue = "Medium", Type = ModifierType.Base, SourceName = "Human Race" },
            new() { TargetStatName = "Size", Value = 1, Type = ModifierType.Enhancement, SourceName = "Enlarge Person Spell" },
            new() { TargetStatName = "Size", Value = 1, Type = ModifierType.Untyped, SourceName = "Custom Magical Effect" }
        };

        var result = SizeCalculator.CalculateSize(effects);

        // Medium (4) + 1 + 1 = Huge (6)
        Assert.That(result, Is.EqualTo(SizeCategory.Huge));
    }

    [Test]
    public void CalculateSize_HasMaxSizeOfColossal()
    {
        var effects = new List<ActiveEffect>
        {
            new() { TargetStatName = "Size", StringValue = "Colossal", Type = ModifierType.Base, SourceName = "Dragon" },
            new() { TargetStatName = "Size", Value = 5, Type = ModifierType.Enhancement, SourceName = "Super Growth" } // Attempt to go beyond max
        };

        var result = SizeCalculator.CalculateSize(effects);
        Assert.That(result, Is.EqualTo(SizeCategory.Colossal));
    }

    [Test]
    public void CalculateSize_HasMinSizeOfFine()
    {
        var effects = new List<ActiveEffect>
        {
            new() { TargetStatName = "Size", StringValue = "Diminutive", Type = ModifierType.Base, SourceName = "Small Thing" },
            new() { TargetStatName = "Size", Value = -5, Type = ModifierType.Enhancement, SourceName = "Super Small Growth" }
        };

        var result = SizeCalculator.CalculateSize(effects);

        Assert.That(result, Is.EqualTo(SizeCategory.Fine));
    }
}
