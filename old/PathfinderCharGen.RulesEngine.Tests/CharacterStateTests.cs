using FluentAssertions;

namespace PathfinderCharGen.RulesEngine.Tests
{
    public class CharacterStateTests
    {
        private CharacterState _characterState;
        public CharacterStateTests()
        {
            _characterState = new CharacterState();
        }

        [Fact]
        public void CurrentCharacterIsNotNull()
        {
            _characterState.CurrentCharacter.Should().NotBeNull();
        }
    }
}


//[Theory]
//[InlineData("Eddie", "Van Halen", "Eddie Van Halen")]
//public void TestEdgeCaseNames(string firstName, string lastName, string fullName)
//{
//    // Arrange
//    SampleClass sampleClass = new SampleClass(fullName);

//    // AssertionScope allows combining the following tests
//    using var _ = new AssertionScope();

//    sampleClass.LastName.Should().Be(lastName);
//    sampleClass.LastName.Should()
//        .StartWith(lastName.Substring(0, 3))
//        .And.EndWith(lastName.Substring(lastName.Length - 3))
//        .And.Contain(" ");
//}