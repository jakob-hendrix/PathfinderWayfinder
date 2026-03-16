namespace Wayfinder.Core.Rules.Engines;

using System.Collections.Generic;
using System.Linq;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Results;

public static class SkillEngine
{
    public static SkillValidationResult ValidateSkillRanksForLevel(
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
}
