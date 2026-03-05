namespace Wayfinder.Core.Interfaces
{
    public interface IPathfinderRulesEngine
    {
        //IClassLibrary ClassRegistry { get; }
        IEquipmentManager EquipmentManager { get; }
        IClassFactory ClassFactory { get; }
        IItemFactory ItemFactory { get; }
        IRaceFactory RaceFactory { get; }
        IClassLevelEngine ClassLevelEngine { get; }
    }
}
