using Wayfinder.Core.Constants;
using Wayfinder.Core.Models.Characters;

namespace Wayfinder.Core.DataDefinitions;

public class ClassDefinition
{
    public string Name { get; set; } = string.Empty;
    public int HitDie { get; set; } = 8;
    public int SkillPointsPerLevel { get; set; } = 2;
    public BabProgressionRate BabRate { get; set; } = BabProgressionRate.Slow;
    public SaveProgressionRate FortitudeRate { get; set; } = SaveProgressionRate.Slow;
    public SaveProgressionRate ReflexRate { get; set; } = SaveProgressionRate.Slow;
    public SaveProgressionRate WillRate { get; set; } = SaveProgressionRate.Slow;

    public List<string> ClassSkills { get; set; } = new();

    // the key is the level number
    public Dictionary<int, LevelDefinition> Levels { get; set; } = new();
    public List<RacialFavoredClassBonus> RacialFcbOptions { get; set; } = new();
}
