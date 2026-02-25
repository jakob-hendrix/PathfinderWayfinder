using Wayfinder.Core.DomainModels.Characters.Race;

namespace Wayfinder.Core.Rules.Services
{
    public class RaceResolutionResult
    {
        public Race? HydratedRace { get; set; }
        public List<string> Errors { get; set; } = new();
        public bool IsValid => !Errors.Any();
    }
}
