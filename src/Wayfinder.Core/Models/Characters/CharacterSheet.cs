using System.Data;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.DomainModels.Skills;
using Wayfinder.Core.DomainModels.Stats;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Logic;
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

    // The Global Effect Bus
    public List<ActiveEffect> ActiveEffects { get; } = new();

    public HydratedRace? Race { get; private set; }

    #region HP
    public int MaxHp => HitPointCalculator.CalculateMaxHp(ClassLevels, Constitution.Total);
    // TODO: ATM this is just basic math.
    // I'd like to add a status indicator for "unconcious" or "dead" or "dying"
    public int CurrentHp => MaxHp - Wounds;
    public int TemporaryHp => BaseCharacter.TemporaryHp;
    public int Wounds => BaseCharacter.Wounds;
    public int NonLethalDamage => BaseCharacter.NonLethalDamage;
    #endregion

    #region Movement
    public ModifiableStat LandSpeed { get; private set; } = new ModifiableStat { Name = StatNames.LandSpeed };
    public ModifiableStat FlySpeed { get; private set; } = new ModifiableStat { Name = StatNames.FlySpeed };
    public ModifiableStat ClimbSpeed { get; private set; } = new ModifiableStat { Name = StatNames.ClimbSpeed };
    public ModifiableStat SwimSpeed { get; private set; } = new ModifiableStat { Name = StatNames.SwimSpeed };

    private void RecalculateMovement()
    {
        LandSpeed = SpeedCalculator.CalculateLandSpeed(ActiveEffects);
        FlySpeed = SpeedCalculator.CalculateFlySpeed(ActiveEffects);
        ClimbSpeed = SpeedCalculator.CalculateClimbSpeed(ActiveEffects);
        SwimSpeed = SpeedCalculator.CalculateSwimSpeed(ActiveEffects);
    }

    #endregion

    #region Combat Stats
    public int BaseAttackBonus => BabCalculator.Calculate(ClassLevels);
    #endregion

    #region Saving Throws
    public ModifiableStat Fortitude { get; private set; } = new ModifiableStat { Name = "Fortitude" };
    public ModifiableStat Reflex { get; private set; } = new ModifiableStat { Name = "Reflex" };
    public ModifiableStat Will { get; private set; } = new ModifiableStat { Name = "Will" };

    public void RecalculateSaves()
    {
        Fortitude = SaveCalculator.CalculateSave(
            saveName: "Fortitude",
            classLevels: ClassLevels,
            abilityScore: Constitution.Total,
            abilityName: "Constitution",
            globalEffects: ActiveEffects);

        Reflex = SaveCalculator.CalculateSave(
            saveName: "Reflex",
            classLevels: ClassLevels,
            abilityScore: Dexterity.Total,
            abilityName: "Dexterity",
            globalEffects: ActiveEffects);

        Will = SaveCalculator.CalculateSave(
            saveName: "Will",
            classLevels: ClassLevels,
            abilityScore: Wisdom.Total,
            abilityName: "Wisdom",
            globalEffects: ActiveEffects);
    }

    #endregion

    #region Ability Scores
    public ModifiableStat Strength { get; private set; } = new ModifiableStat { Name = StatNames.Strength };
    public ModifiableStat Dexterity { get; private set; } = new ModifiableStat { Name = StatNames.Dexterity };
    public ModifiableStat Constitution { get; private set; } = new ModifiableStat { Name = StatNames.Constitution };
    public ModifiableStat Intelligence { get; private set; } = new ModifiableStat { Name = StatNames.Intelligence };
    public ModifiableStat Wisdom { get; private set; } = new ModifiableStat { Name = StatNames.Wisdom };
    public ModifiableStat Charisma { get; private set; } = new ModifiableStat { Name = StatNames.Charisma };

    public void RecalculateAbilityScores()
    {
        Strength = AbilityScoreCalculator.CalculateAbilityScore(StatNames.Strength, BaseCharacter.BaseStrength, ClassLevels, ActiveEffects);
        Dexterity = AbilityScoreCalculator.CalculateAbilityScore(StatNames.Dexterity, BaseCharacter.BaseDexterity, ClassLevels, ActiveEffects);
        Constitution = AbilityScoreCalculator.CalculateAbilityScore(StatNames.Constitution, BaseCharacter.BaseConstitution, ClassLevels, ActiveEffects);
        Intelligence = AbilityScoreCalculator.CalculateAbilityScore(StatNames.Intelligence, BaseCharacter.BaseIntelligence, ClassLevels, ActiveEffects);
        Wisdom = AbilityScoreCalculator.CalculateAbilityScore(StatNames.Wisdom, BaseCharacter.BaseWisdom, ClassLevels, ActiveEffects);
        Charisma = AbilityScoreCalculator.CalculateAbilityScore(StatNames.Charisma, BaseCharacter.BaseCharisma, ClassLevels, ActiveEffects);
    }

    private int GetAbilityScore(AbilityScore ability)
    {
        return ability switch
        {
            AbilityScore.Strength => Strength.Total,
            AbilityScore.Dexterity => Dexterity.Total,
            AbilityScore.Constitution => Constitution.Total,
            AbilityScore.Intelligence => Intelligence.Total,
            AbilityScore.Wisdom => Wisdom.Total,
            AbilityScore.Charisma => Charisma.Total,
            _ => 10
        };
    }
    #endregion

    #region Skills

    public IEnumerable<SkillDefinition> AvailableSkills =>
        _rulesEngine.SkillEngine.GetAvailableSkills(BaseCharacter.CustomSkills);

    public IReadOnlyList<CalculatedSkill> Skills { get; private set; } = new List<CalculatedSkill>();

    public IReadOnlyList<SkillLevelEconomy> SkillEconomy =>
        _rulesEngine.SkillEngine.CalculateSkillEconomy(ClassLevels, Intelligence.Total);

    public void CommitSkillChoices(IEnumerable<SkillRankChoice> newChoices)
    {
        var levelsBeingUpdated = newChoices.Select(c => c.CharacterLevel).Distinct().ToHashSet();

        BaseCharacter.SkillRankChoices.RemoveAll(c => levelsBeingUpdated.Contains(c.CharacterLevel));
        BaseCharacter.SkillRankChoices.AddRange(newChoices);

        RecalculateSkills();
    }

    private void RecalculateSkills()
    {
        Skills = _rulesEngine.SkillEngine.CalculateSkills(
            BaseCharacter.SkillRankChoices,
            ClassLevels,
            AvailableSkills,
            GetAbilityScore,
            ActiveEffects);
    }

    #endregion

    public void UpdateVitals(int wounds, int nonLethalDamage, int temporaryHp)
    {
        BaseCharacter.Wounds = Math.Max(0, wounds);
        BaseCharacter.NonLethalDamage = Math.Max(0, nonLethalDamage);
        BaseCharacter.TemporaryHp = Math.Max(0, temporaryHp);
    }

    #region Class Levels
    public List<HydratedClassLevel>? ClassLevels { get; private set; } = new();

    // Display the state of current class levels as Fighter 1 Wizard 2 etc
    public string ClassSummary
    {
        get
        {
            if (!ClassLevels.Any())
                return "No Class Selected";

            // GroupBy naturally keeps the order of the first time it sees a key.
            // So if Wizard is at index 0, it becomes the first group.
            var summaryParts = ClassLevels
                .GroupBy(l => l.ClassDefinition.Name)
                .Select(g => $"{g.Key} {g.Count()}");

            return string.Join(" ", summaryParts);
        }
    }

    public void RebuildClasses()
    {
        // The Factory builds it, and the Domain stores it
        var result = _rulesEngine.ClassLevelEngine.HydrateLevels(BaseCharacter.ClassLevelChoices);

        if (!result.IsValid)
        {
            ClassLevels = new();
            return;
        }

        ClassLevels = result.HydratedLevels;

        // Remove all class features from effect bus
        ActiveEffects.RemoveAll(e => e.Category == EffectCategory.ClassFeature);

        var newClassFeatureEffects = _rulesEngine.ClassLevelEngine.GenerateClassFeatureEffects(ClassLevels);

        ActiveEffects.AddRange(newClassFeatureEffects);

        // More recalcs?
    }

    #endregion

    // Get a hydrated Race instance
    public void RebuildRace()
    {
        // The Factory builds it, and the Domain stores it
        ActiveEffects.RemoveAll(e => e.Category == EffectCategory.RacialTrait);

        var result = _rulesEngine.RaceFactory.BuildRace(BaseCharacter.RaceChoices);
        if (result.IsValid)
        {
            Race = result.HydratedRace;

            if (Race!.AddedActiveEffects.Any())
            {
                ActiveEffects.AddRange(Race.AddedActiveEffects);
            }
        }
        else
        {
            Race = null;
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
        // Rebuild the base facts
        RebuildRace();
        RebuildClasses();

        // Rebuild the stats
        RecalculateAbilityScores();
        RecalculateSaves();
        RecalculateMovement();
        RecalculateSkills();
    }
}
