using Wayfinder.Core.DomainModels.Items;
using Wayfinder.Core.Enums;

namespace Wayfinder.Core.DomainModels.Characters
{
    /// <summary>
    /// This is the base character entity that will be saved/loaded from storage. This character holds
    /// the 'facts' of the character that the ChararacterSheet will use to derive bonuses from
    /// gear, stats gained from levels, etc
    /// </summary>
    public class CharacterEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        #region User Input Facts
        public string? Name { get; set; } = string.Empty;
        public string? Gender { get; set; }

        // This gives base speed, some ability bonuses, etc
        public string Race { get; set; }
        //public Race? Race { get; set; }

        public Alignment Alignment { get; set; }
        public string Diety { get; set; }
        public int Age { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; } // in inches
        public string PhysicalDescription { get; set; }
        public string Biography { get; set; }

        // Ability Scores
        public int BaseStrength { get; set; } = 10;
        public int BaseDexterity { get; set; } = 10;
        public int BaseConstitution { get; set; } = 10;
        public int BaseIntelligence { get; set; } = 10;
        public int BaseWisdom { get; set; } = 10;
        public int BaseCharisma { get; set; } = 10;
        #endregion

        #region Derived Facts From Data
        // Should Language be an enum? Probably not - allow for RP to add new ones. We can seed a list of languages
        // from data
        public List<string> Languages { get; set; }

        // Levels - includes HP, favored class bonuses, any choices such as Armor Training, etc
        public List<ClassLevel> ClassLevels { get; set; } = new();

        // Skills - skill points per level are derived from class levels, but the chosen ranks
        // are stored here
        public List<SkillRank> SkillRanks { get; set; } = new();
        // Equipment. Item instance will track things like charges/max charges
        public List<ItemInstance> Inventory { get; set; } = new();
        public Dictionary<EquipmentSlot, Guid> EquippedItems { get; set; } = new();
        public List<AttackLoadout> AttacksLoadouts { get; set; } = new();
        #endregion

        #region Current Status Facts
        // State - things like current wounds, toggled effects
        public int Wounds { get; set; }
        public int NonLethalDamage { get; set; }
        public int TemporaryHp { get; set; }
        #endregion
    }
}
