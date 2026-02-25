using Wayfinder.Core.Data.Definitions;

namespace Wayfinder.Core.Services
{
    public interface IRaceLibrary : IDataLibrary
    {
        void Register(RaceDefinition race);
        RaceDefinition GetRaceDefinition(string raceName);
        IEnumerable<RaceDefinition> GetRaceDefinitions();
    }
}
