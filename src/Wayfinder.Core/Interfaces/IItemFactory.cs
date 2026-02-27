using Wayfinder.Core.DomainModels.Items;

namespace Wayfinder.Core.Interfaces
{
    public interface IItemFactory
    {
        ItemInstance CreateItem(string templateId);
    }
}
