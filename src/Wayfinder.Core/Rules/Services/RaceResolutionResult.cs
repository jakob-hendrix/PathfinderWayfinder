using Wayfinder.Core.Models.Characters;

namespace Wayfinder.Core.Rules.Services
{
    public class RaceResolutionResult
    {
        public Race? HydratedRace { get; set; }

        public List<RacialTrait> ActiveRacialTraits { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public bool IsValid => !Errors.Any();
    }
}
