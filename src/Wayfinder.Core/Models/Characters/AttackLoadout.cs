namespace Wayfinder.Core.Models.Characters;

public class AttackLoadout
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "New Loadout";

    // Pointers to the ItemEntity / ItemInstance IDs
    public Guid? MainHandItemId { get; set; }
    public Guid? OffHandItemId { get; set; }

    // Critical for Pathfinder math: Are they gripping a 1H weapon with two hands?
    public bool IsTwoHandingMainWeapon { get; set; }

    // Is this the loadout the character is currently holding?
    public bool IsActive { get; set; }
}
