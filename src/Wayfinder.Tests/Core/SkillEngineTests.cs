using NUnit.Framework;
using Wayfinder.Core.Constants;
using Wayfinder.Core.Data; // Assuming this is where SkillLibrary lives
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Rules.Engines;

[TestFixture]
public class SkillEngineTests
{
    private SkillEngine _engine;
    private SkillLibrary _skillLibrary;

    [SetUp]
    public void Setup()
    {
        _skillLibrary = new SkillLibrary();

        // Seed the real library with a controlled set of core skills for testing
        // (You can easily swap this to StandardSkills.GetCoreSkills() if you want the full list!)
        _skillLibrary.Seed(new List<SkillDefinition>
        {
            CreateSkillDef("Acrobatics", AbilityScore.Dexterity),
            CreateSkillDef("Stealth", AbilityScore.Dexterity)
        });

        _engine = new SkillEngine(_skillLibrary);
    }

    #region Helper Methods
    private SkillDefinition CreateSkillDef(string name, AbilityScore ability, bool isTrainedOnly = false, bool isBackground = false) =>
        new SkillDefinition { Name = name, DefaultAbility = ability, IsTrainedOnly = isTrainedOnly, IsBackground = isBackground };

    private HydratedClassLevel CreateClassLevel(int level, params string[] classSkills) =>
        new HydratedClassLevel
        {
            ClassLevel = level,
            ClassDefinition = new ClassDefinition { ClassSkills = classSkills.ToList() }
        };

    private Func<AbilityScore, int> GetMockAbilityScores() =>
        (ability) => ability switch
        {
            AbilityScore.Dexterity => 14, // +2 Mod
            AbilityScore.Intelligence => 16, // +3 Mod
            _ => 10 // +0 Mod
        };
    #endregion


    [Test]
    public void GetAvailableSkills_WithCustomOverride_PrefersCustomSkillOverBase()
    {
        // Arrange: Override Stealth to be Intelligence-based instead of Dex
        var customSkills = new List<SkillDefinition>
        {
            CreateSkillDef("Stealth", AbilityScore.Intelligence)
        };

        // Act
        var result = _engine.GetAvailableSkills(customSkills).ToList();
        var stealth = result.First(s => s.Name == "Stealth");

        // Assert
        Assert.That(stealth.DefaultAbility, Is.EqualTo(AbilityScore.Intelligence), "Custom override should win against the base library.");
    }

    [Test]
    public void CalculateSkills_ClassSkillWithRanks_PopulatesAllFacadePropertiesCorrectly()
    {
        // Arrange
        var availableSkills = _skillLibrary.GetAllBaseSkills();
        var classLevels = new[] { CreateClassLevel(1, "Acrobatics") }; // Acrobatics IS a class skill
        var choices = new[] { new SkillRankChoice { SkillName = "Acrobatics", Ranks = 2 } };
        var effects = new List<ActiveEffect>(); // No magic items yet

        // Act
        var result = _engine.CalculateSkills(choices, classLevels, availableSkills, GetMockAbilityScores(), effects);
        var acrobatics = result.First(s => s.Name == "Acrobatics");

        // Assert (Math: 2 Ranks + 2 Dex + 3 Class Skill = 7)
        Assert.That(acrobatics.TotalRanks, Is.EqualTo(2));
        Assert.That(acrobatics.TotalBonus, Is.EqualTo(7));

        // Facade Assertions - Proves the getters read the audit log correctly!
        Assert.That(acrobatics.AbilityModifier, Is.EqualTo(2));
        Assert.That(acrobatics.ClassSkillBonus, Is.EqualTo(3));

        // Total (7) - Ranks (2) - Ability (2) - ClassSkill (3) = 0
        Assert.That(acrobatics.MiscBonus, Is.EqualTo(0));
    }

    [Test]
    public void CalculateSkills_IntegratesGlobalEffectsBus_CalculatesMiscBonus()
    {
        // Arrange
        var availableSkills = _skillLibrary.GetAllBaseSkills();
        var classLevels = new[] { CreateClassLevel(1, "Acrobatics") };
        var choices = new[] { new SkillRankChoice { SkillName = "Acrobatics", Ranks = 1 } };

        // Add a magic item buff!
        var effects = new List<ActiveEffect>
        {
            new ActiveEffect { TargetStatName = "Acrobatics", SourceName = "Boots of Elvenkind", Value = 5, Type = ModifierType.Competence }
        };

        // Act
        var result = _engine.CalculateSkills(choices, classLevels, availableSkills, GetMockAbilityScores(), effects);
        var acrobatics = result.First(s => s.Name == "Acrobatics");

        // Assert (Math: 1 Rank + 2 Dex + 3 Class Skill + 5 Boots = 11)
        Assert.That(acrobatics.TotalBonus, Is.EqualTo(11));

        // Total (11) - Ranks (1) - Ability (2) - ClassSkill (3) = 5
        Assert.That(acrobatics.MiscBonus, Is.EqualTo(5), "Misc bonus should exclusively represent the effects bus.");

        // Prove the audit log tracked the specific item
        var bootsMod = acrobatics.Score.Modifiers.FirstOrDefault(m => m.SourceName == "Boots of Elvenkind");
        Assert.That(bootsMod, Is.Not.Null);
        Assert.That(bootsMod!.Value, Is.EqualTo(5));
    }

    [Test]
    public void CalculateProposedTotalBonus_AddingFirstRankToClassSkill_TriggersBump()
    {
        // Arrange: A Class Skill currently sitting at 0 ranks
        var availableSkills = _skillLibrary.GetAllBaseSkills();
        var classLevels = new[] { CreateClassLevel(1, "Stealth") };
        var choices = Array.Empty<SkillRankChoice>(); // 0 ranks

        var calculatedSkills = _engine.CalculateSkills(choices, classLevels, availableSkills, GetMockAbilityScores(), new List<ActiveEffect>());
        var stealth = calculatedSkills.First(s => s.Name == "Stealth");

        // Base math for 0 ranks: 0 (Ranks) + 2 (Dex Mod) = 2
        Assert.That(stealth.TotalBonus, Is.EqualTo(2));

        // Act: User clicks "+" to propose 1 rank
        int proposedBonus = _engine.CalculateProposedTotalBonus(stealth, 1);

        // Assert: 1 (Rank) + 2 (Dex) + 3 (Class Skill triggered!) = 6
        Assert.That(proposedBonus, Is.EqualTo(6), "Adding the first rank to a class skill should add both the rank and the +3 bump.");
    }

    [Test]
    public void CalculateProposedTotalBonus_RemovingLastRankFromClassSkill_RemovesBump()
    {
        // Arrange: A Class Skill currently sitting at 1 rank
        var availableSkills = _skillLibrary.GetAllBaseSkills();
        var classLevels = new[] { CreateClassLevel(1, "Stealth") };
        var choices = new[] { new SkillRankChoice { SkillName = "Stealth", Ranks = 1 } };

        var calculatedSkills = _engine.CalculateSkills(choices, classLevels, availableSkills, GetMockAbilityScores(), new List<ActiveEffect>());
        var stealth = calculatedSkills.First(s => s.Name == "Stealth");

        // Base math for 1 rank: 1 (Ranks) + 2 (Dex Mod) + 3 (Class Skill) = 6
        Assert.That(stealth.TotalBonus, Is.EqualTo(6));

        // Act: User clicks "-" to drop back to 0 ranks
        int proposedBonus = _engine.CalculateProposedTotalBonus(stealth, 0);

        // Assert: 0 (Ranks) + 2 (Dex) = 2
        Assert.That(proposedBonus, Is.EqualTo(2), "Removing the last rank should drop the rank AND the +3 bump.");
    }

    [Test]
    public void CalculateProposedTotalBonus_AddingRanksToNonClassSkill_UpdatesLinearly()
    {
        // Arrange: A Non-Class Skill currently sitting at 2 ranks
        var availableSkills = _skillLibrary.GetAllBaseSkills();
        var classLevels = new[] { CreateClassLevel(1, "Acrobatics") }; // Stealth is NOT a class skill here
        var choices = new[] { new SkillRankChoice { SkillName = "Stealth", Ranks = 2 } };

        var calculatedSkills = _engine.CalculateSkills(choices, classLevels, availableSkills, GetMockAbilityScores(), new List<ActiveEffect>());
        var stealth = calculatedSkills.First(s => s.Name == "Stealth");

        // Base math for 2 ranks: 2 (Ranks) + 2 (Dex Mod) = 4
        Assert.That(stealth.TotalBonus, Is.EqualTo(4));

        // Act: User proposes changing from 2 ranks to 5 ranks (+3 delta)
        int proposedBonus = _engine.CalculateProposedTotalBonus(stealth, 5);

        // Assert: 5 (Ranks) + 2 (Dex) = 7
        Assert.That(proposedBonus, Is.EqualTo(7));
    }


    [Test]
    public void ValidateSkillRanksForLevel_RankCapExceeded_AddsError()
    {
        // Arrange
        var history = new[] { new SkillRankChoice { SkillName = "Stealth", Ranks = 2, CharacterLevel = 1 } };
        var proposed = new[] { new SkillRankChoice { SkillName = "Stealth", Ranks = 2, CharacterLevel = 3 } }; // Asking for level 3
        var available = _skillLibrary.GetAllBaseSkills();

        // Act (Total ranks = 4. Target Level = 3)
        var result = _engine.ValidateSkillRanksForLevel(3, proposed, history, available);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.Contains("Cannot have 4 ranks")), Is.True);
    }
}
