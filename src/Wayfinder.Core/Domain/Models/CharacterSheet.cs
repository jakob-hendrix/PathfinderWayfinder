using Wayfinder.Core.Rules.Services;
using Wayfinder.Tests.Core;

namespace Wayfinder.Core.Domain.Models
{
    /// <summary>
    /// Rich Domain Model that represents a character with state - meaning class levels, ineventory, etc
    /// </summary>
    public class CharacterSheet
    {
        private readonly CharacterEntity _baseCharacter;
        private readonly List<ClassLevel> _levels;

        // Calculators
        private readonly IStatCalculator _statCalculator;
        private readonly IBabCalculator _babCalculator;
        private readonly ISaveCalculator _saveCalculator;
        private readonly IAbilityScoreCalculator _abilityScoreCalculator;

        public CharacterSheet(CharacterEntity baseCharacter, List<ClassLevel> levels, IStatCalculator statCalculator, IBabCalculator babCalculator, ISaveCalculator saveCalculator)
        {
            _baseCharacter = baseCharacter;
            _levels = levels;
            _statCalculator = statCalculator;
            _babCalculator = babCalculator;
            _saveCalculator = saveCalculator;
        }

        // Ability Scores
        public int Strength => CalculateAbilityScore(_baseCharacter.BaseStrength);
        public int Dexterity => CalculateAbilityScore(_baseCharacter.BaseDexterity);
        public int Constitution => CalculateAbilityScore(_baseCharacter.BaseConstitution);
        public int Intelligence => CalculateAbilityScore(_baseCharacter.BaseIntelligence);
        public int Wisdom => CalculateAbilityScore(_baseCharacter.BaseWisdom);
        public int Charisma => CalculateAbilityScore(_baseCharacter.BaseCharisma);

        private int CalculateAbilityScore(int baseScore) => _abilityScoreCalculator.Calculate(baseScore, _levels);
    }
}
