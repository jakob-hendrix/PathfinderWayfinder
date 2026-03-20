using Wayfinder.Core.Models.Characters;

namespace Wayfinder.Core.Models.Results
{
    public class RaceResolutionResult : Result
    {
        public Race? HydratedRace { get; set; }

        public List<RacialTrait> ActiveRacialTraits { get; set; } = new();
    }
}
