using Wayfinder.Core.DataDefinitions;

namespace Wayfinder.Core.Interfaces
{
    public interface IClassLibrary : IDataLibrary
    {
        public void Register(ClassDefinition classDefinition);
        public ClassDefinition GetClassDefinition(string className);
        IEnumerable<ClassDefinition>? GetAll();
    }
}
