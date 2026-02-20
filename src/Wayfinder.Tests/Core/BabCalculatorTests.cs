using NUnit.Framework;
using Wayfinder.Core.Domain.Constants;
using Wayfinder.Core.Domain.Data.Classes;
using Wayfinder.Core.Domain.Models;
using Wayfinder.Core.Rules.Services;
using Wayfinder.Tests.Core.Mocks;

namespace Wayfinder.Tests.Core
{
    [TestFixture]
    public class BabCalculatorTests
    {
        private IBabCalculator _calculator;
        private MockClassRegistry _classRegistry;

        [SetUp]
        public void Setup()
        {
            _classRegistry = new MockClassRegistry();
            _classRegistry.Classes.Add("Fighter", new CharacterTestClass("Fighter", BabProgressionRate.Fast));
            _classRegistry.Classes.Add("Rogue", new CharacterTestClass("Rogue", BabProgressionRate.Medium));
            _classRegistry.Classes.Add("Wizard", new CharacterTestClass("Wizard", BabProgressionRate.Slow));

            _calculator = new BabCalculator(_classRegistry);
        }

        [Test]
        public void CalculateBab_ShouldReturn0_WhenNoLevels()
        {
            var levels = new List<ClassLevel>();
            Assert.That(_calculator.Calculate(levels), Is.EqualTo(0));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        [TestCase(20, 20)]
        public void CalculateBab_FastRate_ShouldReturnCorrectBab(int levelCount, int expectedBab)
        {
            var levels = new List<ClassLevel>();

            for (int i = 1; i <= levelCount; i++)
            {
                levels.Add(new ClassLevel
                {
                    Class = _classRegistry.GetClass("Fighter"),
                    Level = i
                });
            }

            Assert.That(_calculator.Calculate(levels), Is.EqualTo(expectedBab));
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
            var levels = new List<ClassLevel>();

            for (int i = 1; i <= levelCount; i++)
            {
                levels.Add(new ClassLevel
                {
                    Class = _classRegistry.GetClass("Rogue"),
                    Level = i
                });
            }

            Assert.That(_calculator.Calculate(levels), Is.EqualTo(expectedBab));
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
            var levels = new List<ClassLevel>();

            for (int i = 1; i <= levelCount; i++)
            {
                levels.Add(new ClassLevel
                {
                    Class = _classRegistry.GetClass("Wizard"),
                    Level = i
                });
            }

            Assert.That(_calculator.Calculate(levels), Is.EqualTo(expectedBab));
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
            var levels = new List<ClassLevel>();

            // A test case of 1,1,0,0 might be 1 Wizard level (slow) and 1 Rogue level (medium),
            // which is the base rules is 0.75 (round down to 0) and 05. (round down to 0) for a
            // total of 0

            for (int i = 1; i <= fastRateLevels; i++)
            {
                levels.Add(new ClassLevel
                {
                    Class = _classRegistry.GetClass("Fighter"),
                    Level = i
                });
            }

            for (int i = 1; i <= mediumRateLevels; i++)
            {
                levels.Add(new ClassLevel
                {
                    Class = _classRegistry.GetClass("Rogue"),
                    Level = i
                });
            }

            for (int i = 1; i <= slowRateLevels; i++)
            {
                levels.Add(new ClassLevel
                {
                    Class = _classRegistry.GetClass("Wizard"),
                    Level = i
                });
            }

            Assert.That(_calculator.Calculate(levels), Is.EqualTo(expectedBab));
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
            var levels = new List<ClassLevel>();

            var classA = new CharacterTestClass("Class A", babRate);
            _classRegistry.Classes["Class A"] = classA;

            for (int i = 1; i <= classALevels; i++)
            {
                levels.Add(new ClassLevel
                {
                    Class = classA,
                    Level = i
                });
            }

            var classB = new CharacterTestClass("Class B", babRate);
            _classRegistry.Classes["Class B"] = classB;

            for (int i = 1; i <= classBLevels; i++)
            {
                levels.Add(new ClassLevel
                {
                    Class = classB,
                    Level = i
                });
            }

            Assert.That(_calculator.Calculate(levels), Is.EqualTo(expectedBab));
        }
    }
}
