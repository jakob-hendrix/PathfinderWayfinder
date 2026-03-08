using Wayfinder.Core.Enums;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Common;

namespace Wayfinder.Tests.Core.Fakes;

public class CharacterTestClass : BaseCharacterClass
{
    // Testing BAB
    public CharacterTestClass(string name, BabProgressionRate babRate)
    {
        Name = name;
        BabRate = babRate;
    }

    // Testing Saves
    public CharacterTestClass(string name, SaveProgressionRate fortRate, SaveProgressionRate willRate, SaveProgressionRate reflexRate)
    {
        Name = name;
        FortitudeRate = fortRate;
        WillRate = willRate;
        ReflexRate = reflexRate;
    }

    public override string Name { get; } = default;

    public override string Description { get; } = default;

    public override int HitDie { get; } = default;

    public override int SkillPointsPerLevel { get; } = default;

    public override BabProgressionRate BabRate { get; } = default;

    public override SaveProgressionRate FortitudeRate { get; } = default;

    public override SaveProgressionRate WillRate { get; } = default;

    public override SaveProgressionRate ReflexRate { get; } = default;

    public override void ApplyClassFeature(int level, CharacterSheet sheet, List<Bonus> bonuses)
    {

    }
}
