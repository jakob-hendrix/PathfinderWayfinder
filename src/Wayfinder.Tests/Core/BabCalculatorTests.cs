using NUnit.Framework;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.DataServices;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Rules.Calculators;

namespace Wayfinder.Tests.Core
{
    [TestFixture]
    public class BabCalculatorTests
    {
        private IClassLibrary _classRegistry;

        [SetUp]
        public void Setup()
        {
            _classRegistry = new ClassLibrary();

            _classRegistry.Register(new ClassDefinition() { Name = "Fighter", BabRate = BabProgressionRate.Fast });
            _classRegistry.Register(new ClassDefinition() { Name = "Fighter2", BabRate = BabProgressionRate.Fast });
            _classRegistry.Register(new ClassDefinition() { Name = "Rogue", BabRate = BabProgressionRate.Medium });
            _classRegistry.Register(new ClassDefinition() { Name = "Rogue2", BabRate = BabProgressionRate.Medium });
            _classRegistry.Register(new ClassDefinition() { Name = "Wizard", BabRate = BabProgressionRate.Slow });
            _classRegistry.Register(new ClassDefinition() { Name = "Wizard2", BabRate = BabProgressionRate.Slow });
        }

        [Test]
        public void CalculateBab_ShouldReturn0_WhenNoLevels()
        {
            var levels = new List<HydratedClassLevel>();
            Assert.That(BabCalculator.Calculate(levels), Is.EqualTo(0));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        [TestCase(20, 20)]
        public void CalculateBab_FastRate_ShouldReturnCorrectBab(int levelCount, int expectedBab)
        {
            var levels = new List<HydratedClassLevel>();

            for (int i = 1; i <= levelCount; i++)
            {
                levels.Add(new HydratedClassLevel
                {
                    ClassDefinition = _classRegistry.GetClassDefinition("Fighter"),
                    ClassLevel = i
                });
            }

            Assert.That(BabCalculator.Calculate(levels), Is.EqualTo(expectedBab));
        }

        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(4, 3)]
        [TestCase(5, 3)]
        [TestCase(6, 4)]
        [TestCase(7, 5)]
        [TestCase(8, 6)]
        [TestCase(9, 6)]
        [TestCase(10, 7)]
        [TestCase(11, 8)]
        [TestCase(12, 9)]
        [TestCase(13, 9)]
        [TestCase(14, 10)]
        [TestCase(15, 11)]
        [TestCase(16, 12)]
        [TestCase(17, 12)]
        [TestCase(18, 13)]
        [TestCase(19, 14)]
        [TestCase(20, 15)]
        public void CalculateBab_MediumRate_ShouldReturnCorrectBab(int levelCount, int expectedBab)
        {
            var levels = new List<HydratedClassLevel>();

            for (int i = 1; i <= levelCount; i++)
            {
                levels.Add(new HydratedClassLevel
                {
                    ClassDefinition = _classRegistry.GetClassDefinition("Rogue"),
                    ClassLevel = i
                });
            }

            Assert.That(BabCalculator.Calculate(levels), Is.EqualTo(expectedBab));
        }

        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(4, 2)]
        [TestCase(5, 2)]
        [TestCase(6, 3)]
        [TestCase(7, 3)]
        [TestCase(8, 4)]
        [TestCase(9, 4)]
        [TestCase(10, 5)]
        [TestCase(11, 5)]
        [TestCase(12, 6)]
        [TestCase(13, 6)]
        [TestCase(14, 7)]
        [TestCase(15, 7)]
        [TestCase(16, 8)]
        [TestCase(17, 8)]
        [TestCase(18, 9)]
        [TestCase(19, 9)]
        [TestCase(20, 10)]
        public void CalculateBab_SlowRate_ShouldReturnCorrectBab(int levelCount, int expectedBab)
        {
            var levels = new List<HydratedClassLevel>();

            for (int i = 1; i <= levelCount; i++)
            {
                levels.Add(new HydratedClassLevel
                {
                    ClassDefinition = _classRegistry.GetClassDefinition("Wizard"),
                    ClassLevel = i
                });
            }

            Assert.That(BabCalculator.Calculate(levels), Is.EqualTo(expectedBab));
        }

        [TestCase(20, 0, 0, 10)]
        [TestCase(0, 20, 0, 15)]
        [TestCase(0, 0, 20, 20)]
        [TestCase(1, 1, 0, 0)]
        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 2, 0, 1)]
        [TestCase(1, 3, 0, 2)]
        [TestCase(2, 3, 0, 3)]
        public void CalculateBab_MulticlassWithDifferentRates_ShouldReturnCorrectBab(int slowRateLevels, int mediumRateLevels, int fastRateLevels, int expectedBab)
        {
            var levels = new List<HydratedClassLevel>();

            // A test case of 1,1,0,0 might be 1 Wizard level (slow) and 1 Rogue level (medium),
            // which is the base rules is 0.75 (round down to 0) and 05. (round down to 0) for a
            // total of 0

            for (int i = 1; i <= fastRateLevels; i++)
            {
                levels.Add(new HydratedClassLevel
                {
                    ClassDefinition = _classRegistry.GetClassDefinition("Fighter"),
                    ClassLevel = i
                });
            }

            for (int i = 1; i <= mediumRateLevels; i++)
            {
                levels.Add(new HydratedClassLevel
                {
                    ClassDefinition = _classRegistry.GetClassDefinition("Rogue"),
                    ClassLevel = i
                });
            }

            for (int i = 1; i <= slowRateLevels; i++)
            {
                levels.Add(new HydratedClassLevel
                {
                    ClassDefinition = _classRegistry.GetClassDefinition("Wizard"),
                    ClassLevel = i
                });
            }

            Assert.That(BabCalculator.Calculate(levels), Is.EqualTo(expectedBab));
        }

        [TestCase(BabProgressionRate.Fast, 1, 1, 2)]
        [TestCase(BabProgressionRate.Fast, 1, 5, 6)]
        [TestCase(BabProgressionRate.Medium, 1, 2, 1)]
        [TestCase(BabProgressionRate.Medium, 2, 2, 2)]
        [TestCase(BabProgressionRate.Medium, 1, 3, 2)]
        [TestCase(BabProgressionRate.Medium, 1, 4, 3)]
        [TestCase(BabProgressionRate.Slow, 1, 1, 0)]
        [TestCase(BabProgressionRate.Slow, 2, 1, 1)]
        [TestCase(BabProgressionRate.Slow, 2, 2, 2)]
        [TestCase(BabProgressionRate.Slow, 3, 3, 2)]
        public void CalculateBab_MulticlassWithSameRates_ShouldReturnCorrectBab(BabProgressionRate babRate, int classALevels, int classBLevels, int expectedBab)
        {
            var levels = new List<HydratedClassLevel>();

            var classA = babRate switch
            {
                BabProgressionRate.Fast => _classRegistry.GetClassDefinition("Fighter"),
                BabProgressionRate.Medium => _classRegistry.GetClassDefinition("Rogue"),
                BabProgressionRate.Slow => _classRegistry.GetClassDefinition("Wizard"),
            };

            for (int i = 1; i <= classALevels; i++)
            {
                levels.Add(new HydratedClassLevel
                {
                    ClassDefinition = classA,
                    ClassLevel = i
                });
            }

            var classB = babRate switch
            {
                BabProgressionRate.Fast => _classRegistry.GetClassDefinition("Fighter2"),
                BabProgressionRate.Medium => _classRegistry.GetClassDefinition("Rogue2"),
                BabProgressionRate.Slow => _classRegistry.GetClassDefinition("Wizard2"),
            };

            for (int i = 1; i <= classBLevels; i++)
            {
                levels.Add(new HydratedClassLevel
                {
                    ClassDefinition = classB,
                    ClassLevel = i
                });
            }

            Assert.That(BabCalculator.Calculate(levels), Is.EqualTo(expectedBab));
        }
    }
}
