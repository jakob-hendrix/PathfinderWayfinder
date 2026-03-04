namespace Wayfinder.UI.ViewModels;

using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.Core.Enums;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;

// Represents the 'draft' of a class level
public partial class ClassLevelDetailViewModel : ObservableObject
{
    private readonly IClassLevelEngine _engine;

    [ObservableProperty] private int _characterLevel;
    [ObservableProperty] private string _className = string.Empty;
    [ObservableProperty] private FavoredClassBonus _fcbChoice;
    [ObservableProperty] private AbilityScore? _abilityScoreIncrease;

    [ObservableProperty] private List<string> _validationErrors = new();

    public bool IsValid => !ValidationErrors.Any();
    public bool GrantsAbilityScoreIncrease => _engine.IsAbilityScoreIncreaseLevel(CharacterLevel);

    public ClassLevelDetailViewModel(IClassLevelEngine engine, int level)
    {
        _engine = engine;
        CharacterLevel = level;
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
}
