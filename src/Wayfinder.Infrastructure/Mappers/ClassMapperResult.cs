namespace Wayfinder.Core.Models.Results;

using Wayfinder.Core.DataDefinitions;

public class ClassMapperResult : Result
{
    public List<string> Warnings { get; } = new();
    public ClassDefinition? HydratedClass { get; set; }
}
