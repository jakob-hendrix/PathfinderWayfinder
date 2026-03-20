using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Logic.Interfaces;

namespace Wayfinder.Core.Services
{
    public class PathfinderRulesEngine : IPathfinderRulesEngine
    {
        //public IClassLibrary ClassRegistry { get; }

        public IEquipmentManager EquipmentManager { get; }

        public IClassFactory ClassFactory { get; }

        public IItemFactory ItemFactory { get; }

        public IRaceFactory RaceFactory { get; }

        public IClassLevelEngine ClassLevelEngine { get; }

        public ISkillEngine SkillEngine { get; }

        public PathfinderRulesEngine(
            IEquipmentManager equipmentManager,
            IClassFactory classFactory,
            IItemFactory itemFactory,
            IRaceFactory raceFactory,
            IClassLevelEngine classLevelEngine,
            ISkillEngine skillEngine)
        {
            EquipmentManager = equipmentManager;
            ClassFactory = classFactory;
            ItemFactory = itemFactory;
            RaceFactory = raceFactory;
            ClassLevelEngine = classLevelEngine;
            SkillEngine = skillEngine;
        }
    }
}
