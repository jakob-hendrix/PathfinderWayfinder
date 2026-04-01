using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Items;

namespace Wayfinder.Core.Interfaces
{
    public interface IItemFactory
    {
        ItemInstance CreateItem(string templateId);
        public ItemInstance CreateCustomItem(BaseItem customStats);
        public ItemInstance RehydrateItem(ItemEntity savedEntity);
    }
}
