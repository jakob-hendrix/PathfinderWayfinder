using Wayfinder.Core.Enums;

namespace Wayfinder.Core.Models.Items;

public abstract class BaseItem
{
    public string Name { get; set; }
    public double Weight { get; set; }
    public int Cost { get; set; }
    public abstract ItemType Type { get; }

    // Most items are just carried
    public virtual bool IsEquippable => false;
}
