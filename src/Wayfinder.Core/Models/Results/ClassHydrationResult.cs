using Wayfinder.Core.Models.Characters;

namespace Wayfinder.Core.Models.Results;

public class ClassHydrationResult
{
    public List<HydratedClassLevel> HydratedLevels { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public bool IsValid => Errors.Count == 0;
}
