using Wayfinder.Core.DataServices;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Interfaces;

namespace Wayfinder.Core.Services
{
    public class ClassFactory : IClassFactory
    {
        private readonly IClassLibrary _classLibrary;
        private readonly Dictionary<string, BaseCharacterClass> _classCache = new();

        public ClassFactory(IClassLibrary classLibrary)
        {
            _classLibrary = classLibrary;
        }

        public BaseCharacterClass GetClass(string className)
        {
            // Check the cache first
            if (_classCache.TryGetValue(className, out var cachedClass))
            {
                return cachedClass;
            }

            // Otherwise get definition from the library
            var classDefinition = _classLibrary.GetClassDefinition(className);

            // Convert into hydrated class
            var hydratedClass = new DynamicCharacterClass(classDefinition);

            // Cache it for future use
            _classCache[className] = hydratedClass;

            return hydratedClass;
        }
    }
}
