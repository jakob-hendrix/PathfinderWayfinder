namespace Wayfinder.Core.Models.Results;

using Wayfinder.Core.DataDefinitions;

public class ItemMapperResult : Result
{
    public List<string> Warnings { get; } = new();
    public ItemDefinition? HydratedItem { get; set; }
}
