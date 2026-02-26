using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.Core.Data.Definitions;
using Wayfinder.Core.DomainModels.Characters;
using Wayfinder.Core.DomainModels.Characters.RaceModels;
using Wayfinder.Core.Enums;
using Wayfinder.Core.Services;

namespace Wayfinder.App.Services
{
    public partial class BaseCharacterViewModel : ObservableObject, IDisposable
    {
        private readonly CharacterStateService _stateService;
        private readonly IRaceLibrary _raceLibrary;

        public BaseCharacterViewModel(CharacterStateService stateService, IRaceLibrary raceLibrary)
        {
            _stateService = stateService;
            _raceLibrary = raceLibrary;

            _stateService.OnStateChanged += OnCharacterStateChanged;
        }

        private void OnCharacterStateChanged()
        {
            OnPropertyChanged(nameof(ActiveCharacter));
            OnPropertyChanged(nameof(ActiveSheet));
        }

        // Pull character from state
        public CharacterEntity? ActiveCharacter => _stateService.ActiveCharacter;

        // Pull the character sheet from state. The character state is where certain entities are
        // constructed. In this case it will give is a rebuilt Race
        public CharacterSheet? ActiveSheet => _stateService.ActiveSheet;

        public void NotifyRaceChanged()
        {
            _stateService.RefreshDomain();
        }

        #region User Choice Options
        // Expose restricted selections
        public IEnumerable<Alignment> AlignmentOptions => Enum.GetValues<Alignment>();
        public IEnumerable<RaceDefinition> AvailableRaces => _raceLibrary.GetRaceDefinitions();
        #endregion

        // Exposed Properties For the UI To Update
        #region ExposedProperties
        public string Name
        {
            get => ActiveCharacter?.Name ?? string.Empty;
            set => SetEntityProperty(() => ActiveCharacter!.Name = value, nameof(Name));
        }

        public int Age
        {
            get => ActiveCharacter?.Age ?? 18;    // get average age from race?
            set => SetEntityProperty(() => ActiveCharacter!.Age = value, nameof(Age));
        }

        public RaceChoices RaceChoices => ActiveCharacter!.RaceChoices;

        public int Weight
        {
            get => ActiveCharacter?.Weight ?? 120;    // get average weight from race?
            set => SetEntityProperty(() => ActiveCharacter!.Weight = value, nameof(Weight));
        }

        public int Height
        {
            get => ActiveCharacter?.Height ?? 66;    // get average height from race?
            set => SetEntityProperty(() => ActiveCharacter!.Height = value, nameof(Height));
        }

        public string Biography
        {
            get => ActiveCharacter?.Biography ?? string.Empty;
            set => SetEntityProperty(() => ActiveCharacter!.Biography = value, nameof(Biography));
        }

        public string PhysicalDescription
        {
            get => ActiveCharacter?.PhysicalDescription ?? string.Empty;
            set => SetEntityProperty(() => ActiveCharacter!.PhysicalDescription = value, nameof(PhysicalDescription));
        }

        public Alignment Alignment
        {
            get => ActiveCharacter?.Alignment ?? Alignment.TrueNeutral;
            set => SetEntityProperty(() => ActiveCharacter!.Alignment = value, nameof(Alignment));
        }

        public int Strength
        {
            get => ActiveCharacter?.BaseStrength ?? 10;
            set => SetEntityProperty(() => ActiveCharacter!.BaseStrength = Math.Max(0, value), nameof(Strength));
        }

        public int Dexterity
        {
            get => ActiveCharacter?.BaseDexterity ?? 10;
            set => SetEntityProperty(() => ActiveCharacter!.BaseDexterity = Math.Max(0, value), nameof(Dexterity));
        }

        public int Constitution
        {
            get => ActiveCharacter?.BaseConstitution ?? 10;
            set => SetEntityProperty(() => ActiveCharacter!.BaseConstitution = Math.Max(0, value), nameof(Constitution));
        }

        public int Intelligence
        {
            get => ActiveCharacter?.BaseIntelligence ?? 10;
            set => SetEntityProperty(() => ActiveCharacter!.BaseIntelligence = Math.Max(0, value), nameof(Intelligence));
        }

        public int Wisdom
        {
            get => ActiveCharacter?.BaseWisdom ?? 10;
            set => SetEntityProperty(() => ActiveCharacter!.BaseWisdom = Math.Max(0, value), nameof(Wisdom));
        }

        public int Charisma
        {
            get => ActiveCharacter?.BaseCharisma ?? 10;
            set => SetEntityProperty(() => ActiveCharacter!.BaseCharisma = Math.Max(0, value), nameof(Charisma));
        }
        // TODO
        // Add a language from seeded language service
        // Add a god from seeded god service
        public string Deity
        {
            get => ActiveCharacter?.Deity ?? string.Empty;
            set => SetEntityProperty(() => ActiveCharacter!.Deity = value, nameof(Deity));
        }
        #endregion

        #region Helper Methods
        private void SetEntityProperty(Action updateAction, string propertyName)
        {
            if (ActiveCharacter == null) return;

            updateAction();
            OnPropertyChanged(propertyName);
            _stateService.NotifyStateChanged();
        }

        public void Dispose()
        {
            _stateService.OnStateChanged -= OnCharacterStateChanged;
        }
        #endregion
    }
}
