using NUnit.Framework;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Tests.Core.Fakes;

namespace Wayfinder.Tests.DomainModels.Characters;

[TestFixture]
public class CharacterSheetTests
{
    private CharacterEntity _entity;
    private TestRulesContext _rulesEngine;
    private CharacterSheet _sheet;

    [SetUp]
    public void Setup()
    {
        // Setup the raw character
        _entity = new CharacterEntity();
        _entity.BaseStrength = 10;
        _entity.BaseDexterity = 10;
        _entity.BaseConstitution = 10;
        _entity.BaseIntelligence = 10;
        _entity.BaseWisdom = 10;
        _entity.BaseCharisma = 10;

        // Build real engine
        _rulesEngine = new TestRulesContext();

        // 3. Initialize the Sheet
        _sheet = new CharacterSheet(_entity, _rulesEngine.Engine);
    }

    // --- ADD / REMOVE CLASS LEVEL TESTS ---

    [Test]
    public void AddClassLevel_WithValidNextLevel_AddsToEntityAndTriggersRebuild()
    {
        // Arrange
        var choice = new ClassLevelChoice { CharacterLevel = 1, ClassName = "Fighter" };

        // Act
        _sheet.AddClassLevel(choice);

        // Assert
        Assert.That(_entity.ClassLevelChoices, Has.Count.EqualTo(1));
        Assert.That(_entity.ClassLevelChoices[0].ClassName, Is.EqualTo("Fighter"));
    }

    [Test]
    public void RemoveHighestClassLevel_WithExistingLevels_RemovesTopLevelAndRebuilds()
    {
        // Arrange
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 1, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 2, ClassName = "Wizard" });

        // Act
        _sheet.RemoveHighestClassLevel();

        // Assert
        Assert.That(_entity.ClassLevelChoices, Has.Count.EqualTo(1));
        Assert.That(_entity.ClassLevelChoices[0].ClassName, Is.EqualTo("Fighter"), "Should remove the last added level (Wizard).");
    }

    // --- ABILITY SCORE CALCULATION TESTS ---

    [Test]
    public void StrengthProperty_CalculatesBasePlusClassLevelBumps()
    {
        // Arrange: Base Score
        _entity.BaseStrength = 15;

        // Arrange: Class Level Bump (+1 Str at Level 4)
        // We add the fact to the entity so the sheet can read the user's choice
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 1, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 2, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 3, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice
        {
            CharacterLevel = 4,
            ClassName = "Fighter",
            AbilityScoreIncrease = AbilityScore.Strength
        });

        _sheet.RebuildClasses();

        Assert.That(_sheet.Strength, Is.EqualTo(16));
    }

    public void DexterityProperty_CalculatesBasePlusClassLevelBumps()
    {
        // Arrange: Base Score
        _entity.BaseDexterity = 15;

        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 1, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 2, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 3, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice
        {
            CharacterLevel = 4,
            ClassName = "Fighter",
            AbilityScoreIncrease = AbilityScore.Dexterity
        });

        // Act
        _sheet.RebuildClasses(); // Force the sheet to ingest the mocked level data

        Assert.That(_sheet.Dexterity, Is.EqualTo(16));
    }

    public void ConstitutionProperty_CalculatesBasePlusClassLevelBumps()
    {
        // Arrange: Base Score
        _entity.BaseConstitution = 15;

        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 1, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 2, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 3, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice
        {
            CharacterLevel = 4,
            ClassName = "Fighter",
            AbilityScoreIncrease = AbilityScore.Constitution
        });

        // Act
        Assert.That(_sheet.Constitution, Is.EqualTo(16));
    }

    public void IntelligenceProperty_CalculatesBasePlusClassLevelBumps()
    {
        // Arrange: Base Score
        _entity.BaseIntelligence = 15;

        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 1, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 2, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 3, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice
        {
            CharacterLevel = 4,
            ClassName = "Fighter",
            AbilityScoreIncrease = AbilityScore.Intelligence
        });

        // Act
        _sheet.RebuildClasses(); // Force the sheet to ingest the mocked level data

        int totalStat = _sheet.Intelligence;
        Assert.That(totalStat, Is.EqualTo(16));
    }

    public void WisdomProperty_CalculatesBasePlusClassLevelBumps()
    {
        // Arrange: Base Score
        _entity.BaseWisdom = 15;

        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 1, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 2, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 3, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice
        {
            CharacterLevel = 4,
            ClassName = "Fighter",
            AbilityScoreIncrease = AbilityScore.Wisdom
        });

        // Act
        _sheet.RebuildClasses(); // Force the sheet to ingest the mocked level data

        int totalStat = _sheet.Wisdom;
        Assert.That(totalStat, Is.EqualTo(16));
    }

    public void CharismaProperty_CalculatesBasePlusClassLevelBumps()
    {
        // Arrange: Base Score
        _entity.BaseCharisma = 15;

        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 1, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 2, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 3, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice
        {
            CharacterLevel = 4,
            ClassName = "Fighter",
            AbilityScoreIncrease = AbilityScore.Charisma
        });

        // Act
        _sheet.RebuildClasses(); // Force the sheet to ingest the mocked level data

        int totalStat = _sheet.Charisma;
        Assert.That(totalStat, Is.EqualTo(16));
    }

    // --- CLASS SUMMARY TESTS ---

    [Test]
    public void ClassSummary_WithMultipleClasses_FormatsCorrectly()
    {
        // Arrange: Seed the specific classes we need into the fake library
        _rulesEngine.ClassLibrary.Register(new ClassDefinition { Name = "Wizard" });
        _rulesEngine.ClassLibrary.Register(new ClassDefinition { Name = "Fighter" });

        // Arrange: Add the facts (choices) to the entity in chronological order
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 1, ClassName = "Wizard" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 2, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 3, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 4, ClassName = "Wizard" });

        // Act: Tell the sheet to process the facts into reality
        _sheet.RebuildClasses();
        var summary = _sheet.ClassSummary;

        // Assert: GroupBy naturally keeps the first seen key first. 
        // Wizard was seen at Level 1, so it groups to Wizard 2, Fighter 2.
        Assert.That(summary, Is.EqualTo("Wizard 2 Fighter 2"));
    }
}
