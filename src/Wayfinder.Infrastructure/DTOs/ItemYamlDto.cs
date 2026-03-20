using YamlDotNet.Serialization;

namespace Wayfinder.Infrastructure.DTOs
{
    public class ItemYamlDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Weight { get; set; }
        public int Cost { get; set; }
        [YamlMember(Alias = "Type")]
        public string ItemType { get; set; }
        public string Description { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;

        public Dictionary<string, string> Properties { get; set; } = new();
    }
}
