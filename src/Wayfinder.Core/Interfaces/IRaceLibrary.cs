using Wayfinder.Core.Data.Definitions;

namespace Wayfinder.Core.Interfaces
{
    public interface IRaceLibrary : IDataLibrary
    {
        void Register(RaceDefinition race);
        RaceDefinition GetRaceDefinition(string raceName);
        IEnumerable<RaceDefinition> GetRaceDefinitions();
    }
}
