using NUnit.Framework;
using Wayfinder.Core.Domain.Constants;
using Wayfinder.Core.Domain.Data.Classes;
using Wayfinder.Core.Domain.Models;
using Wayfinder.Core.Rules.Services;
using Wayfinder.Tests.Core.Mocks;

namespace Wayfinder.Tests.Core
{
    [TestFixture]
    public class SaveCalculatorTests
    {
        private ISaveCalculator _calculator;
        private MockClassRegistry _classRegistry;

        [SetUp]
        public void Setup()
        {
            _classRegistry = new MockClassRegistry();
            _classRegistry.Classes.Add("Fighter", new CharacterTestClass("Fighter", SaveProgressionRate.Fast, SaveProgressionRate.Slow, SaveProgressionRate.Slow));
            _classRegistry.Classes.Add("Rogue", new CharacterTestClass("Rogue", SaveProgressionRate.Slow, SaveProgressionRate.Slow, SaveProgressionRate.Fast));
            _classRegistry.Classes.Add("Wizard", new CharacterTestClass("Wizard", SaveProgressionRate.Slow, SaveProgressionRate.Fast, SaveProgressionRate.Slow));

            _calculator = new SaveCalculator(_classRegistry);
        }

        [Test]
        public void CalculateSave_ShouldReturn0_WhenNoLevels()
        {
            var levels = new List<ClassLevel>();
            Assert.That(_calculator.Calculate(levels, SaveType.Will), Is.EqualTo(0));
            Assert.That(_calculator.Calculate(levels, SaveType.Fortitude), Is.EqualTo(0));
            Assert.That(_calculator.Calculate(levels, SaveType.Reflex), Is.EqualTo(0));
        }

        [TestCase(1, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 4)]
        [TestCase(6, 5)]
        [TestCase(7, 5)]
        [TestCase(8, 6)]
        [TestCase(9, 6)]
        [TestCase(10, 7)]
        [TestCase(11, 7)]
        [TestCase(12, 8)]
        [TestCase(13, 8)]
        [TestCase(14, 9)]
        [TestCase(15, 9)]
        [TestCase(16, 10)]
        [TestCase(17, 10)]
        [TestCase(18, 11)]
        [TestCase(19, 11)]
        [TestCase(20, 12)]
        public void CalculateSave_SingleClass_FastRate_ShouldReturnCorrectSave(int levelCount, int expectedSave)
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

            Assert.That(_calculator.Calculate(levels, SaveType.Fortitude), Is.EqualTo(expectedSave));
        }

        [TestCase(1, 0)]
        [TestCase(2, 0)]
        [TestCase(3, 1)]
        [TestCase(4, 1)]
        [TestCase(5, 1)]
        [TestCase(6, 2)]
        [TestCase(7, 2)]
        [TestCase(8, 2)]
        [TestCase(9, 3)]
        [TestCase(10, 3)]
        [TestCase(11, 3)]
        [TestCase(12, 4)]
        [TestCase(13, 4)]
        [TestCase(14, 4)]
        [TestCase(15, 5)]
        [TestCase(16, 5)]
        [TestCase(17, 5)]
        [TestCase(18, 6)]
        [TestCase(19, 6)]
        [TestCase(20, 6)]
        public void CalculateSave_SingleClass_SlowRate_ShouldReturnCorrectSave(int levelCount, int expectedSave)
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

            Assert.That(_calculator.Calculate(levels, SaveType.Will), Is.EqualTo(expectedSave));
        }

        [TestCase(1, 0, 2)]
        [TestCase(0, 1, 2)]
        [TestCase(1, 1, 4)] // level 1 bonus applied 2x
        [TestCase(2, 1, 5)]
        [TestCase(3, 1, 5)]
        [TestCase(4, 1, 6)]
        [TestCase(4, 2, 7)]
        [TestCase(4, 3, 7)]
        [TestCase(4, 4, 8)]
        public void CalculateSave_MulticlassWithFastRates_ShouldReturnCorrectSave(int classALevels, int classBLevels, int expectedSave)
        {
            var levels = new List<ClassLevel>();

            var classA = new CharacterTestClass("Class A", SaveProgressionRate.Fast, SaveProgressionRate.Fast, SaveProgressionRate.Fast);
            _classRegistry.Classes["Class A"] = classA;

            for (int i = 1; i <= classALevels; i++)
            {
                levels.Add(new ClassLevel
                {
                    Class = classA,
                    Level = i
                });
            }

            var classB = new CharacterTestClass("Class B", SaveProgressionRate.Fast, SaveProgressionRate.Fast, SaveProgressionRate.Fast);
            _classRegistry.Classes["Class B"] = classB;

            for (int i = 1; i <= classBLevels; i++)
            {
                levels.Add(new ClassLevel
                {
                    Class = classB,
                    Level = i
                });
            }

            Assert.That(_calculator.Calculate(levels, SaveType.Fortitude), Is.EqualTo(expectedSave));
        }

        [TestCase(1, 0, 0)]
        [TestCase(0, 1, 0)]
        [TestCase(1, 1, 0)] // level 1 bonus applied 2x
        [TestCase(2, 1, 0)]
        [TestCase(3, 1, 1)]
        [TestCase(4, 1, 1)]
        [TestCase(4, 2, 1)]
        [TestCase(4, 3, 2)]
        [TestCase(4, 4, 2)]
        [TestCase(6, 4, 3)]
        [TestCase(7, 4, 3)]
        [TestCase(8, 4, 3)]
        [TestCase(9, 4, 4)]
        [TestCase(9, 5, 4)]
        [TestCase(9, 6, 5)]
        public void CalculateSave_MulticlassWithSlowRates_ShouldReturnCorrectSave(int classALevels, int classBLevels, int expectedSave)
        {
            var levels = new List<ClassLevel>();

            var classA = new CharacterTestClass("Class A", SaveProgressionRate.Slow, SaveProgressionRate.Slow, SaveProgressionRate.Slow);
            _classRegistry.Classes["Class A"] = classA;

            for (int i = 1; i <= classALevels; i++)
            {
                levels.Add(new ClassLevel
                {
                    Class = classA,
                    Level = i
                });
            }

            var classB = new CharacterTestClass("Class B", SaveProgressionRate.Slow, SaveProgressionRate.Slow, SaveProgressionRate.Slow);
            _classRegistry.Classes["Class B"] = classB;

            for (int i = 1; i <= classBLevels; i++)
            {
                levels.Add(new ClassLevel
                {
                    Class = classB,
                    Level = i
                });
            }

            Assert.That(_calculator.Calculate(levels, SaveType.Will), Is.EqualTo(expectedSave));
        }
    }
}
