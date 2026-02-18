using Wayfinder.Core.Domain.Models;

namespace Wayfinder.App.Services
{
    public class CharacterSessionService
    {
        public CharacterSheet? CurrentCharacter { get; private set; }

        // Notify pages when the character sheet changes
        public event Action? OnChange;

        public void LoadCharacter(CharacterSheet character)
        {
            CurrentCharacter = character;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
