using Wayfinder.Core.DataDefinitions;
using YamlDotNet.Serialization;

namespace Wayfinder.Infrastructure.DTOs
{
    public class ClassYamlDto
    {
        public string Name { get; set; } = string.Empty;
        [YamlMember(Alias = "BAB")]
        public string BabRate { get; set; } = "Slow";
        [YamlMember(Alias = "HD")]
        public int HitDie { get; set; } = 8;
        [YamlMember(Alias = "SkillPoints")]
        public int SkillPointsPerLevel { get; set; } = 2;
        [YamlMember(Alias = "Fortitude")]
        public string FortitudeRate { get; set; } = "Slow";
        [YamlMember(Alias = "Reflex")]
        public string ReflexRate { get; set; } = "Slow";
        [YamlMember(Alias = "Will")]
        public string WillRate { get; set; } = "Slow";

        public Dictionary<int, LevelDefinition> Levels { get; set; } = new();
        [YamlMember(Alias = "RacialFCBs")]
        public List<RacialFavoredClassBonusDto> RacialFcbOptions { get; set; } = new();

    }
}
