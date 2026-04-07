using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Interfaces;

namespace Wayfinder.Tests.Core.Fakes;

public class InMemoryItemLibrary : IItemLibrary
{
    private Dictionary<string, ItemDefinition> _items = new();
    public void Clear() => _items.Clear();
    public List<ItemDefinition> GetAllDefinitions() => _items.Values.ToList();
    public ItemDefinition GetItemDefinition(string id) => _items.TryGetValue(id, out var def) ? def : null;
    public void Register(ItemDefinition itemDefinition) => _items[itemDefinition.Id] = itemDefinition;
}
