using Wayfinder.Core.Data.Definitions;
using Wayfinder.Core.Extensions;

namespace Wayfinder.Core.Services
{
    public interface IItemLibrary : IDataLibrary
    {
        public void Register(ItemDefinition itemDefinition);
        public ItemDefinition GetItemDefinition(string id);
    }

    public class ItemLibrary : IItemLibrary
    {
        private readonly Dictionary<string, ItemDefinition> _itemDefinitions = new();

        public void Clear() => _itemDefinitions.Clear();

        public ItemDefinition GetItemDefinition(string id)
        {
            if (_itemDefinitions.TryGetValue(id, out var itemDefinition))
            {
                return itemDefinition;
            }
            throw new KeyNotFoundException($"Item definition with ID '{id}' not found in item library.");
        }

        public void Register(ItemDefinition itemDefinition)
        {
            if (itemDefinition.Id == null && itemDefinition.Name == null)
            {
                throw new ArgumentException("Item definition must have either an ID or a Name.");
            }

            var finalId = string.IsNullOrEmpty(itemDefinition.Id)
                ? itemDefinition.Name.GenerateIdFromName()
                : itemDefinition.Id;

            _itemDefinitions[finalId] = itemDefinition;
        }
    }
}
