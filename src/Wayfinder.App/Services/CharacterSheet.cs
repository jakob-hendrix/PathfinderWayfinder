using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.Core.Domain.Models;
using Wayfinder.Core.Rules.Services;

namespace Wayfinder.App.Services
{
    public partial class CharacterSheet : ObservableObject
    {
        private readonly IStatCalculator _statCalculator;
        private readonly IBabCalculator _babCalculator;

        public CharacterSheet(IStatCalculator statCalculator, IBabCalculator babCalculator)
        {
            _statCalculator = statCalculator;
            _babCalculator = babCalculator;
        }

        private CharacterEntity? _currentBaseCharacter;
    }
}
