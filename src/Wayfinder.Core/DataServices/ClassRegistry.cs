using Wayfinder.Core.Domain.Data.Classes;
using Wayfinder.Core.Domain.Models;

namespace Wayfinder.Core.DataServices
{
    public class ClassRegistry : IClassRegistry
    {
        private static readonly Dictionary<string, BaseCharacterClass> _classes = new()
        {
            { "Fighter", new FighterClass()  },
            { "Rogue", new RogueClass()  },
            { "Wizard", new WizardClass()  },
            // Add more classes here as they are implemented
        };

        public BaseCharacterClass GetClass(string name)
        {
            return _classes.TryGetValue(name, out var characterClass) ? characterClass : throw new Exception($"Class {name} not found in class registry");
        }
    }
}
