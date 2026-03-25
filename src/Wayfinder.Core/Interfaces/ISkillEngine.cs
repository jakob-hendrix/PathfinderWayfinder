namespace Wayfinder.Core.Logic.Interfaces;

using System;
using System.Collections.Generic;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.DomainModels.Skills;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Results;

public interface ISkillEngine
{
    /// <summary>
    /// Safely merges base skills and custom entity skills, allowing custom skills to override base skills of the same name.
    /// </summary>
    IEnumerable<SkillDefinition> GetAvailableSkills(IEnumerable<SkillDefinition> customSkills);

    SkillValidationResult ValidateSkillRanksForLevel(
        int targetLevel,
        IEnumerable<SkillRankChoice> proposedChoicesForThisLevel,
        IEnumerable<SkillRankChoice> historicalChoices,
        IEnumerable<SkillDefinition> availableSkills);

    IReadOnlyList<CalculatedSkill> CalculateSkills(
        IEnumerable<SkillRankChoice> choices,
        IEnumerable<HydratedClassLevel> classLevels,
        IEnumerable<SkillDefinition> availableSkills,
        Func<AbilityScore, int> getAbilityScore,
        IEnumerable<ActiveEffect> globalEffects);

    IReadOnlyList<SkillLevelEconomy> CalculateSkillEconomy(
        IEnumerable<HydratedClassLevel> classLevels,
        int intelligenceScore);

    int CalculateProposedTotalBonus(CalculatedSkill baseSkillState, int proposedTotalRanks);
}
