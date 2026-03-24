using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.Core.DomainModels.Stats;
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
        private readonly CharacterStateService _characterStateService;
        private readonly AppStateService _appStateService;
        public CharacterSheet? ActiveSheet => _characterStateService.ActiveSheet;

        public CharacterSheetViewModel(IAppLogger logger, AppStateService appStateService, CharacterStateService stateService)
        {
            _logger = logger;
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
            if (ActiveSheet == null) return;

            Wounds = ActiveSheet.Wounds;
            NonLethalDamage = ActiveSheet.NonLethalDamage;
            TemporaryHp = ActiveSheet.TemporaryHp;
        }

        public ObservableCollection<ItemInstance> Inventory { get; } = new();

        #region Properties Exposed to the UI
        public Race? CurrentRace => ActiveSheet?.Race;
        public bool HasRace => CurrentRace != null;
        public string RaceFullTitle => CurrentRace != null
                ? $"{CurrentRace.Name}{(CurrentRace.Subrace != null ? $" ({CurrentRace.Subrace.Name})" : "")}"
                : "No Race Selected";

        public IEnumerable<RacialTrait> ActiveTraits =>
            CurrentRace?.SelectedRacialTraits ?? Enumerable.Empty<RacialTrait>();

        public string Alignment => SafeCalculate(
            () => ActiveSheet?.BaseCharacter.Alignment.ToString().SplitCamelCase() ?? "No Alignment Selected",
            "No Alignment Selected",
            "Alignment Display");

        public string Gender => SafeCalculate(
            () => ActiveSheet?.BaseCharacter.Gender ?? "Not Specified",
            "Not Specified",
            "Gender Display");

        public string Deity => SafeCalculate(
            () => ActiveSheet?.BaseCharacter.Deity ?? "None",
            "None",
            "Deity Display");

        public int Age => SafeCalculate(
            () => ActiveSheet?.BaseCharacter.Age ?? 0,
            0,
            "Age Display");

        public string PhysicalDescription => SafeCalculate(
            () => ActiveSheet?.BaseCharacter.PhysicalDescription ?? "Nothing distinguishing at all.",
            "Nothing distinguishing at all.",
            "Physical Description Display");

        // For the ability scores, if ActiveSheet is null, it will throw an exception 
        // which SafeCalculate will catch, safely returning the 0 fallback!
        public int Strength => SafeCalculate(() => ActiveSheet?.Strength ?? 0, 0, "Strength Calculation");
        public int Dexterity => SafeCalculate(() => ActiveSheet?.Dexterity ?? 0, 0, "Dexterity Calculation");
        public int Constitution => SafeCalculate(() => ActiveSheet?.Constitution ?? 0, 0, "Constitution Calculation");
        public int Intelligence => SafeCalculate(() => ActiveSheet?.Intelligence ?? 0, 0, "Intelligence Calculation");
        public int Wisdom => SafeCalculate(() => ActiveSheet?.Wisdom ?? 0, 0, "Wisdom Calculation");
        public int Charisma => SafeCalculate(() => ActiveSheet?.Charisma ?? 0, 0, "Charisma Calculation");

        public int BaseAttackBonus => ActiveSheet?.BaseAttackBonus ?? 0;
        public ModifiableStat FortitudeSave => SafeCalculate(
            action: () => ActiveSheet?.Fortitude ?? new ModifiableStat { Name = "Fortitude" },
            fallbackValue: new ModifiableStat { Name = "Fortitude (Error)" },
            context: "FortitudeSave Property Getter"
        );

        public ModifiableStat ReflexSave => SafeCalculate(
            action: () => ActiveSheet?.Reflex ?? new ModifiableStat { Name = "Reflex" },
            fallbackValue: new ModifiableStat { Name = "Reflex (Error)" },
            context: "ReflexSave Property Getter"
        );

        public ModifiableStat WillSave => SafeCalculate(
            action: () => ActiveSheet?.Will ?? new ModifiableStat { Name = "Will" },
            fallbackValue: new ModifiableStat { Name = "Will (Error)" },
            context: "WillSave Property Getter"
        );

        // Mutable - bound to UI
        public int Wounds
        {
            // Look directly at the source of truth
            get => ActiveSheet?.BaseCharacter.Wounds ?? 0;
            set
            {
                if (ActiveSheet != null && ActiveSheet.BaseCharacter.Wounds != value)
                {
                    // Mutate the source of truth directly
                    ActiveSheet.BaseCharacter.Wounds = value;

                    // Alert Blazor that the UI needs to redraw
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ActiveSheet));

                    // Alert the global app that we need to save to the database!
                    _characterStateService.NotifyStateChanged();
                }
            }
        }

        public int NonLethalDamage
        {
            // Look directly at the source of truth
            get => ActiveSheet?.BaseCharacter.NonLethalDamage ?? 0;
            set
            {
                if (ActiveSheet != null && ActiveSheet.BaseCharacter.NonLethalDamage != value)
                {
                    // Mutate the source of truth directly
                    ActiveSheet.BaseCharacter.NonLethalDamage = value;

                    // Alert Blazor that the UI needs to redraw
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ActiveSheet));

                    // Alert the global app that we need to save to the database!
                    _characterStateService.NotifyStateChanged();
                }
            }
        }

        public int TemporaryHp
        {
            // Look directly at the source of truth
            get => ActiveSheet?.BaseCharacter.TemporaryHp ?? 0;
            set
            {
                if (ActiveSheet != null && ActiveSheet.BaseCharacter.TemporaryHp != value)
                {
                    // Mutate the source of truth directly
                    ActiveSheet.BaseCharacter.TemporaryHp = value;

                    // Alert Blazor that the UI needs to redraw
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ActiveSheet));

                    // Alert the global app that we need to save to the database!
                    _characterStateService.NotifyStateChanged();
                }
            }
        }

        public int MaxHp => ActiveSheet?.MaxHp ?? 0;
        public int CurrentHp => ActiveSheet?.CurrentHp ?? 0;

        #endregion

        public void ToggleItemEquipped(Guid itemId)
        {
            // Update character
            ActiveSheet.ToggleEquip(itemId);

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
            ActiveSheet.ToggleCarried(itemId);

            var item = Inventory.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                item.IsCarried = !item.IsCarried;
            }

            TriggerFullRecalc();
        }

        private void TriggerFullRecalc()
        {
            if (ActiveSheet == null) return;

            try
            {
                // 1. The domain orchestrates its own internal math updates
                ActiveSheet.Refresh();
            }
            catch (Exception ex)
            {
                _logger.LogError("Catastrophic failure during ActiveSheet.Refresh()", ex);
            }
            finally
            {
                // 2. Tell the UI to rebuild the screen.
                // We put this in a finally block so that even if one specific math calculation 
                // fails mid-refresh, the UI still updates to reflect whatever *did* succeed.
                OnPropertyChanged(string.Empty);
            }
        }

        private T SafeCalculate<T>(Func<T> action, T fallbackValue, string context)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {context}", ex);
                return fallbackValue;
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
            if (ActiveSheet == null) return;

            // 1. Wipe the UI's existing projections of the character sheet
            Inventory.Clear();

            // 2. Ask the sheet for hydrated facts
            var refreshedItems = ActiveSheet.GetHydratedInventory();

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
            if (ActiveSheet is null)
            {
                InitializeNewCharacter();
            }
        }

        public void InitializeNewCharacter()
        {
            _characterStateService.CreateNewCharacter();
            RebuildState();
        }
    }
}
