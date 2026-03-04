using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.Core.Interfaces;
using Wayfinder.Core.Models.Characters;
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

        public void RefreshDomain()
        {
            if (ActiveCharacter == null) return;

            // Trigger the sheet to recalculate it bits and bobs
            ActiveSheet?.Refresh();

            // 2. Announce to anyone listening that the factory just rebuilt the world
            StateChanged?.Invoke();
        }

        // Fired when our base character is modified, letting other view know to update
        // their math
        public event Action? StateChanged;

        public void NotifyStateChanged() => StateChanged?.Invoke();
    }
}
