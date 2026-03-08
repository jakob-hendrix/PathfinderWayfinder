using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Results;

namespace Wayfinder.Core.Interfaces;

public interface IClassLevelEngine
{
    public ClassHydrationResult HydrateLevels(IEnumerable<ClassLevelChoice> levelChoices);
    public List<string> ValidateChoice(ClassLevelChoice choice);
    public bool IsAbilityScoreIncreaseLevel(int characterLevel);
}
