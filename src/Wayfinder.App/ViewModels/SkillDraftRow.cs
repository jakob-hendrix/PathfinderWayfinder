using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.Core.DomainModels.Skills;
using Wayfinder.Core.Interfaces;

public partial class SkillDraftRow : ObservableObject
{
    public CalculatedSkill SkillInfo { get; }
    public int[] RanksPerLevel { get; }

    public int DraftTotalRanks => RanksPerLevel.Sum();

    // Now it's just a dumb observable property!
    [ObservableProperty]
    private int _draftTotalBonus;

    public SkillDraftRow(CalculatedSkill skillInfo, int maxLevel)
    {
        SkillInfo = skillInfo;
        RanksPerLevel = new int[maxLevel + 1];
        DraftTotalBonus = skillInfo.TotalBonus; // Set initial state
    }

    // The UI model asks the Engine to do the math
    public void Recalculate(IPathfinderRulesEngine rules)
    {
        // Tell the UI the sum has changed
        OnPropertyChanged(nameof(DraftTotalRanks));

        // Ask the engine for the new official total
        DraftTotalBonus = rules.SkillEngine.CalculateProposedTotalBonus(SkillInfo, DraftTotalRanks);
    }
}
