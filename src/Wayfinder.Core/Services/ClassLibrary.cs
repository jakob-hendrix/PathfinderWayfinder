using Wayfinder.Core.Data.Definitions;
using Wayfinder.Core.Interfaces;

namespace Wayfinder.Core.DataServices
{
    public class ClassLibrary : IClassLibrary
    {
        private readonly Dictionary<string, ClassDefinition> _definitions = new();

        public void Clear() => _definitions.Clear();

        public ClassDefinition GetClassDefinition(string className)
        {
            if (_definitions.TryGetValue(className, out var classDefinition))
            {
                return classDefinition;
            }
            throw new KeyNotFoundException($"Class '{className}' not found in class library.");
        }

        public void Register(ClassDefinition classDefinition)
        {
            _definitions[classDefinition.Name] = classDefinition;
        }
    }
}
