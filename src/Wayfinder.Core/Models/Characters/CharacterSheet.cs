using System.Data;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.DomainModels.Skills;
using Wayfinder.Core.Enums;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Items;
using Wayfinder.Core.Rules.Calculators;

namespace Wayfinder.Core.Models.Characters;

/// <summary>
/// Rich Domain Model that represents a character with state - meaning class levels, ineventory, etc
/// </summary>
public class CharacterSheet
{
    private readonly IPathfinderRulesEngine _rulesEngine;

    public CharacterEntity BaseCharacter { get; }

    public CharacterSheet(CharacterEntity baseCharacterFacts, IPathfinderRulesEngine rulesEngine)
    {
        BaseCharacter = baseCharacterFacts;
        _rulesEngine = rulesEngine;
        Refresh();
    }

    #region Character Stats

    public Race? Race { get; private set; }
    public List<HydratedClassLevel>? ClassLevels { get; private set; }

    public int MaxHp => HitPointCalculator.CalculateMaxHp(ClassLevels, Constitution);
    // TODO: ATM this is just basic math.
    // I'd like to add a status indicator for "unconcious" or "dead" or "dying"
    public int CurrentHp => MaxHp - Wounds;
    public int TemporaryHp => BaseCharacter.TemporaryHp;
    public int Wounds => BaseCharacter.Wounds;
    public int NonLethalDamage => BaseCharacter.NonLethalDamage;

    public int BaseAttackBonus => BabCalculator.Calculate(ClassLevels);
    public int FortitudeSave => SaveCalculator.Calculate(ClassLevels, SaveType.Fortitude, Constitution);
    public int ReflexSave => SaveCalculator.Calculate(ClassLevels, SaveType.Reflex, Dexterity);
    public int WillSave => SaveCalculator.Calculate(ClassLevels, SaveType.Will, Wisdom);

    // Ability Scores
    public int Strength => CalculateAbilityScore(AbilityScore.Strength, BaseCharacter.BaseStrength);
    public int Dexterity => CalculateAbilityScore(AbilityScore.Dexterity, BaseCharacter.BaseDexterity);
    public int Constitution => CalculateAbilityScore(AbilityScore.Constitution, BaseCharacter.BaseConstitution);
    public int Intelligence => CalculateAbilityScore(AbilityScore.Intelligence, BaseCharacter.BaseIntelligence);
    public int Wisdom => CalculateAbilityScore(AbilityScore.Wisdom, BaseCharacter.BaseWisdom);
    public int Charisma => CalculateAbilityScore(AbilityScore.Charisma, BaseCharacter.BaseCharisma);

    public IEnumerable<SkillDefinition> AvailableSkills =>
        _rulesEngine.SkillEngine.GetAvailableSkills(BaseCharacter.CustomSkills);

    public IReadOnlyList<CalculatedSkill> Skills =>
        _rulesEngine.SkillEngine.CalculateSkills(
            BaseCharacter.SkillRankChoices,
            ClassLevels,
            AvailableSkills,
            GetAbilityScore);

    public IReadOnlyList<SkillLevelEconomy> SkillEconomy =>
        _rulesEngine.SkillEngine.CalculateSkillEconomy(ClassLevels, Intelligence);

    public void CommitSkillChoices(IEnumerable<SkillRankChoice> newChoices)
    {
        var levelsBeingUpdated = newChoices.Select(c => c.CharacterLevel).Distinct().ToHashSet();

        BaseCharacter.SkillRankChoices.RemoveAll(c => levelsBeingUpdated.Contains(c.CharacterLevel));
        BaseCharacter.SkillRankChoices.AddRange(newChoices);
    }

    #endregion

    public void UpdateVitals(int wounds, int nonLethalDamage, int temporaryHp)
    {
        BaseCharacter.Wounds = Math.Max(0, wounds);
        BaseCharacter.NonLethalDamage = Math.Max(0, nonLethalDamage);
        BaseCharacter.TemporaryHp = Math.Max(0, temporaryHp);
    }

    // Display the state of current class levels as Fighter 1 Wizard 2 etc
    public string ClassSummary
    {
        get
        {
            if (ClassLevels == null || !ClassLevels.Any())
                return "No Class Selected";

            // GroupBy naturally keeps the order of the first time it sees a key.
            // So if Wizard is at index 0, it becomes the first group.
            var summaryParts = ClassLevels
                .GroupBy(l => l.ClassDefinition.Name)
                .Select(g => $"{g.Key} {g.Count()}");

            return string.Join(" ", summaryParts);
        }
    }

    // Get a hydrated Race instance
    public void RebuildRace()
    {
        // The Factory builds it, and the Domain stores it
        var result = _rulesEngine.RaceFactory.BuildRace(BaseCharacter.RaceChoices);
        if (result.IsValid)
        {
            Race = result.HydratedRace;
        }
        else
        {
            // TODO: throw error?
            Race = null;
        }
    }

    // Get a hydrated Race instance
    public void RebuildClasses()
    {
        // The Factory builds it, and the Domain stores it
        var result = _rulesEngine.ClassLevelEngine.HydrateLevels(BaseCharacter.ClassLevelChoices);
        if (result.IsValid)
        {
            ClassLevels = result.HydratedLevels;
        }
        else
        {
            // TODO: throw error?
            ClassLevels = null;
        }
    }

    // Return a list of hydrated items from the current state of the base character's inventory
    public List<ItemInstance> GetHydratedInventory()
    {
        return BaseCharacter.Inventory.Select(item =>
        {
            var instance = _rulesEngine.ItemFactory.CreateItem(item.TemplateId);

            // TODO: may need more work here to apply custom item
            // 'facts' to this instance. Like, custom name, enchantments, etc
            // That or allow the ItemFactory to take an ItemInstance and make a copy

            return instance;
        }).ToList();
    }

    public void ToggleEquip(Guid instanceId)
    {
        var item = BaseCharacter.Inventory.FirstOrDefault(i => i.Id == instanceId);
        if (item != null)
        {
            // TODO: implement equippgin items
            // item.IsEquipped = !item.IsEquipped;
        }
    }

    public void ToggleCarried(Guid instanceId)
    {
        var item = BaseCharacter.Inventory.FirstOrDefault(i => i.Id == instanceId);
        if (item != null)
        {
            item.IsCarried = !item.IsCarried;
        }
    }

    // Sheet Actions
    public void AddClassLevel(ClassLevelChoice validChoice)
    {
        // Ensure we are adding exactly the next level in the sequence
        int expectedLevel = BaseCharacter.ClassLevelChoices.Count + 1;
        if (validChoice.CharacterLevel != expectedLevel) return;

        BaseCharacter.ClassLevelChoices.Add(validChoice);
        RebuildClasses(); // Re-runs the hydration engine!
    }

    public void RemoveHighestClassLevel()
    {
        if (BaseCharacter.ClassLevelChoices.Any())
        {
            BaseCharacter.ClassLevelChoices.RemoveAt(BaseCharacter.ClassLevelChoices.Count - 1);
            RebuildClasses();
        }
    }

    public void Refresh()
    {
        RebuildRace();
        RebuildClasses();
    }

    // Helper functions
    /// <summary>
    /// Calculate the current game-ready value of an ability score, taking into account levels, racial bonuses,
    /// buff and conditions, etc
    /// </summary>
    /// <param name="scoreType"></param>
    /// <param name="baseScore"></param>
    /// <returns></returns>
    private int CalculateAbilityScore(AbilityScore scoreType, int baseScore) => AbilityScoreCalculator.CalculateCurrentValue(baseScore, scoreType, ClassLevels);

    private int GetAbilityScore(AbilityScore ability)
    {
        return ability switch
        {
            AbilityScore.Strength => Strength,
            AbilityScore.Dexterity => Dexterity,
            AbilityScore.Constitution => Constitution,
            AbilityScore.Intelligence => Intelligence,
            AbilityScore.Wisdom => Wisdom,
            AbilityScore.Charisma => Charisma,
            _ => 10
        };
    }
}
