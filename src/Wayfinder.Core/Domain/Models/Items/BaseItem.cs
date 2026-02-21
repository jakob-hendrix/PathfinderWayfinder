using Wayfinder.Core.Domain.Constants;

namespace Wayfinder.Core.Domain.Models.Items
{
    public abstract class BaseItem
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public int GoldValue { get; set; }
        public abstract ItemType Type { get; }

        // Most items are just carried
        public virtual bool IsEquippable => false;
    }
}
