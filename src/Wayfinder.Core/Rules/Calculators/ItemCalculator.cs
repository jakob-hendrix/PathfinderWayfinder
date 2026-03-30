namespace Wayfinder.Core.Rules.Calculators;

using System;
using System.Collections.Generic;
using System.Linq;
using Wayfinder.Core.Models.Items;

public static class InventoryCalculator
{
    /// <summary>
    /// Calculates the total carried weight, respecting nested containers and Stored status.
    /// </summary>
    public static double CalculateTotalCarriedWeight(IEnumerable<ItemInstance> inventory)
    {
        var inventoryList = inventory.ToList();
        double totalWeight = 0;

        foreach (var item in inventoryList)
        {
            if (IsEffectivelyCarried(item, inventoryList))
            {
                totalWeight += item.TotalWeight;
            }
        }

        return totalWeight;
    }

    /// <summary>
    /// Recursively checks if an item is actually on the character's person.
    /// </summary>
    private static bool IsEffectivelyCarried(ItemInstance item, List<ItemInstance> fullInventory)
    {
        // If it's explicitly stored, it's not carried.
        if (item.State == ItemState.Stored) return false;

        // If it's not in a container, and it's Carried/Equipped, it is on the person.
        if (item.ContainerId == null || item.ContainerId == Guid.Empty)
            return true;

        // It is in a container. We must check the container's state.
        var container = fullInventory.FirstOrDefault(i => i.Id == item.ContainerId);

        if (container == null)
            return true; // Orphaned item (defensive fallback)

        // Recursively check the parent container
        return IsEffectivelyCarried(container, fullInventory);
    }
}
