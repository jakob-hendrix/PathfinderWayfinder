using NUnit.Framework;
using Wayfinder.Core.Extensions;

namespace Wayfinder.Tests.Core
{
    [TestFixture]
    public class PathfinderExtensionsTests
    {
        private class DummyLevel
        {
            public string Name { get; set; } = string.Empty;
        }

        [TestCase("test", "test")]
        [TestCase("test 1", "test_1")]
        [TestCase("  test 2", "test_2")]
        [TestCase("  test 3  ", "test_3")]
        [TestCase("Heart of the Fields", "heart_of_the_fields")]
        [TestCase("Heart of the Fields (test)", "heart_of_the_fields_test")]
        public void GenerateIdFromName_ShouldReturnCorrectId(string name, string expectedId)
        {
            Assert.That(name.GenerateIdFromName(), Is.EqualTo(expectedId));
        }

        [TestCase("test", "test")]
        [TestCase("Test", "Test")]
        [TestCase("TestTwo", "Test Two")]
        [TestCase("TestThree TestFour", "Test Three Test Four")]
        public void SplitCamelCase_ShouldReturnCorrectResult(string input, string expected)
        {
            Assert.That(input.SplitCamelCase(), Is.EqualTo(expected));
        }

        [Test]
        public void GetDataByLevel_WithValidLevel_ReturnsCorrectItem()
        {
            var levels = new List<DummyLevel>()
            {
                new DummyLevel{ Name = "Level 1"},
                new DummyLevel{Name = "Level 2"}
            };

            var result1 = levels.GetDataByLevel(1);
            var result2 = levels.GetDataByLevel(2);

            Assert.That(result1?.Name, Is.EqualTo("Level 1"));
            Assert.That(result2?.Name, Is.EqualTo("Level 2"));
        }

        [Test]
        public void GetDataByLevel_WithLevelOutOfBounds_ReturnsNull()
        {
            var levels = new List<DummyLevel>()
            {
                new DummyLevel{ Name = "Level 1"}
            };

            var result = levels.GetDataByLevel(2);

            Assert.That(result, Is.Null, "Requesting level beyond the collection should return null");
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(21)]
        public void GetDataByLevel_WithLevelOutOfBounds_ReturnsNull(int badIndex)
        {
            var levels = new List<DummyLevel>() { new DummyLevel { Name = "Level 1" } };
            var result = levels.GetDataByLevel(badIndex);
            Assert.That(result, Is.Null);
        }
    }
}
