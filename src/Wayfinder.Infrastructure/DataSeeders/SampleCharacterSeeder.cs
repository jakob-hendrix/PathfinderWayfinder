using Wayfinder.Core.Constants;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;

namespace Wayfinder.Infrastructure.DataSeeders
{
    public class SampleCharacterSeeder
    {
        private readonly IPathfinderDataLibrary _libraries;
        private readonly IPathfinderRulesEngine _engine;

        public SampleCharacterSeeder(IPathfinderRulesEngine engine, IPathfinderDataLibrary libraries)
        {
            _engine = engine;
            _libraries = libraries;
        }

        public CharacterEntity BuildSampleCharacter()
        {
            // TODO change this to use a character sheet's functions to construct the
            // character from the ground up, like a player might do, using seeded data
            var entity = new CharacterEntity
            {
                Id = Guid.NewGuid(),
                Name = "Sosuke Bosuke",
                Gender = "Male",
                RaceChoices = new RaceChoices
                {
                    RaceName = "Human",
                },
                BaseStrength = 16, // Bumped this up slightly so he can carry all this gear!
                BaseDexterity = 13,
                BaseConstitution = 15,
                BaseIntelligence = 10,
                BaseWisdom = 9,
                BaseCharisma = 10,
            };

            #region Class Levels

            entity.ClassLevelChoices.Add(new ClassLevelChoice
            {
                ClassName = "Fighter",
                CharacterLevel = 1,
                SelectedFavoredClassBonus = FavoredClassBonus.HitPoint,
                HpGained = 10,
            });

            entity.ClassLevelChoices.Add(new ClassLevelChoice
            {
                ClassName = "Fighter",
                CharacterLevel = 2,
                SelectedFavoredClassBonus = FavoredClassBonus.AlternateRacial,
                HpGained = 5
            });

            entity.ClassLevelChoices.Add(new ClassLevelChoice
            {
                ClassName = "Fighter",
                CharacterLevel = 3,
                SelectedFavoredClassBonus = FavoredClassBonus.HitPoint,
                HpGained = 5
            });

            #endregion

            #region Skill Choices

            entity.SkillRankChoices.Add(new SkillRankChoice
            {
                CharacterLevel = 1,
                SkillName = "Climb",
                Ranks = 1
            });

            #endregion

            #region Inventory

            // 1. Armor (Pre-equipped!)
            entity.Inventory.Add(new ItemEntity
            {
                Id = Guid.NewGuid(),
                TemplateId = "breastplate",
                Quantity = 1,
                State = ItemState.Equipped,
                EquippedSlot = EquipmentSlot.Armor
            });

            // 2. Shield
            entity.Inventory.Add(new ItemEntity
            {
                Id = Guid.NewGuid(),
                TemplateId = "heavy_steel_shield",
                Quantity = 1,
                State = ItemState.Carried
            });

            // 3. Weapons
            entity.Inventory.Add(new ItemEntity
            {
                Id = Guid.NewGuid(),
                TemplateId = "longsword",
                Quantity = 1,
                State = ItemState.Carried
            });

            entity.Inventory.Add(new ItemEntity
            {
                Id = Guid.NewGuid(),
                TemplateId = "bardiche",
                Quantity = 1,
                State = ItemState.Carried
            });

            entity.Inventory.Add(new ItemEntity
            {
                Id = Guid.NewGuid(),
                TemplateId = "shortbow",
                Quantity = 1,
                State = ItemState.Carried
            });

            // 4. Adventuring Gear
            entity.Inventory.Add(new ItemEntity
            {
                Id = Guid.NewGuid(),
                TemplateId = "backpack", // Explicit ID from the adventuring gear YAML
                Quantity = 1,
                State = ItemState.Carried
            });

            #endregion

            #region Attack Loadouts

            // Pre-configure a default combat loadout
            entity.AttacksLoadouts.Add(new AttackLoadout
            {
                Id = Guid.NewGuid(),
                Name = "Sword & Board",
                MainHandItemId = entity.Inventory.First(i => i.TemplateId == "longsword").Id,
                OffHandItemId = entity.Inventory.First(i => i.TemplateId == "heavy_steel_shield").Id,
                IsTwoHandingMainWeapon = false,
                IsActive = true
            });

            entity.AttacksLoadouts.Add(new AttackLoadout
            {
                Id = Guid.NewGuid(),
                Name = "Two-Handed Cleave",
                MainHandItemId = entity.Inventory.First(i => i.TemplateId == "bardiche").Id,
                OffHandItemId = null,
                IsTwoHandingMainWeapon = true,
                IsActive = false
            });

            #endregion

            return entity;
        }
    }
}
