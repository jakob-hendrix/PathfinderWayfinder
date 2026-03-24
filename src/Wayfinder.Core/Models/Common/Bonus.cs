using Wayfinder.Core.Enums;

namespace Wayfinder.Core.Models.Common;

public class Bonus
{
    public ModifierType Type { get; set; }
    public int Value { get; set; } = 0;
    public string Source { get; set; } = "Unknown";

    //public Bonus(ModifierType type, int value, string source)
    //{
    //    Type = type;
    //    Value = value;
    //    Source = source;
    //}

    // add a bool to force stacking?
    public bool IsStackable
    {
        get
        {
            switch (this.Type)
            {
                case ModifierType.Circumstance or ModifierType.Dodge or ModifierType.Untyped:
                    return true;
                default:
                    return false;
            }
        }
    }
}
