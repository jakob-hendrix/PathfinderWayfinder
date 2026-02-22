using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.Core.DomainModels.Characters;
using Wayfinder.Core.Services;

namespace Wayfinder.App.Services
{
    // TODO: add auto-save feature
    // "Debounced save" - save whenever a change happens and some period of idle time occurs
    public partial class CharacterStateViewModel : ObservableObject
    {
        // Holds the main state of the character, including all calculated values, inventory, etc
        private readonly IAppLogger _logger;
        private readonly IPathfinderRulesEngine _rulesEngine;

        public CharacterStateViewModel(IAppLogger logger, IPathfinderRulesEngine rulesEngine)
        {
            _logger = logger;
            _rulesEngine = rulesEngine;

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

            ActiveCharacter = new CharacterSheet(entity, _rulesEngine);
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
