using Wayfinder.Core.Constants;

namespace Wayfinder.Core.Models.Items;

public abstract class BaseItem
{
    public string Name { get; set; }
    public double Weight { get; set; }
    public double Cost { get; set; }
    public abstract ItemType Type { get; }

    // By default, items have no equip slot (like Adventuring Gear)
    public virtual EquipmentSlot? Slot => null;
}
