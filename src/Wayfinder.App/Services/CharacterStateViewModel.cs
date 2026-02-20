using CommunityToolkit.Mvvm.ComponentModel;
using Wayfinder.Core.Domain.Models;

namespace Wayfinder.App.Services
{
    public partial class CharacterStateViewModel : ObservableObject
    {
        // Holds the main state of the character, including all calculated values, inventory, etc
        private CharacterSheet _sheet;

        [ObservableProperty]
        private string _characterName;

        public int Strength => _sheet.Strength;
        // TODO do others

        // Trick to trigger a change in all properties, to handle the ripple of Pathfinder changes
        // Let's see how much lag this introduces...
        public void Refresh()
        {
            OnPropertyChanged(string.Empty);
        }

    }
}
