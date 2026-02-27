using Wayfinder.Core.Data.Definitions;
using Wayfinder.Core.Interfaces;

namespace Wayfinder.Core.Services
{
    public class RaceLibrary : IRaceLibrary
    {
        private readonly Dictionary<string, RaceDefinition> _races = new(StringComparer.OrdinalIgnoreCase);

        public void Clear() => _races.Clear();

        public RaceDefinition GetRaceDefinition(string raceName)
        {
            if (_races.TryGetValue(raceName, out var raceDefinition))
            {
                return raceDefinition;
            }
            throw new KeyNotFoundException($"Race '{raceName}' not found in race library.");
        }

        public IEnumerable<RaceDefinition> GetRaceDefinitions() => _races.Values;

        public void Register(RaceDefinition race) => _races[race.Name] = race;
    }
}
