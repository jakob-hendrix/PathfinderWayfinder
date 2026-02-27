namespace Wayfinder.Core.DataDefinitions;

public class ClassDefinition
{
    public string Name { get; set; } = string.Empty;
    public string BabRate { get; set; } = "Slow";
    public int HitDie { get; set; } = 8;
    public int SkillPointsPerLevel { get; set; } = 2;
    public string FortitudeRate { get; set; } = "Slow";
    public string ReflexRate { get; set; } = "Slow";
    public string WillRate { get; set; } = "Slow";

    // the key is the level number
    public Dictionary<int, LevelDefinition> Levels { get; set; } = new();
}
