namespace Wayfinder.Core.Rules.Engines;

using System.Collections.Generic;
using System.Linq;
using Wayfinder.Core.Constants;
using Wayfinder.Core.Data.Interfaces;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.DomainModels.Skills;
using Wayfinder.Core.DomainModels.Stats;
using Wayfinder.Core.Logic;
using Wayfinder.Core.Logic.Interfaces;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Results;
using Wayfinder.Core.Rules.Calculators;

public class SkillEngine : ISkillEngine
{
    private readonly ISkillLibrary _skillLibrary;

    public SkillEngine(ISkillLibrary skillLibrary)
    {
        _skillLibrary = skillLibrary;
    }

    public IEnumerable<SkillDefinition> GetAvailableSkills(IEnumerable<SkillDefinition>? customSkills)
    {
        var baseLibrary = _skillLibrary.GetAllBaseSkills();

        // Defensively handle nulls before concatenating
        var safeCustomSkills = customSkills ?? Enumerable.Empty<SkillDefinition>();

        return safeCustomSkills
            .Concat(baseLibrary)
            .GroupBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First());
    }

    public IReadOnlyList<CalculatedSkill> CalculateSkills(
            IEnumerable<SkillRankChoice> choices,
            IEnumerable<HydratedClassLevel> classLevels,
            IEnumerable<SkillDefinition> availableSkills,
            Func<AbilityScore, int> getAbilityScore,
            IEnumerable<ActiveEffect> globalEffects)
    {
        var calculatedSkills = new List<CalculatedSkill>();

        var allClassSkills = classLevels
            .SelectMany(l => l.ClassDefinition.ClassSkills)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet();

        var ranksByName = choices
            .GroupBy(c => c.SkillName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Sum(c => c.Ranks), StringComparer.OrdinalIgnoreCase);

        foreach (var def in availableSkills)
        {
            int ranks = ranksByName.TryGetValue(def.Name, out var r) ? r : 0;
            int score = getAbilityScore(def.DefaultAbility);
            int abilityMod = AbilityScoreCalculator.CalculateModifier(score);

            bool isClassSkill = allClassSkills.Contains(def.Name);

            // 2. Build the base modifiers for the StatCalculator
            var baseModifiers = new List<StatModifier>
            {
                new StatModifier(def.DefaultAbility.ToString(), abilityMod, ModifierType.Ability, true)
            };

            if (isClassSkill && ranks > 0)
            {
                baseModifiers.Add(new StatModifier("Class Skill", 3, ModifierType.Untyped, true));
            }


            var modifiableScore = StatCalculator.Calculate(
                statName: def.Name,
                baseValue: ranks, // Ranks act as the "Base" value for skills
                globalEffects: globalEffects,
                baseModifiers: baseModifiers
            );

            calculatedSkills.Add(new CalculatedSkill
            {
                Name = def.Name,
                KeyAbility = def.DefaultAbility,
                IsClassSkill = isClassSkill,
                IsTrainedOnly = def.IsTrainedOnly,
                IsBackground = def.IsBackground,
                TotalRanks = ranks,
                Score = modifiableScore // Assign the calculated stat!
            });
        }

        return calculatedSkills.OrderBy(s => s.Name).ToList();
    }

    public SkillValidationResult ValidateSkillRanksForLevel(
        int targetLevel,
        IEnumerable<SkillRankChoice> proposedChoicesForThisLevel,
        IEnumerable<SkillRankChoice> historicalChoices,
        IEnumerable<SkillDefinition> availableSkills) // Passed in from the Library or Sheet
    {
        var result = new SkillValidationResult();

        // Defensively filter history to only include choices up to the level asking for
        // validation. We ignore future levels allowing the caller to just pass in the full history
        // without needing to slice it themselves.
        var relevantHistory = historicalChoices.Where(c => c.CharacterLevel <= targetLevel);

        // Validate proposed choices belong to the target level
        var validProposed = new List<SkillRankChoice>();
        foreach (var choice in proposedChoicesForThisLevel)
        {
            if (choice.CharacterLevel != targetLevel)
            {
                result.AddError($"Proposed rank for {choice.SkillName} is tagged as Level {choice.CharacterLevel}, but we are validating Level {targetLevel}.");
                continue;
            }
            if (choice.Ranks < 0)
            {
                result.AddError($"Cannot spend negative ranks on {choice.SkillName}.");
                continue;
            }
            validProposed.Add(choice);
        }

        // Validate Rank Cap (Total Ranks <= Target Level)
        var allChoices = relevantHistory.Concat(proposedChoicesForThisLevel).ToList();

        var ranksBySkill = allChoices
            .GroupBy(c => c.SkillName)
            .Select(g => new { SkillName = g.Key, TotalRanks = g.Sum(c => c.Ranks) });

        foreach (var skill in ranksBySkill)
        {
            if (skill.TotalRanks > targetLevel)
            {
                result.AddError($"Cannot have {skill.TotalRanks} ranks in {skill.SkillName}. Max ranks cannot exceed character level ({targetLevel}).");
            }
        }

        // 2. Validate existence and separate Standard vs Background point expenditures
        foreach (var choice in proposedChoicesForThisLevel)
        {
            if (choice.Ranks < 0)
            {
                result.AddError($"Cannot spend negative ranks on {choice.SkillName}.");
                continue;
            }

            var def = availableSkills.FirstOrDefault(s => s.Name == choice.SkillName);
            if (def == null)
            {
                result.AddError($"Skill '{choice.SkillName}' is not a recognized skill for this character.");
                continue;
            }

            if (def.IsBackground)
            {
                result.BackgroundRanksSpent += choice.Ranks;
            }
            else
            {
                result.StandardRanksSpent += choice.Ranks;
            }
        }

        return result;
    }

    public IReadOnlyList<SkillLevelEconomy> CalculateSkillEconomy(
        IEnumerable<HydratedClassLevel> classLevels,
        int intelligenceScore)
    {
        var economyList = new List<SkillLevelEconomy>();

        foreach (var level in classLevels)
        {
            economyList.Add(new SkillLevelEconomy
            {
                Level = level.ClassLevel,
                MaxStandardPoints = SkillPointCalculator.CalculateStandardSkillPoints(level, intelligenceScore),
                MaxBackgroundPoints = SkillPointCalculator.CalculateBackgroundSkillPoints()
            });
        }

        return economyList;
    }

    /// <summary>
    /// Calculates what a skill's total bonus WOULD be if the user changes their invested ranks.
    /// </summary>
    public int CalculateProposedTotalBonus(CalculatedSkill currentSkill, int proposedTotalRanks)
    {
        // 1. What is the current value of the math we are allowed to change?
        int currentVariableMath = currentSkill.TotalRanks +
            GetClassSkillBonus(currentSkill.IsClassSkill, currentSkill.TotalRanks);

        // 2. What would the new value of that math be?
        int proposedVariableMath = proposedTotalRanks +
            GetClassSkillBonus(currentSkill.IsClassSkill, proposedTotalRanks);

        // 3. What is the exact difference?
        int delta = proposedVariableMath - currentVariableMath;

        // 4. Apply that difference to the fully-audited total!
        return currentSkill.Score.Total + delta;
    }

    /// <summary>
    /// Return the one-time class skill with any ranks bonus
    /// </summary>
    /// <param name="isClassSkill"></param>
    /// <param name="ranks"></param>
    /// <returns></returns>
    private static int GetClassSkillBonus(bool isClassSkill, int ranks) => (isClassSkill && ranks > 0) ? 3 : 0;
}
