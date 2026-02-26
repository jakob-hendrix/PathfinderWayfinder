using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.Core.DomainModels.Characters;
using Wayfinder.Core.Services;
using Wayfinder.Infrastructure.DataSeeders;

namespace Wayfinder.App.Services
{
    public partial class CharacterStateService : ObservableObject
    {
        private readonly IPathfinderRulesEngine _engine;
        private readonly IPathfinderDataLibrary _dataLibrary;
        private readonly SampleCharacterSeeder _characterSeeder;

        public CharacterStateService(IPathfinderRulesEngine engine, IPathfinderDataLibrary dataLibrary, SampleCharacterSeeder characterSeeder)
        {
            _engine = engine;
            _dataLibrary = dataLibrary;
            _characterSeeder = characterSeeder;

            if (_activeCharacter == null)
                _activeCharacter = _characterSeeder.BuildSampleCharacter();

            this.LoadCharacter(_activeCharacter);
        }

        // The living facts
        [ObservableProperty]
        private CharacterEntity? _activeCharacter;

        // The living domain
        [ObservableProperty]
        private CharacterSheet? _activeSheet;

        public void LoadCharacter(CharacterEntity entity)
        {
            ActiveCharacter = entity;
            ActiveSheet = new CharacterSheet(entity, _engine);
            RefreshDomain();
        }

        private void RefreshDomain()
        {
            if (ActiveSheet == null) return;

            ActiveSheet.Refresh();
            OnStateChanged?.Invoke();
        }

        // Fired when our base character is modified, letting other view know to update
        // their math
        public event Action? OnStateChanged;

        public void NotifyStateChanged() => OnStateChanged?.Invoke();
    }
}
