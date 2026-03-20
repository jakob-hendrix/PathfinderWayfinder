namespace Wayfinder.Core.DataDefinitions;

public class ItemDefinition
{
    // This is to prevent name collision. An id can be provided explicitly in the YAML,
    // but if not we'll generate one based on the name
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Weight { get; set; }
    public int Cost { get; set; }
    public string ItemType { get; set; }
    public string Description { get; set; } = string.Empty;
    public string URL { get; set; } = string.Empty;

    public Dictionary<string, string> Properties { get; set; } = new();
}
