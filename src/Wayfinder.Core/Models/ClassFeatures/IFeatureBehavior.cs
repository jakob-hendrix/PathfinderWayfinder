namespace Wayfinder.Core.Logic.Features;

using System.Collections.Generic;

public interface IFeatureBehavior
{
    // The string that matches your YAML file (e.g., "Bravery")
    string FeatureName { get; }

    // The method that turns the feature into math
    IEnumerable<ActiveEffect> GenerateEffects(int rank);
}
