using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Enums;

namespace Wayfinder.Core.Models.Characters;

public class HydratedClassLevel
{
    public int CharacterLevel { get; set; }
    public int ClassLevel { get; set; }
    public ClassDefinition ClassDefinition { get; set; } = null!;
    public List<GrantedFeatSlot> GrantedFeatSlots { get; set; } = new();
    public bool GrantsAbilityScoreIncrease { get; set; }
    public int BaseSkillPointsGranted { get; set; }
    public bool IsFavoredClass { get; set; }
    public FavoredClassBonus AppliedFavoredClassBonus { get; set; }

    // TODO: class features granted

}
