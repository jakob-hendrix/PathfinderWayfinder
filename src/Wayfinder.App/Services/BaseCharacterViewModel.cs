using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.Core.DomainModels.Characters;
using Wayfinder.Core.Enums;

namespace Wayfinder.App.Services
{
    public partial class BaseCharacterViewModel : ObservableObject
    {
        private readonly CharacterStateService _stateService;

        public BaseCharacterViewModel(CharacterStateService stateService)
        {
            _stateService = stateService;
        }

        // Pull character from state
        public CharacterEntity? Character => _stateService.ActiveCharacter;

        // Expose restricted selections
        public IEnumerable<Alignment> AlignmentOptions => Enum.GetValues<Alignment>();

        // Exposed Properties For the UI To Update
        #region ExposedProperties
        public string Name
        {
            get => Character?.Name ?? string.Empty;
            set => SetEntityProperty(() => Character!.Name = value, nameof(Name));
        }

        public int Age
        {
            get => Character?.Age ?? 18;    // get average age from race?
            set => SetEntityProperty(() => Character!.Age = value, nameof(Age));
        }

        public string Race
        {
            get => Character?.Race ?? "Human";    // get default race
            set => SetEntityProperty(() => Character!.Race = value, nameof(Race));
        }

        public int Weight
        {
            get => Character?.Weight ?? 120;    // get average weight from race?
            set => SetEntityProperty(() => Character!.Weight = value, nameof(Weight));
        }

        public int Height
        {
            get => Character?.Height ?? 66;    // get average height from race?
            set => SetEntityProperty(() => Character!.Height = value, nameof(Height));
        }

        public string Biography
        {
            get => Character?.Biography ?? string.Empty;
            set => SetEntityProperty(() => Character!.Biography = value, nameof(Biography));
        }

        public string PhysicalDescription
        {
            get => Character?.PhysicalDescription ?? string.Empty;
            set => SetEntityProperty(() => Character!.PhysicalDescription = value, nameof(PhysicalDescription));
        }

        public Alignment Alignment
        {
            get => Character?.Alignment ?? Alignment.TrueNeutral;
            set => SetEntityProperty(() => Character!.Alignment = value, nameof(Alignment));
        }

        public int Strength
        {
            get => Character?.BaseStrength ?? 10;
            set => SetEntityProperty(() => Character!.BaseStrength = Math.Max(0, value), nameof(Strength));
        }

        public int Dexterity
        {
            get => Character?.BaseDexterity ?? 10;
            set => SetEntityProperty(() => Character!.BaseDexterity = Math.Max(0, value), nameof(Dexterity));
        }

        public int Constitution
        {
            get => Character?.BaseConstitution ?? 10;
            set => SetEntityProperty(() => Character!.BaseConstitution = Math.Max(0, value), nameof(Constitution));
        }

        public int Intelligence
        {
            get => Character?.BaseIntelligence ?? 10;
            set => SetEntityProperty(() => Character!.BaseIntelligence = Math.Max(0, value), nameof(Intelligence));
        }

        public int Wisdom
        {
            get => Character?.BaseWisdom ?? 10;
            set => SetEntityProperty(() => Character!.BaseWisdom = Math.Max(0, value), nameof(Wisdom));
        }

        public int Charisma
        {
            get => Character?.BaseCharisma ?? 10;
            set => SetEntityProperty(() => Character!.BaseCharisma = Math.Max(0, value), nameof(Charisma));
        }
        // TODO
        // Add a language from seeded language service
        // Add a god from seeded god service
        public string Deity
        {
            get => Character?.Deity ?? string.Empty;
            set => SetEntityProperty(() => Character!.Deity = value, nameof(Deity));
        }
        #endregion

        #region Helper Methods
        private void SetEntityProperty(Action updateAction, string propertyName)
        {
            if (Character == null) return;

            updateAction();
            OnPropertyChanged(propertyName);
            _stateService.NotifyStateChanged();
        }
        #endregion
    }
}
