using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.App.Services;
using Wayfinder.Core.DomainModels.Stats;
using Wayfinder.Core.Extensions;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Items;
using Wayfinder.Core.Rules.Calculators;

namespace Wayfinder.App.ViewModels
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

        #region Race
        public HydratedRace? CurrentRace => ActiveSheet?.Race;
        public bool HasRace => CurrentRace != null;
        public string RaceFullTitle => CurrentRace != null
                ? $"{CurrentRace.Name}{(CurrentRace.Subrace != null ? $" ({CurrentRace.Subrace.Name})" : "")}"
                : "No Race Selected";

        public IEnumerable<RacialTrait> ActiveTraits =>
            CurrentRace?.SelectedRacialTraits ?? Enumerable.Empty<RacialTrait>();
        #endregion

        #region Demographics
        public string Alignment => SafeGet(
            () => ActiveSheet?.BaseCharacter.Alignment.ToString().SplitCamelCase(),
            fallback: "No Alignment Selected");

        public string Gender => SafeGet(() => ActiveSheet?.BaseCharacter.Gender, fallback: "Not Specified");
        public string Deity => SafeGet(() => ActiveSheet?.BaseCharacter.Deity, fallback: "None");
        public int Age => SafeGet(() => ActiveSheet?.BaseCharacter.Age);
        public string PhysicalDescription => SafeGet(() => ActiveSheet?.BaseCharacter.PhysicalDescription);
        #endregion

        #region Ability Scores
        public ModifiableStat Strength => SafeGet(() => ActiveSheet?.Strength, StatNames.Strength);
        public ModifiableStat Dexterity => SafeGet(() => ActiveSheet?.Dexterity, StatNames.Dexterity);
        public ModifiableStat Constitution => SafeGet(() => ActiveSheet?.Constitution, StatNames.Constitution);
        public ModifiableStat Intelligence => SafeGet(() => ActiveSheet?.Intelligence, StatNames.Intelligence);
        public ModifiableStat Wisdom => SafeGet(() => ActiveSheet?.Wisdom, StatNames.Wisdom);
        public ModifiableStat Charisma => SafeGet(() => ActiveSheet?.Charisma, StatNames.Charisma);
        #endregion

        #region Combat Stats
        public int BaseAttackBonus => SafeGet(() => ActiveSheet?.BaseAttackBonus);
        public AcCalculationResult ArmorClass
        {
            get
            {
                if (ActiveSheet != null) return ActiveSheet.ArmorClass;
                return ArmorClassCalculator.Calculate(
                    10,
                    Array.Empty<ItemInstance>(),
                    Array.Empty<ActiveEffect>()
                );
            }
        }
        #endregion

        #region Saves
        public ModifiableStat Fortitude => SafeGet(() => ActiveSheet?.Fortitude, StatNames.Fortitude);
        public ModifiableStat Reflex => SafeGet(() => ActiveSheet?.Reflex, StatNames.Reflex);
        public ModifiableStat Will => SafeGet(() => ActiveSheet?.Will, StatNames.Will);
        #endregion

        #region Hit Points
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

        #region Movement
        public ModifiableStat LandSpeed => SafeGet(() => ActiveSheet?.LandSpeed, StatNames.LandSpeed);
        public ModifiableStat FlySpeed => SafeGet(() => ActiveSheet?.FlySpeed, StatNames.FlySpeed);
        public ModifiableStat ClimbSpeed => SafeGet(() => ActiveSheet?.ClimbSpeed, StatNames.ClimbSpeed);
        public ModifiableStat SwimSpeed => SafeGet(() => ActiveSheet?.SwimSpeed, StatNames.SwimSpeed);

        // 1. Add a property to expose the multiplier to the UI
        public int RunMultiplier => SpeedCalculator.CalculateRunMultiplier(ActiveSheet!.CurrentEncumbrance);

        // 2. Update RunSpeed to use the dynamic multiplier instead of the hardcoded 4
        public int RunSpeed => (LandSpeed?.Total ?? 0) * RunMultiplier;
        #endregion

        #region Encumbrance
        public string CurrentEncumbrance => ActiveSheet!.CurrentEncumbrance.ToString();
        public double TotalCarriedWeight => ActiveSheet!.TotalCarriedWeight;

        public int LightLoadLimit => ActiveSheet!.LightLoadLimit;
        public int MediumLoadLimit => ActiveSheet!.MediumLoadLimit;
        public int MaxCarryCapacity => ActiveSheet!.MaxCarryCapacity; // This is the Heavy limit

        public int LiftOverHead => ActiveSheet!.LiftOverHead;
        public int LiftOffGround => ActiveSheet!.LiftOffGround;
        public int PushOrDrag => ActiveSheet!.PushOrDrag;
        #endregion

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

        #region Helper Methods
        private ModifiableStat SafeGet(Func<ModifiableStat?> selector, string statName, [CallerMemberName] string propertyName = "")
        {
            try
            {
                return selector() ?? new ModifiableStat { Name = statName };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed getting stat in property {propertyName}", ex);
                return new ModifiableStat { Name = $"{statName} (Error)" };
            }
        }

        private T SafeGet<T>(Func<T?> selector, T fallback = default, [CallerMemberName] string propertyName = "") where T : struct
        {
            try
            {
                return selector() ?? fallback;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed getting value in property {propertyName}", ex);
                return fallback;
            }
        }

        private string SafeGet(Func<string?> selector, string fallback = "", [CallerMemberName] string propertyName = "")
        {
            try
            {
                return selector() ?? fallback;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed getting string in property {propertyName}", ex);
                return fallback;
            }
        }
        #endregion

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

            // Do inventory stuff

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
