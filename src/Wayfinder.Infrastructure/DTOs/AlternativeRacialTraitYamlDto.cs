using YamlDotNet.Serialization;

namespace Wayfinder.Infrastructure.DTOs
{
    public class AlternativeRacialTraitYamlDto : RacialTraitYamlDto
    {
        [YamlMember(Alias = "Replaces")]
        public List<string> ReplacesTraitNames { get; set; } = new();
    }
}
