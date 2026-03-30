using Microsoft.Extensions.DependencyInjection;
using Wayfinder.Core.Configuration;
using Wayfinder.Core.Data;
using Wayfinder.Core.Data.Interfaces;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Factories;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Logic.Interfaces;
using Wayfinder.Core.Rules.Engines;
using Wayfinder.Core.Services;
// ... your other using statements ...

namespace Wayfinder.Tests.Core.Fakes;

public class TestRulesContext
{
    // The Master DI Container for this specific test run
    public ServiceProvider Provider { get; }

    // Existing tests will continue to use these exactly as before!
    // They now dynamically resolve from the DI container.
    public InMemoryClassLibrary ClassLibrary => Provider.GetRequiredService<InMemoryClassLibrary>();
    public InMemoryRaceLibrary RaceLibrary => Provider.GetRequiredService<InMemoryRaceLibrary>();
    public InMemoryItemLibrary ItemLibrary => Provider.GetRequiredService<InMemoryItemLibrary>();
    public ISkillLibrary SkillLibrary => Provider.GetRequiredService<ISkillLibrary>();
    public IClassFeatureRegistry ClassFeatureRegistry => Provider.GetRequiredService<IClassFeatureRegistry>();
    public IPathfinderRulesEngine Engine => Provider.GetRequiredService<IPathfinderRulesEngine>();

    public TestRulesContext()
    {
        var services = new ServiceCollection();

        // 1. ADD THE PATHFINDER CORE (This runs the Reflection magic to find all Class Features!)
        services.AddClassFeatures();

        // 2. REGISTER FAKE DATA STORES (Singletons for the lifespan of this test)
        // We register both the concrete InMemory class (for our test assertions) 
        // AND map it to the interface so the real engines know how to use it.
        services.AddSingleton<InMemoryClassLibrary>();
        services.AddSingleton<IClassLibrary>(x => x.GetRequiredService<InMemoryClassLibrary>());

        services.AddSingleton<InMemoryRaceLibrary>();
        services.AddSingleton<IRaceLibrary>(x => x.GetRequiredService<InMemoryRaceLibrary>());
        services.AddSingleton<InMemoryItemLibrary>();
        services.AddSingleton<IItemLibrary>(x => x.GetRequiredService<InMemoryItemLibrary>());
        services.AddSingleton<ISkillLibrary, SkillLibrary>();
        services.AddTransient<IClassLevelEngine, ClassLevelEngine>();
        services.AddTransient<IClassFactory, ClassFactory>();
        services.AddTransient<IItemFactory, ItemFactory>();
        services.AddTransient<IRaceFactory, RaceFactory>();
        services.AddTransient<ISkillEngine, SkillEngine>();
        services.AddTransient<IPathfinderRulesEngine, PathfinderRulesEngine>();

        Provider = services.BuildServiceProvider();

        SeedStandardData();
    }

    private void SeedStandardData()
    {
        // Add a generic Fighter class for leveling tests
        ClassLibrary.Register(new ClassDefinition
        {
            Name = "Fighter",
            SkillPointsPerLevel = 2
        });

        // Add a generic Human race so RaceFactory doesn't choke on initialization
        RaceLibrary.Register(new RaceDefinition
        {
            Name = "Human"
        });

        // Set default skill
        SkillLibrary.Seed(StandardSkills.GetCoreSkills());
    }
}
