using Microsoft.Extensions.DependencyInjection;
using Wayfinder.Core.Logic.Features;
using Wayfinder.Core.Logic.Interfaces;

namespace Wayfinder.Core.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClassFeatures(this IServiceCollection services)
    {
        // 1. Register the master registry
        services.AddSingleton<IClassFeatureRegistry, ClassFeatureRegistry>();

        // 2. Get the assembly where your behaviors live 
        // (Assuming they are all in the same assembly as IFeatureBehavior)
        var targetAssembly = typeof(IFeatureBehavior).Assembly;
        var behaviorInterface = typeof(IFeatureBehavior);

        // 3. Use Reflection to find every class that implements the interface
        var featureBehaviors = targetAssembly.GetTypes()
            .Where(type => type.IsClass
                        && !type.IsAbstract
                        && behaviorInterface.IsAssignableFrom(type));

        // 4. Automatically loop through and register them all!
        foreach (var behaviorType in featureBehaviors)
        {
            // We use Singleton here because these classes just contain pure math/logic.
            // They don't hold any user state, so it's perfectly safe and highly performant
            // to just let the app share one instance of BraveryBehavior across the whole engine.
            services.AddSingleton(behaviorInterface, behaviorType);
        }

        return services;
    }
}
