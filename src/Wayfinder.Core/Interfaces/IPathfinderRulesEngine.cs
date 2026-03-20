using Wayfinder.Core.Logic.Interfaces;

namespace Wayfinder.Core.Interfaces
{
    public interface IPathfinderRulesEngine
    {
        IEquipmentManager EquipmentManager { get; }
        IClassFactory ClassFactory { get; }
        IItemFactory ItemFactory { get; }
        IRaceFactory RaceFactory { get; }
        IClassLevelEngine ClassLevelEngine { get; }
        ISkillEngine SkillEngine { get; }
    }
}
