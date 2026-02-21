using Wayfinder.Core.Domain.Constants;
using Wayfinder.Core.Domain.Models.Items;

namespace Wayfinder.Core.Domain.Models.Characters
{
    /// <summary>
    /// This is the base character entity that will be saved/loaded from storage.
    /// 
    /// The CharacterSheet will be the rich domain model that is used for all calculations and logic
    /// all of which is derived from the facts of this class
    /// </summary>
    public class CharacterEntity
    {
        public string? Name { get; set; } = string.Empty;
        public string? Gender { get; set; }

        public Race? Race { get; set; }

        // Classes
        public List<ClassLevel> ClassLevels { get; set; } = new();

        // Ability Scores
        public int BaseStrength { get; set; } = 10;
        public int BaseDexterity { get; set; } = 10;
        public int BaseConstitution { get; set; } = 10;
        public int BaseIntelligence { get; set; } = 10;
        public int BaseWisdom { get; set; } = 10;
        public int BaseCharisma { get; set; } = 10;

        // Equipment

        // The master list of all items linked to the character, equipped, carried, or stored
        public List<ItemInstance> Inventory { get; set; } = new();
        public Dictionary<EquipmentSlot, Guid> EquippedItems { get; set; } = new();

        // TODO: we will need to track weapons/shields seperately because we want the ability of the user
        // to define any number of main hand/off hand combos, since mid-combat switching is common and fast
    }
}
