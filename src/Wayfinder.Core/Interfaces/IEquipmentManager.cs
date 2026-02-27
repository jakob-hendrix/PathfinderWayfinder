using Wayfinder.Core.DomainModels.Characters;
using Wayfinder.Core.Enums;

namespace Wayfinder.Core.Interfaces
{
    public interface IEquipmentManager
    {
        double GetTotalCarriedWeight(CharacterEntity entity);
        EncumbranceLevel GetEncumbrance(int totalCarriedWeight, int strength);
    }
}
