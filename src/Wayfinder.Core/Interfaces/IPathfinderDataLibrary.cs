namespace Wayfinder.Core.Interfaces
{
    public interface IPathfinderDataLibrary
    {
        IClassLibrary ClassLibrary { get; }
        IItemLibrary ItemLibrary { get; }
        IRaceLibrary RaceLibrary { get; }
    }
}
