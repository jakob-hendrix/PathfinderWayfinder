using System.Dynamic;
using PathfinderCharGen.RulesEngine.DataModels;

namespace PathfinderCharGen.RulesEngine
{
    public class CharacterState
    {
        public PlayerCharacter CurrentCharacter { get; set; }

        public CharacterState()
        {
            CurrentCharacter = new PlayerCharacter();
        }
    }
}
