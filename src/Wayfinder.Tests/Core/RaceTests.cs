using NUnit.Framework;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Factories;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Rules.Services;
using Wayfinder.Core.Services;

namespace Wayfinder.Tests.Core
{
    [TestFixture]
    public class RaceTests
    {
        private RaceLibrary _raceLibrary;
        private RaceFactory _raceFactory;

        [SetUp]
        public void Setup()
        {
            // Build a race definition
            _raceLibrary = new RaceLibrary();

            _raceLibrary.Register(new RaceDefinition
            {
                Name = "Human",
                CreatureType = "Humanoid",
                DefaultRacialTraits = new List<RacialTrait>
                {
                    new RacialTrait { Name = "Skilled" },
                    new RacialTrait { Name = "Bonus Feat" }
                },
                AlternativeRacialTraits = new List<AlternativeRacialTrait>
                {
                    new AlternativeRacialTrait
                    {
                        Name = "Heart of the Fields",
                        ReplacesRacialTraits = [ "Skilled" ]
                    },
                    new AlternativeRacialTrait
                    {
                        Name = "Focused Study",
                        ReplacesRacialTraits = [ "Bonus Feat" ]
                    }
                },
                Subraces = new List<Subrace>()
                {
                    new Subrace()
                    {
                        Name = "Country Folk",
                        RacialTraits = new List<AlternativeRacialTrait>
                        {
                            new AlternativeRacialTrait()
                            {
                                Name = "Heart of the Fields"
                            },
                            new AlternativeRacialTrait()
                            {
                                Name = "Focused Study"
                            }
                        }
                    }
                },
            });

            _raceFactory = new RaceFactory(_raceLibrary);
        }

        #region Library
        // TODO
        #endregion

        #region Factory
        [Test]
        public void Factory_BuildsDefaultHumanRace_WithDefaultTraits()
        {
            var def = _raceLibrary.GetRaceDefinition("Human");
            var choices = new RaceChoices { RaceName = "Human" };
            var raceResult = _raceFactory.BuildRace(choices);
            var race = raceResult.HydratedRace;

            Assert.That(race.SelectedRacialTraits, Is.EquivalentTo(def.DefaultRacialTraits));
        }
        #endregion

        #region RacialTraitEngine
        [Test]
        public void GetAvailableAlternatives_WhenPrerequisitesAreMet_ReturnsAlternativeTrait()
        {
            // Arrange
            var defaultTrait = new RacialTrait { Name = "Bonus Feat" };
            var altTrait = new AlternativeRacialTrait
            {
                Name = "Focused Study",
                ReplacesRacialTraits = new List<string> { "Bonus Feat" }
            };

            var baseRace = new RaceDefinition
            {
                Name = "Human",
                DefaultRacialTraits = new List<RacialTrait> { defaultTrait },
                AlternativeRacialTraits = new List<AlternativeRacialTrait> { altTrait }
            };

            var selectedAlts = new List<AlternativeRacialTrait>();

            // Act
            var available = RacialTraitEngine.GetAvailableAlternatives(baseRace, null, selectedAlts);

            // Assert
            Assert.That(available, Has.Count.EqualTo(1), "The engine incorrectly filtered out an available alternative trait.");
            Assert.That(available.First().Name, Is.EqualTo("Focused Study"));
        }

        [Test]
        public void ApplySubrace_WhenValid_ReplacesDefaultRacialTraits()
        {
            // Arrange
            var activeTraits = new Dictionary<string, RacialTrait>
        {
            { "Bonus Feat", new RacialTrait { Name = "Bonus Feat" } },
            { "Skilled", new RacialTrait { Name = "Skilled" } }
        };

            var subraceAltTrait = new AlternativeRacialTrait
            {
                Name = "Country Folk",
                ReplacesRacialTraits = new List<string> { "Bonus Feat" }
            };

            var subrace = new Subrace
            {
                Name = "Rural Human",
                RacialTraits = new List<AlternativeRacialTrait> { subraceAltTrait }
            };

            var result = new RaceResolutionResult();

            // Act
            RacialTraitEngine.ApplySubrace(activeTraits, subrace, result);

            // Assert
            Assert.That(result.IsValid, Is.True, "Applying a valid subrace should not generate errors.");
            Assert.That(activeTraits.ContainsKey("Bonus Feat"), Is.False, "The replaced trait should be removed.");
            Assert.That(activeTraits.ContainsKey("Skilled"), Is.True, "Unrelated traits should remain.");
            Assert.That(activeTraits.ContainsKey("Country Folk"), Is.True, "The new subrace trait should be added.");
        }

        [Test]
        public void ApplySubrace_WhenMissingPrerequisite_GeneratesError()
        {
            // Arrange
            var activeTraits = new Dictionary<string, RacialTrait>
        {
            // Missing "Bonus Feat" here
            { "Skilled", new RacialTrait { Name = "Skilled" } }
        };

            var subraceAltTrait = new AlternativeRacialTrait
            {
                Name = "Country Folk",
                ReplacesRacialTraits = new List<string> { "Bonus Feat" }
            };

            var subrace = new Subrace
            {
                Name = "Rural Human",
                RacialTraits = new List<AlternativeRacialTrait> { subraceAltTrait }
            };

            var result = new RaceResolutionResult();

            // Act
            RacialTraitEngine.ApplySubrace(activeTraits, subrace, result);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors[0], Does.Contain("requires replacing 'Bonus Feat'"));
            Assert.That(activeTraits.ContainsKey("Country Folk"), Is.False, "Trait should not be added if prereqs fail.");
        }
        #endregion
    }
}
