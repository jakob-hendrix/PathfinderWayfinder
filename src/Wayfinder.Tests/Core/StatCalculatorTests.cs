using NUnit.Framework;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DomainModels.Stats;
using Wayfinder.Core.Logic;


namespace Wayfinder.Tests.DomainModels.Characters;

[TestFixture]
public class StatCalculatorTests
{
    [Test]
    public void Calculate_WithOnlyBaseValue_ReturnsCorrectTotalAndLog()
    {
        // Act
        var result = StatCalculator.Calculate(StatNames.AC, 10, Enumerable.Empty<ActiveEffect>());

        // Assert
        Assert.That(result.Name, Is.EqualTo("Armor Class"));
        Assert.That(result.Total, Is.EqualTo(10));
        Assert.That(result.Modifiers, Has.Count.EqualTo(1));

        var baseMod = result.Modifiers.First();
        Assert.That(baseMod.SourceName, Is.EqualTo("Base"));
        Assert.That(baseMod.Value, Is.EqualTo(10));
        Assert.That(baseMod.IsApplied, Is.True);
    }

    [Test]
    public void Calculate_WithBaseModifiers_AppliesThemCorrectly()
    {
        // Arrange
        var baseMods = new List<StatModifier>
        {
            new StatModifier("Dexterity", 3, ModifierType.Ability, true)
        };

        // Act
        var result = StatCalculator.Calculate(StatNames.Reflex, 2, Enumerable.Empty<ActiveEffect>(), baseMods);

        // Assert
        Assert.That(result.Total, Is.EqualTo(5)); // 2 Base + 3 Dex
        Assert.That(result.Modifiers.Any(m => m.SourceName == "Dexterity" && m.IsApplied), Is.True);
    }

    [Test]
    public void Calculate_StackingTypes_AlwaysApplyAllBonuses()
    {
        // Arrange
        var globalEffects = new List<ActiveEffect>
        {
            new ActiveEffect { SourceName = "Dodge Feat", TargetStatName = StatNames.AC, Value = 1, Type = ModifierType.Dodge },
            new ActiveEffect { SourceName = "Haste", TargetStatName = StatNames.AC, Value = 1, Type = ModifierType.Dodge },
            new ActiveEffect { SourceName = "Untyped Buff", TargetStatName = StatNames.AC, Value = 2, Type = ModifierType.Untyped }
        };

        // Act
        var result = StatCalculator.Calculate(StatNames.AC, 10, globalEffects);

        // Assert
        // 10 Base + 1 Dodge + 1 Dodge + 2 Untyped = 14
        Assert.That(result.Total, Is.EqualTo(14));

        // Ensure all 4 modifiers (1 base + 3 effects) are marked as applied in the audit log
        Assert.That(result.Modifiers.Count(m => m.IsApplied), Is.EqualTo(4));
    }

    [Test]
    public void Calculate_NonStackingTypes_OnlyAppliesHighestBonus()
    {
        // Arrange
        var globalEffects = new List<ActiveEffect>
        {
            new ActiveEffect { SourceName = "Ring of Protection +1", TargetStatName = StatNames.AC, Value = 1, Type = ModifierType.Deflection },
            new ActiveEffect { SourceName = "Shield of Faith +3", TargetStatName = StatNames.AC, Value = 3, Type = ModifierType.Deflection }
        };

        // Act
        var result = StatCalculator.Calculate(StatNames.AC, 10, globalEffects);

        // Assert
        // 10 Base + 3 Deflection (highest applies) = 13. The +1 is ignored.
        Assert.That(result.Total, Is.EqualTo(13));

        // Audit Log Checks: Both should be in the list, but only the +3 is marked applied
        var ringMod = result.Modifiers.First(m => m.SourceName.Contains("Ring"));
        var spellMod = result.Modifiers.First(m => m.SourceName.Contains("Shield"));

        Assert.That(ringMod.IsApplied, Is.False, "The lower deflection bonus should be crossed out.");
        Assert.That(spellMod.IsApplied, Is.True, "The higher deflection bonus should be applied.");
    }

    [Test]
    public void Calculate_FiltersOutUnrelatedAndConditionalEffects()
    {
        // Arrange
        var globalEffects = new List<ActiveEffect>
        {
            new ActiveEffect { SourceName = "Amulet of Health", TargetStatName = StatNames.Constitution, Value = 2, Type = ModifierType.Enhancement },
            new ActiveEffect { SourceName = "Cloak of Resistance", TargetStatName = StatNames.Fortitude, Value = 1, Type = ModifierType.Enhancement },
            new ActiveEffect { SourceName = "Bravery", TargetStatName = StatNames.Fortitude, Value = 2, Type = ModifierType.Untyped, IsConditional = true } // Conditional!
        };

        // Act
        var result = StatCalculator.Calculate(StatNames.Fortitude, 4, globalEffects);

        // Assert
        // 4 Base + 1 Enhancement (Cloak). 
        // Constitution is ignored (wrong target). Bravery is ignored (conditional).
        Assert.That(result.Total, Is.EqualTo(5));

        Assert.That(result.Modifiers.Any(m => m.SourceName.Contains("Amulet")), Is.False, "Unrelated stats should not appear in the audit log.");
        Assert.That(result.Modifiers.Any(m => m.SourceName.Contains("Bravery")), Is.False, "Conditional stats should be filtered out of the main math.");
    }

    [Test]
    public void Calculate_NonStackingPenalties_OnlyAppliesWorstPenalty()
    {
        // Arrange: Two different sources applying an Enhancement penalty
        var globalEffects = new List<ActiveEffect>
        {
            new ActiveEffect { SourceName = "Minor Curse", TargetStatName = StatNames.Strength, Value = -2, Type = ModifierType.Enhancement },
            new ActiveEffect { SourceName = "Major Curse", TargetStatName = StatNames.Strength, Value = -4, Type = ModifierType.Enhancement }
        };

        // Act
        var result = StatCalculator.Calculate(StatNames.Strength, 14, globalEffects);

        // Assert
        // 14 Base - 4 (worst penalty applies, -2 is ignored) = 10
        Assert.That(result.Total, Is.EqualTo(10));

        var minorCurse = result.Modifiers.First(m => m.SourceName == "Minor Curse");
        var majorCurse = result.Modifiers.First(m => m.SourceName == "Major Curse");

        Assert.That(minorCurse.IsApplied, Is.False, "The lesser penalty should be crossed out.");
        Assert.That(majorCurse.IsApplied, Is.True, "The worst penalty should be applied.");
    }

    [Test]
    public void Calculate_UntypedEffectsFromSameSource_DoNotStack()
    {
        // Arrange: The character is somehow affected by the exact same spell twice
        var globalEffects = new List<ActiveEffect>
        {
            new ActiveEffect { SourceName = "Ray of Enfeeblement", TargetStatName = StatNames.Strength, Value = -3, Type = ModifierType.Untyped },
            new ActiveEffect { SourceName = "Ray of Enfeeblement", TargetStatName = StatNames.Strength, Value = -5, Type = ModifierType.Untyped }
        };

        // Act
        var result = StatCalculator.Calculate(StatNames.Strength, 14, globalEffects);

        // Assert
        // Because they have the exact same SourceName, the untyped penalties do NOT stack. Worst applies.
        Assert.That(result.Total, Is.EqualTo(9)); // 14 - 5

        Assert.That(result.Modifiers.Count(m => m.IsApplied && m.SourceName == "Ray of Enfeeblement"), Is.EqualTo(1));
    }

    [Test]
    public void Calculate_UntypedEffectsFromDifferentSources_DoStack()
    {
        // Arrange: Two different untyped penalties
        var globalEffects = new List<ActiveEffect>
        {
            new ActiveEffect { SourceName = "Fatigued", TargetStatName = StatNames.Strength, Value = -2, Type = ModifierType.Untyped },
            new ActiveEffect { SourceName = "Ray of Enfeeblement", TargetStatName = StatNames.Strength, Value = -5, Type = ModifierType.Untyped }
        };

        // Act
        var result = StatCalculator.Calculate(StatNames.Strength, 14, globalEffects);

        // Assert
        // Because they have DIFFERENT SourceNames, untyped penalties DO stack.
        Assert.That(result.Total, Is.EqualTo(7)); // 14 - 2 - 5

        Assert.That(result.Modifiers.First(m => m.SourceName == "Fatigued").IsApplied, Is.True);
        Assert.That(result.Modifiers.First(m => m.SourceName == "Ray of Enfeeblement").IsApplied, Is.True);
    }
}
