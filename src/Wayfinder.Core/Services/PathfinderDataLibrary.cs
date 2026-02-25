using Wayfinder.Core.DataServices;

namespace Wayfinder.Core.Services
{
    public interface IPathfinderDataLibrary
    {
        IClassLibrary ClassLibrary { get; }
        IItemLibrary ItemLibrary { get; }
        IRaceLibrary RaceLibrary { get; }
    }

    public class PathfinderDataLibrary : IPathfinderDataLibrary
    {
        public IClassLibrary ClassLibrary { get; }

        public IItemLibrary ItemLibrary { get; }

        public IRaceLibrary RaceLibrary { get; }

        public PathfinderDataLibrary(IClassLibrary classLibrary, IItemLibrary itemLibrary, IRaceLibrary raceLibrary)
        {
            ClassLibrary = classLibrary;
            ItemLibrary = itemLibrary;
            RaceLibrary = raceLibrary;
        }
    }
}
