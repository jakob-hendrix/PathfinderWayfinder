using NUnit.Framework;
using Wayfinder.Core.Rules.Extensions;

namespace Wayfinder.Tests.Core
{
    [TestFixture]
    public class PathfinderMathExtensionsTests
    {
        [TestCase(0, -5)]
        [TestCase(1, -5)]
        [TestCase(2, -4)]
        [TestCase(3, -4)]
        [TestCase(4, -3)]
        [TestCase(5, -3)]
        [TestCase(6, -2)]
        [TestCase(7, -2)]
        [TestCase(8, -1)]
        [TestCase(9, -1)]
        [TestCase(10, 0)]
        [TestCase(11, 0)]
        [TestCase(12, 1)]
        [TestCase(13, 1)]
        [TestCase(14, 2)]
        [TestCase(15, 2)]
        [TestCase(16, 3)]
        [TestCase(17, 3)]
        [TestCase(18, 4)]
        [TestCase(19, 4)]
        [TestCase(20, 5)]
        [TestCase(33, 11)]
        public void ToModifier_ShouldReturnCorrectModifier(int score, int expected)
        {
            var result = score.ToModifier();
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
