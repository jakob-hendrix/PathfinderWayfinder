using YamlDotNet.Serialization;

namespace Wayfinder.Infrastructure.DTOs
{
    public class RacialTraitYamlDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [YamlMember(Alias = "Effects")]
        public List<EffectDto> GrantedEffects { get; init; } = new();
    }
}
