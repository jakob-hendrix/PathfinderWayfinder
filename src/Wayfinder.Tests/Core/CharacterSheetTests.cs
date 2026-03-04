using Moq;
using NUnit.Framework;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Enums;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Results;

namespace Wayfinder.Tests.DomainModels.Characters;

[TestFixture]
public class CharacterSheetTests
{
    private CharacterEntity _entity;
    private Mock<IPathfinderRulesEngine> _mockEngine;
    private Mock<IClassLevelEngine> _mockClassEngine;
    private Mock<IRaceFactory> _mockRaceFactory;
    private CharacterSheet _sheet;

    [SetUp]
    public void Setup()
    {
        // 1. Setup the raw facts
        _entity = new CharacterEntity();
        _entity.BaseStrength = 10;
        _entity.BaseDexterity = 10;
        _entity.BaseConstitution = 10;
        _entity.BaseIntelligence = 10;
        _entity.BaseWisdom = 10;
        _entity.BaseCharisma = 10;

        // 2. Setup the Engine Mocks
        _mockEngine = new Mock<IPathfinderRulesEngine>();
        _mockClassEngine = new Mock<IClassLevelEngine>();
        _mockRaceFactory = new Mock<IRaceFactory>();

        _mockRaceFactory
                .Setup(r => r.BuildRace(It.IsAny<RaceChoices>()))
                .Returns(new RaceResolutionResult { HydratedRace = null });

        // Link the class engine to the rules engine (adjust based on your exact interface)
        _mockEngine
            .Setup(e => e.ClassLevelEngine)
            .Returns(_mockClassEngine.Object);

        _mockEngine
            .Setup(e => e.RaceFactory)
            .Returns(_mockRaceFactory.Object);

        // 3. Initialize the Sheet
        _sheet = new CharacterSheet(_entity, _mockEngine.Object);
    }

    // --- ADD / REMOVE CLASS LEVEL TESTS ---

    [Test]
    public void AddClassLevel_WithValidNextLevel_AddsToEntityAndTriggersRebuild()
    {
        // Arrange
        var choice = new ClassLevelChoice { CharacterLevel = 1, ClassName = "Fighter" };

        // Mock the engine to return a successful hydration when RebuildClasses is called
        _mockClassEngine
            .Setup(e => e.HydrateLevels(It.IsAny<IEnumerable<ClassLevelChoice>>()))
            .Returns(new ClassHydrationResult { HydratedLevels = new List<HydratedClassLevel>() });

        // Act
        _sheet.AddClassLevel(choice);

        // Assert
        Assert.That(_entity.ClassLevelChoices, Has.Count.EqualTo(1));
        Assert.That(_entity.ClassLevelChoices[0].ClassName, Is.EqualTo("Fighter"));

        // Verify the sheet actually asked the engine to rebuild
        _mockClassEngine.Verify(e => e.HydrateLevels(It.IsAny<IEnumerable<ClassLevelChoice>>()), Times.Once);
    }

    [Test]
    public void RemoveHighestClassLevel_WithExistingLevels_RemovesTopLevelAndRebuilds()
    {
        // Arrange
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 1, ClassName = "Fighter" });
        _entity.ClassLevelChoices.Add(new ClassLevelChoice { CharacterLevel = 2, ClassName = "Wizard" });

        _mockClassEngine
            .Setup(e => e.HydrateLevels(It.IsAny<IEnumerable<ClassLevelChoice>>()))
            .Returns(new ClassHydrationResult { HydratedLevels = new List<HydratedClassLevel>() });

        // Act
        _sheet.RemoveHighestClassLevel();

        // Assert
        Assert.That(_entity.ClassLevelChoices, Has.Count.EqualTo(1));
        Assert.That(_entity.ClassLevelChoices[0].ClassName, Is.EqualTo("Fighter"), "Should remove the last added level (Wizard).");
        _mockClassEngine.Verify(e => e.HydrateLevels(It.IsAny<IEnumerable<ClassLevelChoice>>()), Times.Once);
    }

    // --- ABILITY SCORE CALCULATION TESTS ---

    [Test]
    public void StrengthProperty_CalculatesBasePlusClassLevelBumps()
    {
        // Arrange: Base Score
        _entity.BaseStrength = 15;

        // Arrange: Class Level Bump (+1 Str at Level 4)
        // We add the fact to the entity so the sheet can read the user's choice
        _entity.ClassLevelChoices.Add(new ClassLevelChoice
        {
            CharacterLevel = 4,
            ClassName = "Fighter",
            AbilityScoreIncrease = AbilityScore.Strength
        });

        // Mock the hydration result to reflect that level 4 actually grants a bump
        var hydratedLevel = new HydratedClassLevel
        {
            CharacterLevel = 4,
            GrantsAbilityScoreIncrease = true
        };

        var hydrationResult = new ClassHydrationResult
        {
            HydratedLevels = new List<HydratedClassLevel> { hydratedLevel }
        };

        _mockClassEngine
            .Setup(e => e.HydrateLevels(It.IsAny<IEnumerable<ClassLevelChoice>>()))
            .Returns(hydrationResult);

        // Act
        _sheet.RebuildClasses(); // Force the sheet to ingest the mocked level data

        // This triggers your private CalculateAbilityScore(AbilityScore.Strength)
        int totalStrength = _sheet.Strength;

        // Assert: 15 (Base) + 1 (Level 4 Bump) = 16
        // (Racial bonuses are explicitly excluded until implemented)
        Assert.That(totalStrength, Is.EqualTo(16));
    }

    public void DexterityProperty_CalculatesBasePlusClassLevelBumps()
    {
        // Arrange: Base Score
        _entity.BaseDexterity = 15;

        // Arrange: Class Level Bump (+1 Str at Level 4)
        // We add the fact to the entity so the sheet can read the user's choice
        _entity.ClassLevelChoices.Add(new ClassLevelChoice
        {
            CharacterLevel = 4,
            ClassName = "Fighter",
            AbilityScoreIncrease = AbilityScore.Dexterity
        });

        // Mock the hydration result to reflect that level 4 actually grants a bump
        var hydratedLevel = new HydratedClassLevel
        {
            CharacterLevel = 4,
            GrantsAbilityScoreIncrease = true
        };

        var hydrationResult = new ClassHydrationResult
        {
            HydratedLevels = new List<HydratedClassLevel> { hydratedLevel }
        };

        _mockClassEngine
            .Setup(e => e.HydrateLevels(It.IsAny<IEnumerable<ClassLevelChoice>>()))
            .Returns(hydrationResult);

        // Act
        _sheet.RebuildClasses(); // Force the sheet to ingest the mocked level data

        // This triggers your private CalculateAbilityScore(AbilityScore.Strength)
        int totalStat = _sheet.Dexterity;

        // Assert: 15 (Base) + 1 (Level 4 Bump) = 16
        // (Racial bonuses are explicitly excluded until implemented)
        Assert.That(totalStat, Is.EqualTo(16));
    }

    public void ConstitutionProperty_CalculatesBasePlusClassLevelBumps()
    {
        // Arrange: Base Score
        _entity.BaseConstitution = 15;

        // Arrange: Class Level Bump (+1 Str at Level 4)
        _entity.ClassLevelChoices.Add(new ClassLevelChoice
        {
            CharacterLevel = 4,
            ClassName = "Fighter",
            AbilityScoreIncrease = AbilityScore.Constitution
        });

        var hydratedLevel = new HydratedClassLevel
        {
            CharacterLevel = 4,
            GrantsAbilityScoreIncrease = true
        };

        var hydrationResult = new ClassHydrationResult
        {
            HydratedLevels = new List<HydratedClassLevel> { hydratedLevel }
        };

        _mockClassEngine
            .Setup(e => e.HydrateLevels(It.IsAny<IEnumerable<ClassLevelChoice>>()))
            .Returns(hydrationResult);

        // Act
        _sheet.RebuildClasses(); // Force the sheet to ingest the mocked level data
        int totalStat = _sheet.Constitution;
        Assert.That(totalStat, Is.EqualTo(16));
    }

    public void IntelligenceProperty_CalculatesBasePlusClassLevelBumps()
    {
        // Arrange: Base Score
        _entity.BaseIntelligence = 15;
        _entity.ClassLevelChoices.Add(new ClassLevelChoice
        {
            CharacterLevel = 4,
            ClassName = "Fighter",
            AbilityScoreIncrease = AbilityScore.Intelligence
        });

        var hydratedLevel = new HydratedClassLevel
        {
            CharacterLevel = 4,
            GrantsAbilityScoreIncrease = true
        };

        var hydrationResult = new ClassHydrationResult
        {
            HydratedLevels = new List<HydratedClassLevel> { hydratedLevel }
        };

        _mockClassEngine
            .Setup(e => e.HydrateLevels(It.IsAny<IEnumerable<ClassLevelChoice>>()))
            .Returns(hydrationResult);

        // Act
        _sheet.RebuildClasses(); // Force the sheet to ingest the mocked level data

        int totalStat = _sheet.Intelligence;
        Assert.That(totalStat, Is.EqualTo(16));
    }

    public void WisdomProperty_CalculatesBasePlusClassLevelBumps()
    {
        // Arrange: Base Score
        _entity.BaseWisdom = 15;
        _entity.ClassLevelChoices.Add(new ClassLevelChoice
        {
            CharacterLevel = 4,
            ClassName = "Fighter",
            AbilityScoreIncrease = AbilityScore.Wisdom
        });

        var hydratedLevel = new HydratedClassLevel
        {
            CharacterLevel = 4,
            GrantsAbilityScoreIncrease = true
        };

        var hydrationResult = new ClassHydrationResult
        {
            HydratedLevels = new List<HydratedClassLevel> { hydratedLevel }
        };

        _mockClassEngine
            .Setup(e => e.HydrateLevels(It.IsAny<IEnumerable<ClassLevelChoice>>()))
            .Returns(hydrationResult);

        // Act
        _sheet.RebuildClasses(); // Force the sheet to ingest the mocked level data

        int totalStat = _sheet.Wisdom;
        Assert.That(totalStat, Is.EqualTo(16));
    }

    public void CharismaProperty_CalculatesBasePlusClassLevelBumps()
    {
        // Arrange: Base Score
        _entity.BaseCharisma = 15;
        _entity.ClassLevelChoices.Add(new ClassLevelChoice
        {
            CharacterLevel = 4,
            ClassName = "Fighter",
            AbilityScoreIncrease = AbilityScore.Charisma
        });

        var hydratedLevel = new HydratedClassLevel
        {
            CharacterLevel = 4,
            GrantsAbilityScoreIncrease = true
        };

        var hydrationResult = new ClassHydrationResult
        {
            HydratedLevels = new List<HydratedClassLevel> { hydratedLevel }
        };

        _mockClassEngine
            .Setup(e => e.HydrateLevels(It.IsAny<IEnumerable<ClassLevelChoice>>()))
            .Returns(hydrationResult);

        // Act
        _sheet.RebuildClasses(); // Force the sheet to ingest the mocked level data

        int totalStat = _sheet.Charisma;
        Assert.That(totalStat, Is.EqualTo(16));
    }

    // --- CLASS SUMMARY TESTS ---

    [Test]
    public void ClassSummary_WithMultipleClasses_FormatsCorrectly()
    {
        // Arrange
        var wizardDef = new ClassDefinition { Name = "Wizard" };
        var fighterDef = new ClassDefinition { Name = "Fighter" };

        var hydratedLevels = new List<HydratedClassLevel>
        {
            new HydratedClassLevel { CharacterLevel = 1, ClassDefinition = wizardDef },
            new HydratedClassLevel { CharacterLevel = 2, ClassDefinition = fighterDef },
            new HydratedClassLevel { CharacterLevel = 3, ClassDefinition = fighterDef },
            new HydratedClassLevel { CharacterLevel = 4, ClassDefinition = wizardDef }
        };

        _mockClassEngine
            .Setup(e => e.HydrateLevels(It.IsAny<IEnumerable<ClassLevelChoice>>()))
            .Returns(new ClassHydrationResult { HydratedLevels = hydratedLevels });

        // Act
        _sheet.RebuildClasses(); // Hydrate the internal state
        var summary = _sheet.ClassSummary;

        // Assert
        Assert.That(summary, Is.EqualTo("Wizard 2 Fighter 2"));
    }
}
