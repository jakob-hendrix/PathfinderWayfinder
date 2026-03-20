using Wayfinder.Core.DataDefinitions;

namespace Wayfinder.Core.Interfaces
{
    public interface IRaceLibrary : IDataLibrary
    {
        void Register(RaceDefinition race);
        RaceDefinition GetRaceDefinition(string raceName);
        IEnumerable<RaceDefinition> GetRaceDefinitions();
    }
}
