using Wayfinder.Core.DataServices;
using Wayfinder.Core.Domain.Models;

namespace Wayfinder.Tests.Core.Mocks
{
    public class MockClassRegistry : IClassRegistry
    {
        public Dictionary<string, BaseCharacterClass> Classes { get; } = new();
        public BaseCharacterClass GetClass(string className) => Classes[className];
    }
}
