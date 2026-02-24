using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.Core.DomainModels.Characters;
using Wayfinder.Core.DomainModels.Items;
using Wayfinder.Core.Extensions;
using Wayfinder.Core.Services;
using Wayfinder.Infrastructure.DataSeeders;

namespace Wayfinder.App.Services
{
    // TODO: add auto-save feature
    // "Debounced save" - save whenever a change happens and some period of idle time occurs
    public partial class CharacterSheetViewModel : ObservableObject, IDisposable
    {
        // Holds the main state of the character, including all calculated values, inventory, etc
        private readonly IAppLogger _logger;
        private readonly IPathfinderRulesEngine _rulesEngine;
        private readonly CharacterStateService _characterStateService;
        private readonly AppStateService _appStateService;
        private readonly SampleCharacterSeeder _characterSeeder;

        public CharacterSheetViewModel(IAppLogger logger, IPathfinderRulesEngine rulesEngine, AppStateService appStateService, SampleCharacterSeeder characterSeeder, CharacterStateService stateService)
        {
            _logger = logger;
            _rulesEngine = rulesEngine;
            _characterStateService = stateService;
            _appStateService = appStateService;
            _characterSeeder = characterSeeder;

            // Watch the UI to see if the data libraries were refreshed. We need to force a recalc
            // of the sheet
            _appStateService.OnDataRefreshed += HandleDataRefreshed;

            // If our base character entity changes, recalc our sheet
            _characterStateService.OnStateChanged += TriggerFullRecalc;

            Initialize();

            _logger.LogInfo($"Initialized CharacterStateViewModel");
        }

        // Character collections 
        [ObservableProperty]
        public CharacterSheet? _activeCharacterSheet;
        public ObservableCollection<ItemInstance> Inventory { get; } = new();

        private void HandleDataRefreshed() => RebuildState();
        private void RebuildState()
        {
            if (ActiveCharacterSheet == null) return;

            // 1. Wipe the UI's existing projections of the character sheet
            Inventory.Clear();

            // 2. Ask the sheet for hydrated facts
            var refreshedItems = ActiveCharacterSheet.GetHydratedInventory();

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
            if (ActiveCharacterSheet is null)
            {
                InitializeNewCharacter();
            }
        }

        public void InitializeNewCharacter()
        {
            _characterStateService.ActiveCharacter = _characterSeeder.BuildSampleCharacter();
            ActiveCharacterSheet = new CharacterSheet(_characterStateService.ActiveCharacter, _rulesEngine);
            RebuildState();
        }

        #region Properties Exposed to the UI
        public string Race => SafeCalculate(() => ActiveCharacterSheet?.BaseCharacter.Race ?? "No Race Selected", "Race Display");
        public string Alignment => SafeCalculate(() => ActiveCharacterSheet?.BaseCharacter.Alignment.ToString().SplitCamelCase() ?? "No Alignment Selected", "Alignment Display");
        public string Gender => SafeCalculate(() => ActiveCharacterSheet?.BaseCharacter.Gender ?? "Not Specified", "Gender Display");
        public string Deity => SafeCalculate(() => ActiveCharacterSheet?.BaseCharacter.Deity ?? "None", "Deity Display");
        public int Age => SafeCalculate(() => ActiveCharacterSheet?.BaseCharacter.Age ?? 0, "Age Display");
        public string PhysicalDescription => SafeCalculate(() => ActiveCharacterSheet?.BaseCharacter.PhysicalDescription ?? "Nothing distinguishing at all.", "Physical Description Display");

        public int Strength => SafeCalculate(() => ActiveCharacterSheet.Strength, "Strength Calculation");
        public int Dexterity => SafeCalculate(() => ActiveCharacterSheet.Dexterity, "Dexterity Calculation");
        public int Constitution => SafeCalculate(() => ActiveCharacterSheet.Constitution, "Constitution Calculation");
        public int Intelligence => SafeCalculate(() => ActiveCharacterSheet.Intelligence, "Intelligence Calculation");
        public int Wisdom => SafeCalculate(() => ActiveCharacterSheet.Wisdom, "Wisdom Calculation");
        public int Charisma => SafeCalculate(() => ActiveCharacterSheet.Charisma, "Charisma Calculation");

        // TODO do others

        #endregion

        public void ToggleItemEquipped(Guid itemId)
        {
            // Update character
            ActiveCharacterSheet.ToggleEquip(itemId);

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
            ActiveCharacterSheet.ToggleCarried(itemId);

            var item = Inventory.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                item.IsCarried = !item.IsCarried;
            }

            RecalculateStats();
        }

        private void RecalculateStats()
        {
            if (ActiveCharacterSheet != null)
            {
                // The character sheet knows how to convert entity facts into library rules
                // We just need to tell the UI that these things have changed
                ActiveCharacterSheet.Refresh();
            }

            // TODO: use OnPrepertyChanged(nameof(prop)) for each property
            // each property on the VM should be pulling from the underlying sheet

            // Trick to trigger a change in all properties, to handle the ripple of Pathfinder changes
            // Let's see how much lag this introduces...
            TriggerFullRecalc();
        }

        private void TriggerFullRecalc() => OnPropertyChanged(string.Empty);


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
            _characterStateService.OnStateChanged -= TriggerFullRecalc;
        }
    }
}
