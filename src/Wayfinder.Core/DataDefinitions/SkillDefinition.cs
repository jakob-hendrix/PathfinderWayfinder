using Wayfinder.Core.Constants;

namespace Wayfinder.Core.DataDefinitions;

public class SkillDefinition
{
    public string Name { get; set; } = string.Empty;
    public AbilityScore DefaultAbility { get; set; }
    public bool IsTrainedOnly { get; set; }
    public bool IsBackground { get; set; }
}
