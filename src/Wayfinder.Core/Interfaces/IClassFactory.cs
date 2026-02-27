using Wayfinder.Core.Models.Characters;

namespace Wayfinder.Core.Interfaces
{
    public interface IClassFactory
    {
        BaseCharacterClass GetClass(string className);
    }
}
