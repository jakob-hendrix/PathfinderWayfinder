using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Enums;

namespace Wayfinder.Core.Models.Characters;

public class DynamicCharacterClass : BaseCharacterClass
{
    private readonly ClassDefinition _definition;

    public DynamicCharacterClass(ClassDefinition definition)
    {
        _definition = definition;
        Name = definition.Name;
        Description = string.Empty; // TODO - add to definition
        HitDie = definition.HitDie;
        SkillPointsPerLevel = definition.SkillPointsPerLevel;
        BabRate = definition.BabRate;
        FortitudeRate = definition.FortitudeRate;
        WillRate = definition.WillRate;
        ReflexRate = definition.ReflexRate;
    }

    public override string Name { get; }

    public override string Description { get; }

    public override int HitDie { get; }

    public override int SkillPointsPerLevel { get; }

    public override BabProgressionRate BabRate { get; }

    public override SaveProgressionRate FortitudeRate { get; }

    public override SaveProgressionRate WillRate { get; }

    public override SaveProgressionRate ReflexRate { get; }

    // This is where the character class gets the Class Feature template conferred
    // at this level
    public IEnumerable<ClassFeatureDefinition> GetClassFeaturesForLevel(int level)
    {
        if (_definition.Levels.TryGetValue(level, out var levelDefinition))
        {
            return levelDefinition.ClassFeatures;
        }
        return Enumerable.Empty<ClassFeatureDefinition>();
    }
}
