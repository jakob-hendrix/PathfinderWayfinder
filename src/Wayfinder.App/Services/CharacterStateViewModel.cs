using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.Core.DataServices;
using Wayfinder.Core.Domain.Models;
using Wayfinder.Core.Rules.Services;
using Wayfinder.Tests.Core;

namespace Wayfinder.App.Services
{
    // TODO: add auto-save feature
    // "Debounced save" - save whenever a change happens and some period of idle time occurs
    public partial class CharacterStateViewModel : ObservableObject
    {
        // Holds the main state of the character, including all calculated values, inventory, etc
        private readonly AppLoggingService _logger;
        private readonly IStatCalculator _statCalculator;
        private readonly IBabCalculator _babCalculator;
        private readonly ISaveCalculator _saveCalculator;
        private readonly IClassRegistry _classRegistry;
        private readonly IAbilityScoreCalculator _abilityScoreCalculator;

        public CharacterStateViewModel(
            AppLoggingService logger,
            IStatCalculator statCalculator,
            IBabCalculator babCalculator,
            ISaveCalculator saveCalculator,
            IClassRegistry classRegistry,
            IAbilityScoreCalculator abilityScoreCalculator)
        {
            _logger = logger;
            _statCalculator = statCalculator;
            _babCalculator = babCalculator;
            _saveCalculator = saveCalculator;
            _classRegistry = classRegistry;
            _abilityScoreCalculator = abilityScoreCalculator;

            _logger.LogInfo($"Initialized CharacterStateViewModel");
        }

        public void Initialize()
        {
            // Check for auto save from crash
            // Prompt to save or load - this will probably be a new page
            if (ActiveCharacter is null)
            {
                InitializeNewCharacter();
            }
        }

        [ObservableProperty]
        public CharacterSheet? _activeCharacter;

        public void InitializeNewCharacter()
        {
            var entity = new CharacterEntity { Name = "Boxly The Brand Spankin New Hero" };

            ActiveCharacter = new CharacterSheet(
                entity,
                _classRegistry,
                _statCalculator,
                _babCalculator,
                _saveCalculator,
                _abilityScoreCalculator);
        }

        public int Strength => SafeCalculate(() => ActiveCharacter.Strength, "Strength Calculation");

        // TODO do others

        // Trick to trigger a change in all properties, to handle the ripple of Pathfinder changes
        // Let's see how much lag this introduces...
        public void Refresh()
        {
            OnPropertyChanged(string.Empty);
        }

        private T SafeCalculate<T>(Func<T> action, string context)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {context}", ex);
                return default;
            }
        }

    }
}
