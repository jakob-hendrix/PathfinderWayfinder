namespace Wayfinder.Core.Logic.Interfaces;

using Wayfinder.Core.Logic.Features;

public interface IClassFeatureRegistry
{
    // Tries to find the C# math behavior for a given data string
    bool TryGetBehavior(string featureName, out IFeatureBehavior behavior);
}
