using Wayfinder.Core.DataServices;

namespace Wayfinder.Core.Services
{
    public interface IPathfinderDataLibrary
    {
        IClassLibrary ClassLibrary { get; }
        IItemLibrary ItemLibrary { get; }
    }

    public class PathfinderDataLibrary : IPathfinderDataLibrary
    {
        public IClassLibrary ClassLibrary { get; }

        public IItemLibrary ItemLibrary { get; }

        public PathfinderDataLibrary(IClassLibrary classLibrary, IItemLibrary itemLibrary)
        {
            ClassLibrary = classLibrary;
            ItemLibrary = itemLibrary;
        }
    }
}
