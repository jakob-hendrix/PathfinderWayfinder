using NUnit.Framework;
using Wayfinder.Core.Extensions;

namespace Wayfinder.Tests.Core
{
    [TestFixture]
    public class PathfinderExtensionsTests
    {
        [TestCase("test", "test")]
        [TestCase("test 1", "test_1")]
        [TestCase("  test 2", "__test_2")]
        [TestCase("  test 3  ", "__test_3")]
        [TestCase("Heart of the Fields", "heart_of_the_fields")]
        [TestCase("Heart of the Fields (test)", "heart_of_the_fields_test")]
        public void GenerateIdFromName_ShouldReturnCorrectId(string name, string expectedId)
        {
            Assert.That(name.GenerateIdFromName(), Is.EqualTo(expectedId));
        }

        [TestCase("test", "test")]
        [TestCase("Test", "Test")]
        [TestCase("TestTwo", "Test Two")]
        [TestCase("TestThree ThreeFour", "Test Three Test Four")]
        public void SplitCamelCase_ShouldReturnCorrectResult(string input, string expected)
        {
            Assert.That(input.SplitCamelCase(), Is.EqualTo(expected));
        }
    }
}
