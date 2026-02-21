namespace Wayfinder.Core.Domain.Models.Items
{
    public abstract class BaseItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public double Weight { get; set; }
        public int GoldValue { get; set; }

        // Most items are just carried
        public virtual bool IsEquippable => false;
    }
}
