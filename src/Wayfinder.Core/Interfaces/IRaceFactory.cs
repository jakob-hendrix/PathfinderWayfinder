using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Rules.Services;

namespace Wayfinder.Core.Interfaces
{
    public interface IRaceFactory
    {
        RaceResolutionResult BuildRace(RaceChoices choices);
    }
}
