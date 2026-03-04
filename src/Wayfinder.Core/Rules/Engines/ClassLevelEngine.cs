using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Results;

namespace Wayfinder.Core.Rules.Engines;

public class ClassLevelEngine : IClassLevelEngine
{
    private readonly IClassLibrary _classLibrary;

    public ClassLevelEngine(IClassLibrary classLibrary)
    {
        _classLibrary = classLibrary;
    }

    public ClassHydrationResult HydrateLevels(IEnumerable<ClassLevelChoice> levelChoices)
    {
        var result = new ClassHydrationResult();
        var orderedChoices = levelChoices.OrderBy(c => c.CharacterLevel).ToList();
        var classLevelTracker = new Dictionary<string, int>();
        string? favoredClassName = null;

        for (int i = 0; i < orderedChoices.Count; i++)
        {
            var choice = orderedChoices[i];

            // Validation logic
            if (choice.CharacterLevel != i + 1)
            {
                result.Errors.Add($"Level sequence broken. Expect level {i + 1} but found {choice.CharacterLevel}");
                break;
            }

            var classDef = _classLibrary.GetClassDefinition(choice.ClassName);
            if (classDef == null)
            {
                result.Errors.Add($"Level {choice.CharacterLevel}: Class name '{choice.ClassName}' not found in library");
                break;
            }

            // Keep track of total levels in *this* class
            if (!classLevelTracker.ContainsKey(classDef.Name))
                classLevelTracker[classDef.Name] = 0;

            classLevelTracker[classDef.Name]++;

            // Favored Class is the level 1 class
            if (choice.CharacterLevel == 1)
                favoredClassName = classDef.Name;

            bool isFavored = classDef.Name == favoredClassName;

            // Build hydrated level
            var hydratedLevel = new HydratedClassLevel
            {
                CharacterLevel = choice.CharacterLevel,
                ClassLevel = classLevelTracker[classDef.Name],
                ClassDefinition = classDef,
                BaseSkillPointsGranted = classDef.SkillPointsPerLevel,
                IsFavoredClass = isFavored,
                AppliedFavoredClassBonus = isFavored ? choice.SelectedFavoredClassBonus : Enums.FavoredClassBonus.None
            };

            // 5. Grant standard progression feats. Odd levels
            if (choice.CharacterLevel % 2 != 0)
            {
                hydratedLevel.GrantedFeatSlots.Add(new GrantedFeatSlot
                {
                    // TODO: this will probably need to change
                    Source = $"Character Level {choice.CharacterLevel}",
                    GrantedAtCharacterLevel = choice.CharacterLevel,
                    Category = null
                });
            }

            // TODO: grant class-specific bonus feats and featurees here

            // Ability Score Increase (level 4, 8, etc)
            if (IsAbilityScoreIncreaseLevel(choice.CharacterLevel))
            {
                hydratedLevel.GrantsAbilityScoreIncrease = true;
                hydratedLevel.IncreasedAbilityScore = choice.AbilityScoreIncrease;
            }

            result.HydratedLevels.Add(hydratedLevel);
        }


        return result;
    }

    /// <summary>
    /// Determine if a given level qualifies for an ability score increase.
    /// </summary>
    /// <param name="characterLevel"></param>
    /// <returns></returns>
    public bool IsAbilityScoreIncreaseLevel(int characterLevel) => characterLevel > 0 && characterLevel % 4 == 0;

    public List<string> ValidateChoice(ClassLevelChoice choice)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(choice.ClassName))
            errors.Add("A class must be selected.");

        // Enforce an ability score choice every 4th level
        if (IsAbilityScoreIncreaseLevel(choice.CharacterLevel) && choice.AbilityScoreIncrease == null)
        {
            errors.Add($"Level {choice.CharacterLevel} requires an ability score increase selection.");
        }

        // TODO: Validate HP input (e.g., must be > 0 and <= max hit die)

        return errors;
    }
}
