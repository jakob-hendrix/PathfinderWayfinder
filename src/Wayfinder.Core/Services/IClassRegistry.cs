using Wayfinder.Core.Domain.Models.Characters;

namespace Wayfinder.Core.DataServices
{
    public interface IClassRegistry
    {
        BaseCharacterClass GetClass(string className);
    }
}
