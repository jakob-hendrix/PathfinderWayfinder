
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
            // TODO change this to use a character sheet's functions to contruct the
            // character from the ground up, like a player might do, using seeded data
            var entity = new CharacterEntity
            {
                Name = "Sosuke Bosuke",
                Gender = "Male",
                RaceChoices = new RaceChoices
                {
                    RaceName = "Human"
                },
                ClassLevelChoices = new List<ClassLevelChoice>
                {
                    new ClassLevelChoice
                    {
                        ClassName = "Fighter",
                        CharacterLevel = 1,
                        SelectedFavoredClassBonus = FavoredClassBonus.HitPoint,
                        HpGained = 10,
                    },
                    new ClassLevelChoice
                    {
                        ClassName = "Fighter",
                        CharacterLevel = 2,
                        SelectedFavoredClassBonus = FavoredClassBonus.AlternateRacial,
                        HpGained = 5
                    },
                },

                BaseStrength = 10,
                BaseDexterity = 13,
                BaseConstitution = 15,
                BaseIntelligence = 5,
                BaseWisdom = 9,
                BaseCharisma = 19,
            };

            entity.SkillRankChoices.Add(new SkillRankChoice
            {
                CharacterLevel = 1,
                SkillName = "Climb",
                Ranks = 1
            });

            // Build inventory
            entity.Inventory.Add(_engine.ItemFactory.CreateItem("chain_shirt"));

            return entity;
        }
    }
}
