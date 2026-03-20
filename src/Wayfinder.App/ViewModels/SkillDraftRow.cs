using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.Core.DomainModels.Skills;

namespace Wayfinder.App.ViewModels;

// Represents a single row in the UI matrix
public partial class SkillDraftRow : ObservableObject
{
    public CalculatedSkill SkillInfo { get; }

    // The ranks drafted for each level (1-indexed for convenience, so index 0 is empty/ignored)
    public int[] RanksPerLevel { get; }

    // Dynamically calculates the total ranks based on the user's current draft
    public int DraftTotalRanks => RanksPerLevel.Sum();

    // Estimates the new total bonus (Base Bonus - Old Ranks + New Draft Ranks)
    public int DraftTotalBonus => SkillInfo.TotalBonus - SkillInfo.TotalRanks + DraftTotalRanks;

    public SkillDraftRow(CalculatedSkill skillInfo, int maxLevel)
    {
        SkillInfo = skillInfo;
        RanksPerLevel = new int[maxLevel + 1]; // +1 so Level 1 = Index 1
    }
}
