using Wayfinder.Core.DataServices;
using Wayfinder.Core.Rules.Services;

namespace Wayfinder.Core.Services
{
    public interface IPathfinderRulesEngine
    {
        IClassLibrary ClassRegistry { get; }
        IStatCalculator StatCalculator { get; }
        IBabCalculator BabCalculator { get; }
        ISaveCalculator SaveCalculator { get; }
        //IAbilityScoreCalculator AbilityScoreCalculator { get; }
        IEquipmentManager EquipmentManager { get; }
        IClassFactory ClassFactory { get; }
        IItemFactory ItemFactory { get; }

    }

    public class PathfinderRulesEngine : IPathfinderRulesEngine
    {
        public IStatCalculator StatCalculator { get; }

        public IBabCalculator BabCalculator { get; }

        public ISaveCalculator SaveCalculator { get; }

        public IClassLibrary ClassRegistry { get; }
        //public IAbilityScoreCalculator AbilityScoreCalculator { get; }

        public IEquipmentManager EquipmentManager { get; }

        public IClassFactory ClassFactory { get; }

        public IItemFactory ItemFactory { get; }

        public PathfinderRulesEngine(
            IStatCalculator statCalculator,
            IBabCalculator babCalculator,
            ISaveCalculator saveCalculator,
            IClassLibrary classRegistry,
            //IAbilityScoreCalculator abilityScoreCalculator,
            IEquipmentManager equipmentManager,
            IClassFactory classFactory,
            IItemFactory itemFactory)
        {
            StatCalculator = statCalculator;
            BabCalculator = babCalculator;
            SaveCalculator = saveCalculator;
            ClassRegistry = classRegistry;
            //AbilityScoreCalculator = abilityScoreCalculator;
            EquipmentManager = equipmentManager;
            ClassFactory = classFactory;
            ItemFactory = itemFactory;
        }
    }
}
