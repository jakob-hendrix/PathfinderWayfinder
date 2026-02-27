using Wayfinder.Core.DomainModels.Characters.RaceModels;
using Wayfinder.Core.DomainModels.Items;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Rules.Calculators;

namespace Wayfinder.Core.DomainModels.Characters
{
    /// <summary>
    /// Rich Domain Model that represents a character with state - meaning class levels, ineventory, etc
    /// </summary>
    public class CharacterSheet
    {
        private readonly IPathfinderRulesEngine _rulesEngine;

        public CharacterSheet(CharacterEntity baseCharacterFacts, IPathfinderRulesEngine rulesEngine)
        {
            BaseCharacterFacts = baseCharacterFacts;
            _rulesEngine = rulesEngine;
        }

        public CharacterEntity BaseCharacterFacts { get; }

        public Race? Race { get; private set; }

        // Ability Scores
        public int Strength => CalculateAbilityScore(BaseCharacterFacts.BaseStrength);
        public int Dexterity => CalculateAbilityScore(BaseCharacterFacts.BaseDexterity);
        public int Constitution => CalculateAbilityScore(BaseCharacterFacts.BaseConstitution);
        public int Intelligence => CalculateAbilityScore(BaseCharacterFacts.BaseIntelligence);
        public int Wisdom => CalculateAbilityScore(BaseCharacterFacts.BaseWisdom);
        public int Charisma => CalculateAbilityScore(BaseCharacterFacts.BaseCharisma);

        // Get a hydrated Race instance
        public void RebuildRace()
        {
            // The Factory builds it, and the Domain stores it
            var result = _rulesEngine.RaceFactory.BuildRace(BaseCharacterFacts.RaceChoices);
            if (result.IsValid)
            {
                Race = result.HydratedRace;
            }
            else
            {
                // TODO: throw error?
                Race = null;
            }
        }

        // Return a list of hydrated items from the current state of the base character's inventory
        public List<ItemInstance> GetHydratedInventory()
        {
            return BaseCharacterFacts.Inventory.Select(item =>
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
            var item = BaseCharacterFacts.Inventory.FirstOrDefault(i => i.Id == instanceId);
            if (item != null)
            {
                // TODO: implement equippgin items
                // item.IsEquipped = !item.IsEquipped;
            }
        }

        public void ToggleCarried(Guid instanceId)
        {
            var item = BaseCharacterFacts.Inventory.FirstOrDefault(i => i.Id == instanceId);
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
            RebuildRace();
        }

        // Helper functions
        private int CalculateAbilityScore(int baseScore) => AbilityScoreCalculator.CalculateCurrentValue(baseScore, BaseCharacterFacts.ClassLevels);
    }
}
