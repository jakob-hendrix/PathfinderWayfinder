using Wayfinder.Core.Models.Items;

namespace Wayfinder.Core.Interfaces
{
    public interface IItemFactory
    {
        ItemInstance CreateItem(string templateId);
    }
}
