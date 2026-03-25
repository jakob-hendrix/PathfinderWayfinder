using Wayfinder.Core.Constants;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Logic.Interfaces;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Results;

namespace Wayfinder.Core.Rules.Engines;

public class ClassLevelEngine : IClassLevelEngine
{
    private readonly IClassFeatureRegistry _featureRegistry;
    private readonly IClassLibrary _classLibrary;

    public ClassLevelEngine(IClassLibrary classLibrary, IClassFeatureRegistry featureRegistry)
    {
        _classLibrary = classLibrary;
        _featureRegistry = featureRegistry;
    }

    public IEnumerable<ActiveEffect> GenerateClassFeatureEffects(IEnumerable<HydratedClassLevel> classLevels)
    {
        var generatedEffects = new List<ActiveEffect>();

        var featureRanks = classLevels
                    .SelectMany(l => l.ClassDefinition.Levels.TryGetValue(l.ClassLevel, out var levelDef)
                        ? levelDef.ClassFeatures
                        : Enumerable.Empty<ClassFeatureDefinition>())
                    .GroupBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

        // 2. Ask the registry to build the math
        foreach (var kvp in featureRanks)
        {
            string featureName = kvp.Key;
            int rank = kvp.Value;

            if (_featureRegistry.TryGetBehavior(featureName, out var behavior))
            {
                generatedEffects.AddRange(behavior.GenerateEffects(rank));
            }
        }

        return generatedEffects;
    }

    public ClassHydrationResult HydrateLevels(IEnumerable<ClassLevelChoice> levelChoices)
    {
        var result = new ClassHydrationResult();
        //var orderedChoices = levelChoices.OrderBy(c => c.CharacterLevel).ToList();
        //var classLevelTracker = new Dictionary<string, int>();

        foreach (var choice in levelChoices.OrderBy(c => c.CharacterLevel))
        {
            var classDef = _classLibrary.GetClassDefinition(choice.ClassName);
            if (classDef == null)
            {
                result.Errors.Add($"Level {choice.CharacterLevel}: Class name '{choice.ClassName}' not found in library");
                break;
            }

            // Build base hydrated level
            var hydratedLevel = new HydratedClassLevel
            {
                CharacterLevel = choice.CharacterLevel,
                ClassLevel = result.HydratedLevels.Count(
                    l => l.ClassDefinition.Name == choice.ClassName) + 1,
                ClassDefinition = classDef,
                BaseSkillPointsGranted = classDef.SkillPointsPerLevel,
                HpGained = choice.HpGained,
            };

            // FCB logic
            hydratedLevel.IsFavoredClass = FavoredClassBonusEngine.IsFavoredClass(
                choice.ClassName,
                choice.CharacterLevel,
                result.HydratedLevels);

            if (hydratedLevel.IsFavoredClass)
            {
                hydratedLevel.AppliedFavoredClassBonus = choice.SelectedFavoredClassBonus;
            }
            else
            {
                hydratedLevel.AppliedFavoredClassBonus = FavoredClassBonus.None;
            }

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
