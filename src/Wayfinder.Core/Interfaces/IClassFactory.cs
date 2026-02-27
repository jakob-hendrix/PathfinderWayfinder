using Wayfinder.Core.DomainModels.Characters;

namespace Wayfinder.Core.Interfaces
{
    public interface IClassFactory
    {
        BaseCharacterClass GetClass(string className);
    }
}
