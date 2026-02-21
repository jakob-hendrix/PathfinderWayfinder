using Wayfinder.Core.DataServices;
using Wayfinder.Core.Rules.Services;
using Wayfinder.Tests.Core;

namespace Wayfinder.Core.Services
{
    public interface IPathfinderRulesEngine
    {
        IClassRegistry ClassRegistry { get; }
        IStatCalculator StatCalculator { get; }
        IBabCalculator BabCalculator { get; }
        ISaveCalculator SaveCalculator { get; }
        IAbilityScoreCalculator AbilityScoreCalculator { get; }

    }

    public class PathfinderRulesEngine : IPathfinderRulesEngine
    {
        public IStatCalculator StatCalculator { get; }

        public IBabCalculator BabCalculator { get; }

        public ISaveCalculator SaveCalculator { get; }

        public IClassRegistry ClassRegistry { get; }
        public IAbilityScoreCalculator AbilityScoreCalculator { get; }

        public PathfinderRulesEngine(
            IStatCalculator statCalculator,
            IBabCalculator babCalculator,
            ISaveCalculator saveCalculator,
            IClassRegistry classRegistry,
            IAbilityScoreCalculator abilityScoreCalculator)
        {
            StatCalculator = statCalculator;
            BabCalculator = babCalculator;
            SaveCalculator = saveCalculator;
            ClassRegistry = classRegistry;
            AbilityScoreCalculator = abilityScoreCalculator;
        }
    }
}
