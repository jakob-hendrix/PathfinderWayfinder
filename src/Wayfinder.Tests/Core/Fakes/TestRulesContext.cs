using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Factories;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Rules.Engines;
using Wayfinder.Core.Services;

namespace Wayfinder.Tests.Core.Fakes;

public class TestRulesContext
{
    public InMemoryClassLibrary ClassLibrary { get; }
    public InMemoryRaceLibrary RaceLibrary { get; }
    public InMemoryItemLibrary ItemLibrary { get; }
    public IPathfinderRulesEngine Engine { get; }

    public TestRulesContext()
    {
        // 1. Initialize the fake data stores
        ClassLibrary = new InMemoryClassLibrary();
        RaceLibrary = new InMemoryRaceLibrary();
        ItemLibrary = new InMemoryItemLibrary();

        // 2. Seed standard baseline test data
        SeedStandardData();

        // 3. Instantiate the REAL engines
        var classEngine = new ClassLevelEngine(ClassLibrary);
        var classFactory = new ClassFactory(ClassLibrary);
        var itemFactory = new ItemFactory(ItemLibrary); // No item library needed for these tests
        var raceFactory = new RaceFactory(RaceLibrary);
        var equipmentManager = new EquipmentManager();

        // 4. Assemble the facade
        Engine = new PathfinderRulesEngine(
            equipmentManager,
            classFactory,
            itemFactory,
            raceFactory,
            classEngine);
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
    }
}
