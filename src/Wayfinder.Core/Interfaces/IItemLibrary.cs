using Wayfinder.Core.Data.Definitions;

namespace Wayfinder.Core.Interfaces
{
    public interface IItemLibrary : IDataLibrary
    {
        public void Register(ItemDefinition itemDefinition);
        public ItemDefinition GetItemDefinition(string id);
    }
}
