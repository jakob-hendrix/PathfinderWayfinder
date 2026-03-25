using NUnit.Framework;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.DataServices;
using Wayfinder.Core.Extensions;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Logic.Features;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Rules.Engines;

namespace Wayfinder.Tests.Core;

public class ClassLevelEngineTests
{
    private ClassFeatureRegistry _classFeatures;
    private IClassLibrary _classLibrary;
    private ClassLevelEngine _engine;

    [SetUp]
    public void Setup()
    {
        _classLibrary = new ClassLibrary();
        _classLibrary.Register(new ClassDefinition
        {
            Name = "Fighter",
            HitDie = 10,
            SkillPointsPerLevel = 2
        });

        _engine = new ClassLevelEngine(_classLibrary, _classFeatures);
    }

    [Test]
    public void HydrateLevels_LevelOne_GrantsStandardCharacterFeat()
    {
        var choices = new List<ClassLevelChoice>
        {
            new ClassLevelChoice{ CharacterLevel = 1, ClassName = "Fighter" }
        };

        var result = _engine.HydrateLevels(choices);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.HydratedLevels[0].GrantedFeatSlots, Has.Count.EqualTo(1));
        Assert.That(result.HydratedLevels[0].GrantedFeatSlots[0].Source, Is.EqualTo("Character Level 1"));
    }

    [Test]
    public void HydrateLevels_LevelTwo_GrantsNoFeatsOrAbilityIncreases()
    {
        var choices = new List<ClassLevelChoice>
        {
            new ClassLevelChoice{ CharacterLevel = 1, ClassName = "Fighter" },
            new ClassLevelChoice{ CharacterLevel = 2, ClassName = "Fighter" }
        };

        var result = _engine.HydrateLevels(choices);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.HydratedLevels[1].GrantedFeatSlots, Is.Empty, "Even levels do not grant standard feats");
        Assert.That(result.HydratedLevels[1].GrantsAbilityScoreIncrease, Is.False, "Level 2 does not grant ability score increases");
    }

    [TestCase(1, false)]
    [TestCase(2, false)]
    [TestCase(3, false)]
    [TestCase(4, true)]
    [TestCase(5, false)]
    [TestCase(6, false)]
    [TestCase(7, false)]
    [TestCase(8, true)]
    [TestCase(19, false)]
    [TestCase(20, true)]
    public void HydrateLevels_ExpectedLevels_GrantAbilityScoreIncreases(int testLevel, bool expected)
    {
        var choices = new List<ClassLevelChoice>();
        for (int i = 0; i < testLevel; i++)
        {
            choices.Add(new ClassLevelChoice { CharacterLevel = i + 1, ClassName = "Fighter" });
        }

        var result = _engine.HydrateLevels(choices);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.HydratedLevels?.GetDataByLevel(testLevel).GrantsAbilityScoreIncrease, Is.EqualTo(expected));
    }
}
