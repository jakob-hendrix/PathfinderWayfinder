using Wayfinder.Core.DataServices;
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
        private readonly IClassRegistry _classRegistry;

        // Calculators
        private readonly IStatCalculator _statCalculator;
        private readonly IBabCalculator _babCalculator;
        private readonly ISaveCalculator _saveCalculator;
        private readonly IAbilityScoreCalculator _abilityScoreCalculator;

        public CharacterSheet(
            CharacterEntity baseCharacter,
            IClassRegistry classRegistry,
            IStatCalculator statCalculator,
            IBabCalculator babCalculator,
            ISaveCalculator saveCalculator,
            IAbilityScoreCalculator abilityScoreCalculator)
        {
            _baseCharacter = baseCharacter;
            _classRegistry = classRegistry;
            _statCalculator = statCalculator;
            _babCalculator = babCalculator;
            _saveCalculator = saveCalculator;
            _abilityScoreCalculator = abilityScoreCalculator;
        }

        // Ability Scores
        public int Strength => CalculateAbilityScore(_baseCharacter.BaseStrength);
        public int Dexterity => CalculateAbilityScore(_baseCharacter.BaseDexterity);
        public int Constitution => CalculateAbilityScore(_baseCharacter.BaseConstitution);
        public int Intelligence => CalculateAbilityScore(_baseCharacter.BaseIntelligence);
        public int Wisdom => CalculateAbilityScore(_baseCharacter.BaseWisdom);
        public int Charisma => CalculateAbilityScore(_baseCharacter.BaseCharisma);

        // Sheet Actions
        public void AddLevel(string className)
        {
            // TODO: implement adding a new level to ClassLevels
            // This will need to trigger a full recalc of the sheet
            // Will require validation (max levels, class exists, new class isn't archetype of old class, etc)
            // So this will need to be in a new factory class of some sort
        }

        // Helper functions
        private int CalculateAbilityScore(int baseScore) => _abilityScoreCalculator.Calculate(baseScore, _baseCharacter.ClassLevels);
    }
}
