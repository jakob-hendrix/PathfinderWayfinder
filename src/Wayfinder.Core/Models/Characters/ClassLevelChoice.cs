using Wayfinder.Core.Constants;

namespace Wayfinder.Core.Models.Characters;

public class ClassLevelChoice
{
    public int CharacterLevel { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int HpGained { get; set; }
    public FavoredClassBonus SelectedFavoredClassBonus { get; set; }
    public AbilityScore? AbilityScoreIncrease { get; set; }
}
