namespace Wayfinder.Core.DomainModels.Items
{
    /// <summary>
    /// This represents an instance of an item tracked by the character. It uses a BaseItem as
    /// a template for the item, but allows for enchancement, new names, etc
    /// </summary>
    public class ItemInstance
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TemplateId { get; set; }
        public string Name { get; set; } = string.Empty;

        // The "rulebook" data before changes are made due to the state of this instance
        // (ie enhancement, damage, etc)
        public BaseItem BaseStats { get; set; }

        // False if dropped or stored in a container that is itself not held
        public bool IsCarried { get; set; } = true;

        //If Guid.Empty, it's not in a container
        public Guid ContainerId { get; set; } = Guid.Empty;
    }
}
