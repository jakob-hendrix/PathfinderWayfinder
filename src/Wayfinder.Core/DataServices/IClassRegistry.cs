using Wayfinder.Core.Domain.Models;

namespace Wayfinder.Core.DataServices
{
    public interface IClassRegistry
    {
        BaseCharacterClass GetClass(string className);
    }
}
