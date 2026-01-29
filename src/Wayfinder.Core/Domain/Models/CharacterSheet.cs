using Wayfinder.Core.Domain.Constants;

namespace Wayfinder.Core.Domain.Models
{
    public class CharacterSheet
    {
        private CharacterEntity _character;

        public CharacterSheet()
        {
            _character.BaseAbilityScores[AbilityScore.Strength] = 12; //eg using Dict. remove later
        }

        // 
    }
}
