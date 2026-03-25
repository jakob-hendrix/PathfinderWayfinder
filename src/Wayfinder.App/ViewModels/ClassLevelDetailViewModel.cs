using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.App.Services;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Rules.Engines;

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
    [ObservableProperty] private FavoredClassBonus _fcbChoice;
    [ObservableProperty] private int _hpGained;

    // Make this an observable property so we can set it explicitly
    [ObservableProperty] private IEnumerable<ClassFeatureDefinition> _gainedFeatures = Enumerable.Empty<ClassFeatureDefinition>();

    public bool IsReadOnly { get; }
    public bool IsValid => !ValidationErrors.Any();
    public bool GrantsAbilityScoreIncrease { get; }
    public bool IsFavoredClass { get; private set; }
    public string? AvailableRacialFcbDescription { get; private set; }
    public int ClassHitDie { get; private set; }
    public bool IsFirstLevel => CharacterLevel == 1; // TODO: move this to Hp Calculator

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
        HpGained = pastLevel.HpGained;
        ClassHitDie = pastLevel.ClassDefinition.HitDie;

        // Directly load the features using the known internal class level
        IsFavoredClass = pastLevel.IsFavoredClass;
        FcbChoice = pastLevel.AppliedFavoredClassBonus;

        var classDef = _classLibrary.GetClassDefinition(ClassName);
        var raceName = _stateService.ActiveSheet?.Race?.Name;

        if (IsFavoredClass)
        {
            AvailableRacialFcbDescription = FavoredClassBonusEngine.GetAlternateRacialFcbDescription(classDef, raceName);
        }

        var levelDef = classDef?.Levels[pastLevel.ClassLevel];
        GainedFeatures = levelDef?.ClassFeatures ?? Enumerable.Empty<ClassFeatureDefinition>();
    }

    // MVVM - maps to class name
    partial void OnClassNameChanged(string value)
    {
        if (IsReadOnly) return; // Don't recalculate if we are just viewing history

        var classDef = _classLibrary.GetClassDefinition(value);
        if (classDef != null)
        {
            ClassHitDie = classDef.HitDie;
            if (IsFirstLevel)
            {
                HpGained = ClassHitDie;
            }
        }
        UpdateFavoredClassStatus();
        Validate();
        UpdateGainedFeaturesForDraft();
    }

    private void UpdateFavoredClassStatus()
    {
        if (string.IsNullOrWhiteSpace(ClassName))
        {
            IsFavoredClass = false;
            AvailableRacialFcbDescription = null;
            FcbChoice = FavoredClassBonus.None;
            return;
        }

        IsFavoredClass = FavoredClassBonusEngine.IsFavoredClass(
            ClassName,
            CharacterLevel,
            _stateService.ActiveSheet?.ClassLevels);

        // 2. See if there is an alternate racial FCB available
        AvailableRacialFcbDescription = null;
        if (IsFavoredClass)
        {
            var classDef = _classLibrary.GetClassDefinition(ClassName);
            var raceName = _stateService.ActiveSheet?.Race?.Name;

            AvailableRacialFcbDescription = FavoredClassBonusEngine.GetAlternateRacialFcbDescription(classDef, raceName);
        }

        // 3. Reset the choice if they switch away from a favored class
        if (!IsFavoredClass)
        {
            FcbChoice = FavoredClassBonus.None;
        }
        // Auto-select HP by default if it is a favored class to prevent invalid blank states
        else if (FcbChoice == FavoredClassBonus.None)
        {
            FcbChoice = FavoredClassBonus.HitPoint;
        }
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

    // TODO: move these rules to the domain
    public void Validate()
    {
        if (IsReadOnly || _engine == null) return;

        var choice = ToChoice();
        ValidationErrors = _engine.ValidateChoice(choice);

        // HP Rules
        if (IsFirstLevel && HpGained != ClassHitDie)
            ValidationErrors.Add("For the first level, the HP must be the max possible");

        if (!IsFirstLevel && (HpGained < 1 || HpGained > ClassHitDie))
            ValidationErrors.Add($"HP gained must be between 1 and {ClassHitDie}");

        // Enforce FCB rules
        if (IsFavoredClass && FcbChoice == FavoredClassBonus.None)
        {
            ValidationErrors.Add("You must select a Favored Class Bonus (+1 HP, +1 Skill Point, or Alternate).");
        }
        if (!IsFavoredClass && FcbChoice != FavoredClassBonus.None)
        {
            ValidationErrors.Add("You cannot select a Favored Class Bonus for a class that is not your Favored Class.");
        }

        OnPropertyChanged(nameof(IsValid));
        OnPropertyChanged(nameof(IsFavoredClass));
        OnPropertyChanged(nameof(AvailableRacialFcbDescription));
    }

    public ClassLevelChoice ToChoice()
    {
        return new ClassLevelChoice
        {
            CharacterLevel = CharacterLevel,
            ClassName = ClassName,
            AbilityScoreIncrease = AbilityScoreIncrease,
            SelectedFavoredClassBonus = FcbChoice,
            HpGained = HpGained
        };
    }
}
