using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wayfinder.App.Services;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;

namespace Wayfinder.UI.ViewModels;

public partial class ClassLevelsViewModel : ObservableObject
{
    private readonly CharacterStateService _stateService;
    private readonly IClassLevelEngine _engine; // To pass to the DetailVM
    private readonly IClassLibrary _classLib;

    [ObservableProperty] private ClassLevelDetailViewModel? _activeDetail;

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
        if (nextLevel > 20) return;

        ActiveDetail = new ClassLevelDetailViewModel(_engine, _stateService, _classLib, nextLevel);
        ActiveDetail.Validate();
    }

    public void ViewHistoricalLevel(int level)
    {
        var pastLevel = CurrentLevels.FirstOrDefault(l => l.CharacterLevel == level);
        if (pastLevel != null)
        {
            ActiveDetail = new ClassLevelDetailViewModel(_stateService, _classLib, pastLevel);
        }
    }

    [RelayCommand]
    private void SaveDraft()
    {
        if (ActiveDetail != null && ActiveDetail.IsValid && _stateService.ActiveSheet != null)
        {
            _stateService.ActiveSheet.AddClassLevel(ActiveDetail.ToChoice());

            // Rebuild domain, clear draft, and point view back to the top of the stack
            _stateService.RefreshDomain();
            ActiveDetail = null;
        }
    }

    [RelayCommand]
    private void RemoveTopLevel()
    {
        if (_stateService.ActiveSheet != null)
        {
            _stateService.ActiveSheet.RemoveHighestClassLevel();
            _stateService.RefreshDomain();
            ActiveDetail = null;
        }
    }
}
