namespace Wayfinder.Core.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using Wayfinder.Core.Data.Interfaces;
using Wayfinder.Core.DataDefinitions;

public class SkillLibrary : ISkillLibrary
{
    private readonly Dictionary<string, SkillDefinition> _skills = new(StringComparer.OrdinalIgnoreCase);

    public IEnumerable<SkillDefinition> GetAllBaseSkills()
    {
        return _skills.Values.OrderBy(s => s.Name);
    }

    public SkillDefinition? GetSkill(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;

        _skills.TryGetValue(name, out var skill);
        return skill;
    }

    public void Seed(IEnumerable<SkillDefinition> skills)
    {
        foreach (var skill in skills)
        {
            // Using the indexer [] instead of Add() ensures that if you accidentally 
            // seed a duplicate, it just overwrites it safely instead of crashing.
            _skills[skill.Name] = skill;
        }
    }
}
