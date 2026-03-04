using Wayfinder.Core.Interfaces;

namespace Wayfinder.Core.Services
{
    public class PathfinderRulesEngine : IPathfinderRulesEngine
    {
        public IClassLibrary ClassRegistry { get; }

        public IEquipmentManager EquipmentManager { get; }

        public IClassFactory ClassFactory { get; }

        public IItemFactory ItemFactory { get; }

        public IRaceFactory RaceFactory { get; }

        public IClassLevelEngine ClassLevelEngine { get; }

        public PathfinderRulesEngine(
            IClassLibrary classRegistry,
            IEquipmentManager equipmentManager,
            IClassFactory classFactory,
            IItemFactory itemFactory,
            IRaceFactory raceFactory,
            IClassLevelEngine classLevelEngine)
        {
            ClassRegistry = classRegistry;
            EquipmentManager = equipmentManager;
            ClassFactory = classFactory;
            ItemFactory = itemFactory;
            RaceFactory = raceFactory;
            ClassLevelEngine = classLevelEngine;
        }
    }
}
