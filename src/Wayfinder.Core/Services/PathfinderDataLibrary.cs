using Wayfinder.Core.Data.Interfaces;
using Wayfinder.Core.Interfaces;

namespace Wayfinder.Core.Services
{
    public class PathfinderDataLibrary : IPathfinderDataLibrary
    {
        public IClassLibrary ClassLibrary { get; }

        public IItemLibrary ItemLibrary { get; }

        public IRaceLibrary RaceLibrary { get; }

        public ISkillLibrary SkillLibrary { get; }

        public PathfinderDataLibrary(
            IClassLibrary classLibrary,
            IItemLibrary itemLibrary,
            IRaceLibrary raceLibrary,
            ISkillLibrary skillLibrary)
        {
            ClassLibrary = classLibrary;
            ItemLibrary = itemLibrary;
            RaceLibrary = raceLibrary;
            SkillLibrary = skillLibrary;
        }
    }
}
