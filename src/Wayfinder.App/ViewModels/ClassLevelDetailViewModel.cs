using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.App.Services;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Enums;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;

namespace Wayfinder.UI.ViewModels;

public partial class ClassLevelDetailViewModel : ObservableObject
{
    private readonly IClassLevelEngine? _engine; // Nullable because read-only mode doesn't need it
    private readonly CharacterStateService _stateService;
    private readonly IClassLibrary _classLibrary;

    [ObservableProperty] private int _characterLevel;
    [ObservableProperty] private string _className = string.Empty;
    [ObservableProperty] private AbilityScore? _abilityScoreIncrease;
    [ObservableProperty] private List<string> _validationErrors = new();

    // Make this an observable property so we can set it explicitly
    [ObservableProperty] private IEnumerable<ClassFeatureDefinition> _gainedFeatures = Enumerable.Empty<ClassFeatureDefinition>();

    public bool IsReadOnly { get; }
    public bool IsValid => !ValidationErrors.Any();
    public bool GrantsAbilityScoreIncrease { get; }

    // --- CONSTRUCTOR 1: DRAFT MODE ---
    public ClassLevelDetailViewModel(
        IClassLevelEngine engine,
        CharacterStateService stateService,
        IClassLibrary classLibrary,
        int level)
    {
        _engine = engine;
        _stateService = stateService;
        _classLibrary = classLibrary;

        IsReadOnly = false;
        CharacterLevel = level;
        GrantsAbilityScoreIncrease = _engine.IsAbilityScoreIncreaseLevel(level);
    }

    // --- CONSTRUCTOR 2: READ-ONLY HISTORY MODE ---
    public ClassLevelDetailViewModel(
        CharacterStateService stateService,
        IClassLibrary classLibrary,
        HydratedClassLevel pastLevel)
    {
        _stateService = stateService;
        _classLibrary = classLibrary;

        IsReadOnly = true;
        CharacterLevel = pastLevel.CharacterLevel;
        ClassName = pastLevel.ClassDefinition.Name;
        AbilityScoreIncrease = pastLevel.IncreasedAbilityScore;
        GrantsAbilityScoreIncrease = pastLevel.GrantsAbilityScoreIncrease;

        // Directly load the features using the known internal class level
        var classDef = _classLibrary.GetClassDefinition(ClassName);
        var levelDef = classDef?.Levels[pastLevel.ClassLevel];
        GainedFeatures = levelDef?.ClassFeatures ?? Enumerable.Empty<ClassFeatureDefinition>();
    }

    partial void OnClassNameChanged(string value)
    {
        if (IsReadOnly) return; // Don't recalculate if we are just viewing history

        Validate();
        UpdateGainedFeaturesForDraft();
    }

    private void UpdateGainedFeaturesForDraft()
    {
        if (string.IsNullOrWhiteSpace(ClassName))
        {
            GainedFeatures = Enumerable.Empty<ClassFeatureDefinition>();
            return;
        }

        var classDef = _classLibrary.GetClassDefinition(ClassName);
        if (classDef == null) return;

        int existingClassLevels = _stateService.ActiveSheet?.ClassLevels
            .Count(l => l.ClassDefinition.Name == ClassName && l.CharacterLevel < CharacterLevel) ?? 0;

        var levelDef = classDef.Levels[existingClassLevels + 1];
        GainedFeatures = levelDef?.ClassFeatures ?? Enumerable.Empty<ClassFeatureDefinition>();
    }

    public void Validate()
    {
        if (IsReadOnly || _engine == null) return;

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
            AbilityScoreIncrease = AbilityScoreIncrease
        };
    }
}
