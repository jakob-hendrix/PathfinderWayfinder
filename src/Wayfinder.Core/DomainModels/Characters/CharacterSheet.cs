using Wayfinder.Core.DomainModels.Items;
using Wayfinder.Core.Services;

namespace Wayfinder.Core.DomainModels.Characters
{
    /// <summary>
    /// Rich Domain Model that represents a character with state - meaning class levels, ineventory, etc
    /// </summary>
    public class CharacterSheet
    {
        private readonly IPathfinderRulesEngine _rulesEngine;

        public CharacterSheet(CharacterEntity baseCharacter, IPathfinderRulesEngine rulesEngine)
        {
            BaseCharacter = baseCharacter;
            _rulesEngine = rulesEngine;
        }

        public CharacterEntity BaseCharacter { get; }

        // Ability Scores
        public int Strength => CalculateAbilityScore(BaseCharacter.BaseStrength);
        public int Dexterity => CalculateAbilityScore(BaseCharacter.BaseDexterity);
        public int Constitution => CalculateAbilityScore(BaseCharacter.BaseConstitution);
        public int Intelligence => CalculateAbilityScore(BaseCharacter.BaseIntelligence);
        public int Wisdom => CalculateAbilityScore(BaseCharacter.BaseWisdom);
        public int Charisma => CalculateAbilityScore(BaseCharacter.BaseCharisma);

        // Return a list of hydrated items from the current state of the base character's inventory
        public List<ItemInstance> GetHydratedInventory()
        {
            return BaseCharacter.Inventory.Select(item =>
            {
                var instance = _rulesEngine.ItemFactory.CreateItem(item.TemplateId);

                // TODO: may need more work here to apply custom item
                // 'facts' to this instance. Like, custom name, enchantments, etc
                // That or allow the ItemFactory to take an ItemInstance and make a copy

                return instance;
            }).ToList();
        }

        public void ToggleEquip(Guid instanceId)
        {
            var item = BaseCharacter.Inventory.FirstOrDefault(i => i.Id == instanceId);
            if (item != null)
            {
                // TODO: implement equippgin items
                // item.IsEquipped = !item.IsEquipped;
            }
        }

        public void ToggleCarried(Guid instanceId)
        {
            var item = BaseCharacter.Inventory.FirstOrDefault(i => i.Id == instanceId);
            if (item != null)
            {
                item.IsCarried = !item.IsCarried;
            }
        }

        // Sheet Actions
        public void AddLevel(string className)
        {
            // TODO: implement adding a new level to ClassLevels
            // This will need to trigger a full recalc of the sheet
            // Will require validation (max levels, class exists, new class isn't archetype of old class, etc)
            // So this will need to be in a new factory class of some sort
        }

        public void Refresh()
        {
            // TODO: allows the UI to trigger a full rebuild - is this even necessary?
            // For now, do nothing
        }

        // Helper functions
        private int CalculateAbilityScore(int baseScore) => _rulesEngine.AbilityScoreCalculator.Calculate(baseScore, BaseCharacter.ClassLevels);
    }
}
