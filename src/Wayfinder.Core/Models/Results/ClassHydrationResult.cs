using Wayfinder.Core.Models.Characters;

namespace Wayfinder.Core.Models.Results;

public class ClassHydrationResult : Result
{
    public List<HydratedClassLevel> HydratedLevels { get; set; } = new();
}
