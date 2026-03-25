namespace Wayfinder.Core.Logic.Features;

using System;
using System.Collections.Generic;
using System.Linq;
using Wayfinder.Core.Logic.Interfaces;

public class ClassFeatureRegistry : IClassFeatureRegistry
{
    private readonly Dictionary<string, IFeatureBehavior> _behaviors;

    // Your Dependency Injection container will automatically find all classes 
    // that implement IFeatureBehavior and pass them in here as a list!
    public ClassFeatureRegistry(IEnumerable<IFeatureBehavior> allBehaviors)
    {
        // Creates a highly optimized, case-insensitive lookup dictionary
        _behaviors = allBehaviors.ToDictionary(b => b.FeatureName, StringComparer.OrdinalIgnoreCase);
    }

    public bool TryGetBehavior(string featureName, out IFeatureBehavior behavior)
    {
        return _behaviors.TryGetValue(featureName, out behavior);
    }
}
