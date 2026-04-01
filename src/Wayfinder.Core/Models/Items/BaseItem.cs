using Wayfinder.Core.Constants;

namespace Wayfinder.Core.Models.Items;

public abstract class BaseItem
{
    public string Name { get; set; }
    public double Weight { get; set; }
    public double Cost { get; set; }
    public abstract ItemType Type { get; }

    // Most items are just carried
    public virtual bool IsEquippable => false;
}
