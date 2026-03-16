namespace Wayfinder.Core.Rules.Engines;

using System.Collections.Generic;
using System.Linq;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Results;

public static class SkillEngine
{
    public static SkillValidationResult ValidateSkillRanks(
        int characterLevel,
        IEnumerable<SkillRankChoice> proposedChoicesForThisLevel,
        IEnumerable<SkillRankChoice> historicalChoices,
        IEnumerable<SkillDefinition> availableSkills) // Passed in from the Library or Sheet
    {
        var result = new SkillValidationResult();

        // 1. Validate Rank Cap (Total Ranks <= Character Level)
        var allChoices = historicalChoices.Concat(proposedChoicesForThisLevel).ToList();

        var ranksBySkill = allChoices
            .GroupBy(c => c.SkillName)
            .Select(g => new { SkillName = g.Key, TotalRanks = g.Sum(c => c.Ranks) });

        foreach (var skill in ranksBySkill)
        {
            if (skill.TotalRanks > characterLevel)
            {
                result.AddError($"Cannot have {skill.TotalRanks} ranks in {skill.SkillName}. Max ranks cannot exceed character level ({characterLevel}).");
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
