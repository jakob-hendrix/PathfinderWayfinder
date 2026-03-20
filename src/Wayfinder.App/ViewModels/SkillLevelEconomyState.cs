// Represents the "Currency" and Validation state for a single column (Level)
using CommunityToolkit.Mvvm.ComponentModel;

public partial class SkillLevelEconomyState : ObservableObject
{
    public int Level { get; }
    public int MaxStandardPoints { get; }
    public int MaxBackgroundPoints { get; }

    [ObservableProperty] private int _spentStandardPoints;
    [ObservableProperty] private int _spentBackgroundPoints;
    [ObservableProperty] private bool _isValid = true;
    [ObservableProperty] private List<string> _errors = new();

    public SkillLevelEconomyState(int level, int maxStandard, int maxBackground)
    {
        Level = level;
        MaxStandardPoints = maxStandard;
        MaxBackgroundPoints = maxBackground;
    }
}
