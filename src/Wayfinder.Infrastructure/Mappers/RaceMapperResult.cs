using Wayfinder.Core.DataDefinitions;

namespace Wayfinder.Core.Models.Results;

public class RaceMapperResult : Result
{
    public List<string> Warnings { get; } = new();
    public RaceDefinition? HydratedRace { get; set; }
}
