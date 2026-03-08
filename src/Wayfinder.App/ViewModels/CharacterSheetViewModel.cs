using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.Core.Extensions;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Items;

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
        public CharacterSheet? ActiveCharacterSheet => _characterStateService.ActiveSheet;

        public CharacterSheetViewModel(IAppLogger logger, IPathfinderRulesEngine rulesEngine, AppStateService appStateService, CharacterStateService stateService)
        {
            _logger = logger;
            _rulesEngine = rulesEngine;
            _characterStateService = stateService;
            _appStateService = appStateService;

            LoadVitalsFromDomain();

            // Watch the UI to see if the data libraries were refreshed. We need to force a recalc
            // of the sheet
            _appStateService.OnDataRefreshed += HandleDataRefreshed;

            // If our base character entity changes, recalc our sheet
            _characterStateService.StateChanged += TriggerFullRecalc;

            Initialize();

            _logger.LogInfo($"Initialized CharacterStateViewModel");
        }

        private void LoadVitalsFromDomain()
        {
            if (ActiveCharacterSheet == null) return;

            Wounds = ActiveCharacterSheet.Wounds;
            NonLethalDamage = ActiveCharacterSheet.NonLethalDamage;
            TemporaryHp = ActiveCharacterSheet.TemporaryHp;
        }

        public ObservableCollection<ItemInstance> Inventory { get; } = new();

        #region Properties Exposed to the UI
        public Race? CurrentRace => ActiveCharacterSheet?.Race;
        public bool HasRace => CurrentRace != null;
        public string RaceFullTitle => CurrentRace != null
                ? $"{CurrentRace.Name}{(CurrentRace.Subrace != null ? $" ({CurrentRace.Subrace.Name})" : "")}"
                : "No Race Selected";

        public IEnumerable<RacialTrait> ActiveTraits =>
            CurrentRace?.SelectedRacialTraits ?? Enumerable.Empty<RacialTrait>();

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

        public int BaseAttackBonus => ActiveCharacterSheet?.BaseAttackBonus ?? 0;
        public int FortitudeSave => ActiveCharacterSheet?.FortitudeSave ?? 0;
        public int ReflexSave => ActiveCharacterSheet?.ReflexSave ?? 0;
        public int WillSave => ActiveCharacterSheet?.WillSave ?? 0;

        // Mutable - bound to UI
        [ObservableProperty] private int _wounds;
        [ObservableProperty] private int _nonLethalDamage;
        [ObservableProperty] private int _temporaryHp;

        public int MaxHp => ActiveCharacterSheet?.MaxHp ?? 0;
        public int CurrentHp => ActiveCharacterSheet?.CurrentHp ?? 0;

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

            TriggerFullRecalc();
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

            TriggerFullRecalc();
        }

        private void TriggerFullRecalc()
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
            _characterStateService.StateChanged -= TriggerFullRecalc;
        }


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
            TriggerFullRecalc();
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
            _characterStateService.CreateNewCharacter();
            RebuildState();
        }

        partial void OnWoundsChanged(int value) => ApplyVitalsChange();
        partial void OnNonLethalDamageChanged(int value) => ApplyVitalsChange();
        partial void OnTemporaryHpChanged(int value) => ApplyVitalsChange();

        private void ApplyVitalsChange()
        {
            if (_characterStateService.ActiveSheet == null) return;

            // Tell domain to update it's HP
            _characterStateService.ActiveSheet.UpdateVitals(Wounds, NonLethalDamage, TemporaryHp);

            // Tell UI that current hp probably changed
            OnPropertyChanged(nameof(CurrentHp));
        }
    }
}
