using Wayfinder.Core.Enums;

namespace Wayfinder.Core.Models.Characters;

public class ClassLevelChoice
{
    public int CharacterLevel { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public FavoredClassBonus SelectedFavoredClassBonus { get; set; }
}
