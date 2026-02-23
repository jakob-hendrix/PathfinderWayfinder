using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.Core.DomainModels.Characters;
using Wayfinder.Core.DomainModels.Items;
using Wayfinder.Core.Services;
using Wayfinder.Infrastructure.DataSeeders;

namespace Wayfinder.App.Services
{
    // TODO: add auto-save feature
    // "Debounced save" - save whenever a change happens and some period of idle time occurs
    public partial class CharacterStateViewModel : ObservableObject, IDisposable
    {
        // Holds the main state of the character, including all calculated values, inventory, etc
        private readonly IAppLogger _logger;
        private readonly IPathfinderRulesEngine _rulesEngine;
        private readonly AppStateService _appStateService;
        private readonly SampleCharacterSeeder _characterSeeder;

        public CharacterStateViewModel(IAppLogger logger, IPathfinderRulesEngine rulesEngine, AppStateService appStateService, SampleCharacterSeeder characterSeeder)
        {
            _logger = logger;
            _rulesEngine = rulesEngine;
            _appStateService = appStateService;
            _characterSeeder = characterSeeder;

            // Watch the UI to see if the data libraries were refreshed. We need to force a recalc
            // of the sheet
            _appStateService.OnDataRefreshed += HandleDataRefreshed;

            Initialize();

            _logger.LogInfo($"Initialized CharacterStateViewModel");
        }

        // Character collections 
        [ObservableProperty]
        public CharacterSheet? _activeCharacter;
        public ObservableCollection<ItemInstance> Inventory { get; } = new();

        private void HandleDataRefreshed() => RebuildState();
        private void RebuildState()
        {
            if (ActiveCharacter == null) return;

            // 1. Wipe the UI's existing projections of the character sheet
            Inventory.Clear();

            // 2. Ask the sheet for hydrated facts
            var refreshedItems = ActiveCharacter.GetHydratedInventory();

            // 3. Fill our collection
            foreach (var item in refreshedItems)
            {
                Inventory.Add(item);
            }

            // 4. Recalculate Stats
            RecalculateStats();
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

        public void InitializeNewCharacter()
        {
            var entity = _characterSeeder.BuildSampleCharacter();
            ActiveCharacter = new CharacterSheet(entity, _rulesEngine);
            RebuildState();
        }

        public int Strength => SafeCalculate(() => ActiveCharacter.Strength, "Strength Calculation");

        // TODO do others

        public void ToggleItemEquipped(Guid itemId)
        {
            // Update character
            ActiveCharacter.ToggleEquip(itemId);

            var item = Inventory.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                //item.IsEquipped = !item.IsEquipped;
            }

            RecalculateStats();
        }

        public void ToggleItemCarried(Guid itemId)
        {
            // Update character
            ActiveCharacter.ToggleCarried(itemId);

            var item = Inventory.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                item.IsCarried = !item.IsCarried;
            }

            RecalculateStats();
        }

        private void RecalculateStats()
        {
            // The character sheet knows how to convert entity facts into library rules
            // We just need to tell the UI that these things have changed
            ActiveCharacter.Refresh();

            // TODO: use OnPrepertyChanged(nameof(prop)) for each property
            // each property on the VM should be pulling from the underlying sheet

            // Trick to trigger a change in all properties, to handle the ripple of Pathfinder changes
            // Let's see how much lag this introduces...
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

        public void Dispose()
        {
            _appStateService.OnDataRefreshed -= HandleDataRefreshed;
        }
    }
}
