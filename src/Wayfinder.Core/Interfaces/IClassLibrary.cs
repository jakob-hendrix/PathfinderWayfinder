using Wayfinder.Core.Data.Definitions;

namespace Wayfinder.Core.Interfaces
{
    public interface IClassLibrary : IDataLibrary
    {
        public void Register(ClassDefinition classDefinition);
        public ClassDefinition GetClassDefinition(string className);
    }
}
