using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wayfinder.App.Services;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;

namespace Wayfinder.App.ViewModels;

public partial class SkillViewModel : ObservableObject, IDisposable
{
    private readonly CharacterStateService _stateService;
    private readonly IPathfinderRulesEngine _rules;

    public List<SkillDraftRow> Rows { get; private set; } = new();
    public Dictionary<int, SkillLevelEconomyState> Columns { get; private set; } = new();

    public int CurrentLevel => _stateService.ActiveSheet?.ClassLevels?.Count() ?? 0;
    public bool CanSave => Columns.Values.All(c => c.IsValid);

    public SkillViewModel(CharacterStateService stateService, IPathfinderRulesEngine rules)
    {
        _stateService = stateService;
        _rules = rules;
        _stateService.StateChanged += OnDomainStateChanged;
        InitializeDraft();
    }

    public void InitializeDraft()
    {
        var sheet = _stateService.ActiveSheet;
        if (sheet == null || sheet.ClassLevels == null || CurrentLevel == 0) return;

        Rows.Clear();
        Columns.Clear();

        // 1. Initialize the Columns (The Economy/Budget per level)
        foreach (var economy in sheet.SkillEconomy)
        {
            Columns[economy.Level] = new SkillLevelEconomyState(
                economy.Level,
                economy.MaxStandardPoints,
                economy.MaxBackgroundPoints);
        }

        // 2. Initialize the Rows (The Skills)
        foreach (var skill in sheet.Skills)
        {
            var row = new SkillDraftRow(skill, CurrentLevel);

            // Populate the draft with the historical facts from the Entity
            var historicalChoices = sheet.BaseCharacter.SkillRankChoices.Where(c => c.SkillName == skill.Name);
            foreach (var choice in historicalChoices)
            {
                if (choice.CharacterLevel <= CurrentLevel)
                {
                    row.RanksPerLevel[choice.CharacterLevel] = choice.Ranks;
                }
            }
            Rows.Add(row);
        }

        // 3. Run an initial validation on all levels to set the Spent Points text
        for (int i = 1; i <= CurrentLevel; i++)
        {
            ValidateLevel(i);
        }
    }

    private void OnDomainStateChanged()
    {
        InitializeDraft();

        // Explicitly tell the UI that the structure has completely changed
        OnPropertyChanged(nameof(CurrentLevel));
        OnPropertyChanged(nameof(Rows));
        OnPropertyChanged(nameof(Columns));
    }

    // Called by the UI whenever a number input changes
    public void OnRankChanged(int levelEdited)
    {
        // Tell the UI that row totals have changed
        foreach (var row in Rows)
        {
            row.Recalculate(_rules);
        }

        ValidateLevel(levelEdited);
        OnPropertyChanged(nameof(CanSave)); // Re-evaluate the Save button
    }

    private void ValidateLevel(int level)
    {
        var sheet = _stateService.ActiveSheet;
        if (sheet == null) return;

        // 1. Build the proposed choices for THIS level from the draft matrix
        var proposed = Rows
            .Where(r => r.RanksPerLevel[level] > 0)
            .Select(r => new SkillRankChoice { SkillName = r.SkillInfo.Name, CharacterLevel = level, Ranks = r.RanksPerLevel[level] })
            .ToList();

        // 2. Build the history (everything in the draft BEFORE this level)
        var history = new List<SkillRankChoice>();
        for (int l = 1; l < level; l++)
        {
            history.AddRange(Rows
                .Where(r => r.RanksPerLevel[l] > 0)
                .Select(r => new SkillRankChoice { SkillName = r.SkillInfo.Name, CharacterLevel = l, Ranks = r.RanksPerLevel[l] }));
        }

        // 3. Ask the Engine!
        var result = _rules.SkillEngine.ValidateSkillRanksForLevel(level, proposed, history, sheet.AvailableSkills);

        // 4. Update the Column State
        var column = Columns[level];
        column.SpentStandardPoints = result.StandardRanksSpent;
        column.SpentBackgroundPoints = result.BackgroundRanksSpent;

        column.Errors.Clear();
        column.Errors.AddRange(result.Errors);

        // UI Economy Validation (Checking the math the engine returned)
        if (column.SpentStandardPoints > column.MaxStandardPoints)
            column.Errors.Add($"Spent {column.SpentStandardPoints}/{column.MaxStandardPoints} Standard Points.");

        if (column.SpentBackgroundPoints > column.MaxBackgroundPoints)
            column.Errors.Add($"Spent {column.SpentBackgroundPoints}/{column.MaxBackgroundPoints} Background Points.");

        column.IsValid = !column.Errors.Any();
    }

    [RelayCommand]
    public void Save()
    {
        if (!CanSave || _stateService.ActiveSheet == null) return;

        // Flatten the matrix back into a 1D list of facts
        var finalChoices = new List<SkillRankChoice>();
        foreach (var row in Rows)
        {
            for (int l = 1; l <= CurrentLevel; l++)
            {
                if (row.RanksPerLevel[l] > 0)
                {
                    finalChoices.Add(new SkillRankChoice
                    {
                        SkillName = row.SkillInfo.Name,
                        CharacterLevel = l,
                        Ranks = row.RanksPerLevel[l]
                    });
                }
            }
        }

        _stateService.ActiveSheet.CommitSkillChoices(finalChoices);
    }

    public void Dispose() => _stateService.StateChanged -= OnDomainStateChanged;
}
