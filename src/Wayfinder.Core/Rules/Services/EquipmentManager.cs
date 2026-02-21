using Wayfinder.Core.Domain.Constants;

namespace Wayfinder.Core.Rules.Services
{
    public interface IEquipmentManager
    {
        //int GetTotalCarriedWeight(CharacterSheet sheet);
        EncumbranceLevel GetEncumbrance(int totalCarriedWeight, int strength);
    }

    public class EquipmentManager
    {
    }
}
