namespace Wayfinder.UI.ViewModels;

using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.App.Services;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Enums;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;

// Represents the 'draft' of a class level
public partial class ClassLevelDetailViewModel : ObservableObject
{
    private readonly CharacterStateService _stateService;
    private readonly IClassLevelEngine _engine;
    private readonly IClassLibrary _classLibrary;

    [ObservableProperty] private int _characterLevel;
    [ObservableProperty] private string _className = string.Empty;
    [ObservableProperty] private FavoredClassBonus _fcbChoice;
    [ObservableProperty] private AbilityScore? _abilityScoreIncrease;

    [ObservableProperty] private List<string> _validationErrors = new();

    public bool IsValid => !ValidationErrors.Any();
    public bool GrantsAbilityScoreIncrease => _engine.IsAbilityScoreIncreaseLevel(CharacterLevel);
    public IEnumerable<ClassFeatureDefinition> GainedFeatures => GetGainedFeatures();

    public ClassLevelDetailViewModel(CharacterStateService stateService, IClassLevelEngine engine, IClassLibrary classLibrary, int level)
    {
        _stateService = stateService;
        _engine = engine;
        _classLibrary = classLibrary;
        CharacterLevel = level;
    }

    // CommunityToolkit hook: Fires automatically when _className changes
    partial void OnClassNameChanged(string value)
    {
        Validate();
        // Tell the UI that the GainedFeatures list needs to be re-rendered
        OnPropertyChanged(nameof(GainedFeatures));
    }

    // Call this whenever a property changes in the UI
    public void Validate()
    {
        var choice = ToChoice();
        ValidationErrors = _engine.ValidateChoice(choice);
        OnPropertyChanged(nameof(IsValid));
    }

    public ClassLevelChoice ToChoice()
    {
        return new ClassLevelChoice
        {
            CharacterLevel = CharacterLevel,
            ClassName = ClassName,
            SelectedFavoredClassBonus = FcbChoice,
            AbilityScoreIncrease = AbilityScoreIncrease
        };
    }

    private IEnumerable<ClassFeatureDefinition> GetGainedFeatures()
    {
        if (string.IsNullOrWhiteSpace(ClassName)) return Enumerable.Empty<ClassFeatureDefinition>();

        var classDef = _classLibrary.GetClassDefinition(ClassName);
        if (classDef == null) return Enumerable.Empty<ClassFeatureDefinition>();

        // 1. Figure out how many levels of THIS class the character already has
        // We only count levels that come BEFORE the current draft level
        int existingClassLevels = _stateService.ActiveSheet?.ClassLevels?
            .Count(l => l.ClassDefinition.Name == ClassName && l.CharacterLevel < CharacterLevel) ?? 0;

        // 2. The level they are about to take is existing + 1
        int newClassLevel = existingClassLevels + 1;

        // 3. Find the definition for that specific class level
        var levelDef = classDef.Levels[newClassLevel];

        return levelDef?.ClassFeatures ?? Enumerable.Empty<ClassFeatureDefinition>();
    }
}
