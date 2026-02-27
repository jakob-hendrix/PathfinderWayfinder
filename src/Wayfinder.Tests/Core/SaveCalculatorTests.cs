using NUnit.Framework;
using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.DataServices;
using Wayfinder.Core.Enums;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Rules.Calculators;
using Wayfinder.Core.Services;

namespace Wayfinder.Tests.Core
{
    [TestFixture]
    public class SaveCalculatorTests
    {
        private IClassLibrary _classRegistry;
        private IClassFactory _classFactory;

        [SetUp]
        public void Setup()
        {
            _classRegistry = new ClassLibrary();
            _classRegistry.Register(new ClassDefinition() { Name = "Fighter", FortitudeRate = "Fast", WillRate = "Slow", ReflexRate = "Slow" });
            _classRegistry.Register(new ClassDefinition() { Name = "Rogue", FortitudeRate = "Slow", WillRate = "Slow", ReflexRate = "Fast" });
            _classRegistry.Register(new ClassDefinition() { Name = "Wizard", FortitudeRate = "Slow", WillRate = "Fast", ReflexRate = "Slow" });

            _classRegistry.Register(new ClassDefinition() { Name = "AllFast1", FortitudeRate = "Fast", WillRate = "Fast", ReflexRate = "Fast" });
            _classRegistry.Register(new ClassDefinition() { Name = "AllFast2", FortitudeRate = "Fast", WillRate = "Fast", ReflexRate = "Fast" });

            _classRegistry.Register(new ClassDefinition() { Name = "AllSlow1", FortitudeRate = "Slow", WillRate = "Slow", ReflexRate = "Slow" });
            _classRegistry.Register(new ClassDefinition() { Name = "AllSlow2", FortitudeRate = "Slow", WillRate = "Slow", ReflexRate = "Slow" });

            _classFactory = new ClassFactory(_classRegistry);
        }

        [Test]
        public void CalculateSave_ShouldReturn0_WhenNoLevels()
        {
            var levels = new List<ClassLevel>();
            Assert.That(SaveCalculator.Calculate(levels, SaveType.Will), Is.EqualTo(0));
            Assert.That(SaveCalculator.Calculate(levels, SaveType.Fortitude), Is.EqualTo(0));
            Assert.That(SaveCalculator.Calculate(levels, SaveType.Reflex), Is.EqualTo(0));
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
                    Class = _classFactory.GetClass("Fighter"),
                    Level = i
                });
            }

            Assert.That(SaveCalculator.Calculate(levels, SaveType.Fortitude), Is.EqualTo(expectedSave));
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
                    Class = _classFactory.GetClass("Fighter"),
                    Level = i
                });
            }

            Assert.That(SaveCalculator.Calculate(levels, SaveType.Will), Is.EqualTo(expectedSave));
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

            var classA = _classFactory.GetClass("AllFast1");
            for (int i = 1; i <= classALevels; i++)
            {
                levels.Add(new ClassLevel
                {
                    Class = classA,
                    Level = i
                });
            }

            var classB = _classFactory.GetClass("AllFast2");
            for (int i = 1; i <= classBLevels; i++)
            {
                levels.Add(new ClassLevel
                {
                    Class = classB,
                    Level = i
                });
            }

            Assert.That(SaveCalculator.Calculate(levels, SaveType.Fortitude), Is.EqualTo(expectedSave));
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

            var classA = _classFactory.GetClass("AllSlow1");
            for (int i = 1; i <= classALevels; i++)
            {
                levels.Add(new ClassLevel
                {
                    Class = classA,
                    Level = i
                });
            }

            var classB = _classFactory.GetClass("AllSlow2");
            for (int i = 1; i <= classBLevels; i++)
            {
                levels.Add(new ClassLevel
                {
                    Class = classB,
                    Level = i
                });
            }

            Assert.That(SaveCalculator.Calculate(levels, SaveType.Will), Is.EqualTo(expectedSave));
        }
    }
}
