using Wayfinder.Core.DomainModels.Characters;
using Wayfinder.Core.Services;

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
                //Race = new Race
                //{
                //    // TODO: race factory
                //    Name = "Human"
                //},
                Race = "Human",
                ClassLevels = new List<ClassLevel>
                {
                    new ClassLevel
                    {
                        Class = _engine.ClassFactory.GetClass("Fighter"),
                        Level = 1
                    },
                    new ClassLevel
                    {
                        Class = _engine.ClassFactory.GetClass("Fighter"),
                        Level = 2
                    },
                },
                BaseStrength = 10,
                BaseDexterity = 13,
                BaseConstitution = 15,
                BaseIntelligence = 5,
                BaseWisdom = 9,
                BaseCharisma = 19
            };

            // Build inventory
            entity.Inventory.Add(_engine.ItemFactory.CreateItem("chain_shirt"));

            return entity;
        }
    }
}
