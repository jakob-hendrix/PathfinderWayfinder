using NUnit.Framework;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.DomainModels.Stats;
using Wayfinder.Core.Logic;
using Wayfinder.Core.Models.Characters;

[TestFixture]
public class SaveCalculatorTests
{
    // --- Helper Methods for Test Data ---

    private ClassDefinition CreateClassDef(SaveProgressionRate fort, SaveProgressionRate reflex, SaveProgressionRate will)
    {
        return new ClassDefinition
        {
            FortitudeRate = fort,
            ReflexRate = reflex,
            WillRate = will
        };
    }

    private HydratedClassLevel CreateLevel(ClassDefinition def, int classLevel)
    {
        return new HydratedClassLevel
        {
            ClassDefinition = def,
            ClassLevel = classLevel
        };
    }

    // ==========================================
    // TESTS: CalculateBaseSave (PF1e Math)
    // ==========================================

    [Test]
    public void CalculateBaseSave_GuardClause_ThrowsOnInvalidStatNames()
    {
        var levels = new List<HydratedClassLevel>();

        var ex = Assert.Throws<ArgumentException>(() =>
            SaveCalculator.CalculateBaseSave(levels, StatNames.AC));

        Assert.That(ex.Message, Does.Contain("not a valid saving throw"));
    }

    [TestCase(1, 2)] // 2 + (1/2) = 2
    [TestCase(2, 3)] // 2 + (2/2) = 3
    [TestCase(3, 3)] // 2 + (3/2) = 3
    [TestCase(4, 4)] // 2 + (4/2) = 4
    public void CalculateBaseSave_FastProgression_CalculatesCorrectly(int classLevel, int expectedBaseSave)
    {
        // Arrange (e.g., Fighter Fortitude)
        var fighterDef = CreateClassDef(SaveProgressionRate.Fast, SaveProgressionRate.Slow, SaveProgressionRate.Slow);

        var levels = new List<HydratedClassLevel>();
        for (int i = 1; i <= classLevel; i++)
        {
            levels.Add(CreateLevel(fighterDef, i));
        }

        // Act
        int result = SaveCalculator.CalculateBaseSave(levels, StatNames.Fortitude);

        // Assert
        Assert.That(result, Is.EqualTo(expectedBaseSave));
    }

    [TestCase(1, 0)] // 1/3 = 0
    [TestCase(2, 0)] // 2/3 = 0
    [TestCase(3, 1)] // 3/3 = 1
    [TestCase(4, 1)] // 4/3 = 1
    [TestCase(6, 2)] // 6/3 = 2
    public void CalculateBaseSave_SlowProgression_CalculatesCorrectly(int classLevel, int expectedBaseSave)
    {
        // Arrange (e.g., Wizard Fortitude)
        var wizardDef = CreateClassDef(SaveProgressionRate.Slow, SaveProgressionRate.Slow, SaveProgressionRate.Fast);

        var levels = new List<HydratedClassLevel>();
        for (int i = 1; i <= classLevel; i++)
        {
            levels.Add(CreateLevel(wizardDef, i));
        }

        // Act
        int result = SaveCalculator.CalculateBaseSave(levels, StatNames.Fortitude);

        // Assert
        Assert.That(result, Is.EqualTo(expectedBaseSave));
    }

    [Test]
    public void CalculateBaseSave_MultiClassDip_CorrectlyStacksFastProgressions()
    {
        // Arrange: A classic PF1e "Dip" -> Fighter 1 / Barbarian 1
        var fighterDef = CreateClassDef(SaveProgressionRate.Fast, SaveProgressionRate.Slow, SaveProgressionRate.Slow);
        var barbDef = CreateClassDef(SaveProgressionRate.Fast, SaveProgressionRate.Slow, SaveProgressionRate.Slow);

        var levels = new List<HydratedClassLevel>
        {
            CreateLevel(fighterDef, 1),
            CreateLevel(barbDef, 1)
        };

        // Act
        int fortResult = SaveCalculator.CalculateBaseSave(levels, StatNames.Fortitude);
        int refResult = SaveCalculator.CalculateBaseSave(levels, StatNames.Reflex);

        // Assert
        // Fortitude: Fighter (+2) + Barbarian (+2) = +4
        Assert.That(fortResult, Is.EqualTo(4), "Multi-class fast progressions should grant the +2 multiple times.");

        // Reflex: Fighter (+0) + Barbarian (+0) = +0
        Assert.That(refResult, Is.EqualTo(0), "Multi-class slow progressions should not artificially inflate.");
    }

    [Test]
    public void CalculateSave_IntegratesBaseMathAbilityScoreAndEffects()
    {
        // Arrange
        var fighterDef = CreateClassDef(SaveProgressionRate.Fast, SaveProgressionRate.Slow, SaveProgressionRate.Slow);
        var levels = new List<HydratedClassLevel> { CreateLevel(fighterDef, 1) }; // Base Fort = +2

        // Constitution of 14 grants a +2 modifier
        int conScore = 14;

        var effects = new List<ActiveEffect>
        {
            new ActiveEffect { SourceName = "Cloak of Resistance +1", TargetStatName = StatNames.Fortitude, Value = 1, Type = ModifierType.Enhancement },
            new ActiveEffect { SourceName = "Minor Potion", TargetStatName = StatNames.Fortitude, Value = 1, Type = ModifierType.Alchemical }
        };

        // Act
        var result = SaveCalculator.CalculateSave(
            "Fortitude",
            levels,
            conScore,
            "Constitution",
            effects);

        // Assert
        // Math: 2 (Base) + 2 (Con Mod) + 1 (Enhancement) + 1 (Alchemical) = 6
        Assert.That(result.Name, Is.EqualTo("Fortitude"));
        Assert.That(result.Total, Is.EqualTo(6));

        // Audit Log Checks
        Assert.That(result.Modifiers, Has.Count.EqualTo(4), "Should have Base, Con Mod, Cloak, and Potion in the log.");

        var baseMod = result.Modifiers.First(m => m.SourceName == "Base");
        Assert.That(baseMod.Value, Is.EqualTo(2), "Base math was calculated incorrectly in the pipeline.");

        var abilityMod = result.Modifiers.First(m => m.SourceName == "Constitution");
        Assert.That(abilityMod.Value, Is.EqualTo(2), "Ability score modifier was injected incorrectly.");
        Assert.That(abilityMod.Type, Is.EqualTo(ModifierType.Ability));
    }
}
