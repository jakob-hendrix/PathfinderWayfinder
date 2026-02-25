using YamlDotNet.Serialization;

namespace Wayfinder.Infrastructure.DTOs
{
    public class SubraceYamlDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Just a list of the alternative trait that should already be defined
        [YamlMember(Alias = "Traits")]
        public List<string> AlternativeTraitNames { get; set; } = new();
    }
}
