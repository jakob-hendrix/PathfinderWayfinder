namespace Wayfinder.UI.ViewModels;

using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wayfinder.App.Services;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;

public partial class ClassLevelsViewModel : ObservableObject
{
    private readonly CharacterStateService _stateService;
    private readonly IClassLevelEngine _engine; // To pass to the DetailVM
    private readonly IClassLibrary _classLib;
    [ObservableProperty] private ClassLevelDetailViewModel? _activeDraft;
    [ObservableProperty] private int? _viewingLevelIndex; // Allows looking back at old levels

    public IEnumerable<ClassDefinition> AvailableClasses => _classLib.GetAll();
    public IReadOnlyList<HydratedClassLevel> CurrentLevels => _stateService.ActiveSheet?.ClassLevels ?? new List<HydratedClassLevel>();

    public ClassLevelsViewModel(CharacterStateService stateService, IClassLevelEngine engine, IClassLibrary classLib)
    {
        _stateService = stateService;
        _engine = engine;
        _classLib = classLib;

        _stateService.StateChanged += () => OnPropertyChanged(nameof(CurrentLevels));
    }

    [RelayCommand]
    private void StartNewLevel()
    {
        int nextLevel = CurrentLevels.Count + 1;
        if (nextLevel > 20) return;  //TODO: move the max level check into the b

        ActiveDraft = new ClassLevelDetailViewModel(_stateService, _engine, _classLib, nextLevel);
        ActiveDraft.Validate(); // Prime the validation
        ViewingLevelIndex = null;
    }

    [RelayCommand]
    private void SaveDraft()
    {
        if (ActiveDraft != null && ActiveDraft.IsValid && _stateService.ActiveSheet != null)
        {
            _stateService.ActiveSheet.AddClassLevel(ActiveDraft.ToChoice());

            // Rebuild domain, clear draft, and point view back to the top of the stack
            _stateService.RefreshDomain();
            ActiveDraft = null;
        }
    }

    [RelayCommand]
    private void RemoveTopLevel()
    {
        if (_stateService.ActiveSheet != null)
        {
            _stateService.ActiveSheet.RemoveHighestClassLevel();
            _stateService.RefreshDomain();
            ActiveDraft = null;
            ViewingLevelIndex = null;
        }
    }

    public void ViewHistoricalLevel(int level)
    {
        ViewingLevelIndex = level;
        ActiveDraft = null; // Close the draft if we are just looking at history
    }
}
