using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.Core.DomainModels.Characters;

namespace Wayfinder.App.Services
{
    public partial class CharacterStateService : ObservableObject
    {
        [ObservableProperty]
        private CharacterEntity? _activeCharacter;

        // Fired when our base character is modified, letting other view know to update
        // their math
        public event Action? OnStateChanged;

        public void NotifyStateChanged() => OnStateChanged?.Invoke();
    }
}
