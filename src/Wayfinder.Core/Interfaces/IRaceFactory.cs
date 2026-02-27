using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Results;

namespace Wayfinder.Core.Interfaces
{
    public interface IRaceFactory
    {
        RaceResolutionResult BuildRace(RaceChoices choices);
    }
}
