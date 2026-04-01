using Wayfinder.Core.DataDefinitions;

namespace Wayfinder.Core.Interfaces
{
    public interface IItemLibrary : IDataLibrary
    {
        public void Register(ItemDefinition itemDefinition);
        public ItemDefinition GetItemDefinition(string id);
        public List<ItemDefinition> GetAllDefinitions();
    }
}
