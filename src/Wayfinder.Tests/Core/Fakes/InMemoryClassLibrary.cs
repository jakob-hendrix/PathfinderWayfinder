using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Interfaces;

namespace Wayfinder.Tests.Core.Fakes;

public class InMemoryClassLibrary : IClassLibrary
{
    private readonly Dictionary<string, ClassDefinition> _classes = new();

    public void Clear() => _classes.Clear();

    public IEnumerable<ClassDefinition>? GetAll() => _classes.Values;
    public ClassDefinition? GetClassDefinition(string className) =>
        _classes.TryGetValue(className, out var def) ? def : null;

    public void Register(ClassDefinition classDefinition) => _classes[classDefinition.Name] = classDefinition;
}
