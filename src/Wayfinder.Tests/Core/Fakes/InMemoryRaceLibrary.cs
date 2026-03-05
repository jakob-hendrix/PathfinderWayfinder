using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Interfaces;

namespace Wayfinder.Tests.Core.Fakes;

public class InMemoryRaceLibrary : IRaceLibrary
{
    private readonly Dictionary<string, RaceDefinition> _races = new();

    public void Clear() => _races.Clear();
    public RaceDefinition GetRaceDefinition(string raceName) => _races.TryGetValue(raceName, out var def) ? def : null;
    public IEnumerable<RaceDefinition> GetRaceDefinitions() => _races.Values;
    public void Register(RaceDefinition race) => _races[race.Name] = race;
}
