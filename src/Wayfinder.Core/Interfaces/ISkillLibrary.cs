namespace Wayfinder.Core.Data.Interfaces;

using System.Collections.Generic;
using Wayfinder.Core.DataDefinitions;

public interface ISkillLibrary
{
    IEnumerable<SkillDefinition> GetAllBaseSkills();
    SkillDefinition? GetSkill(string name);
    void Seed(IEnumerable<SkillDefinition> skills);
}
