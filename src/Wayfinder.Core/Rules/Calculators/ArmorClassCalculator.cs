using Wayfinder.Core.Constants;
using Wayfinder.Core.DomainModels.Stats;
using Wayfinder.Core.Logic;
using Wayfinder.Core.Models.Items;

namespace Wayfinder.Core.Rules.Calculators;

public record AcCalculationResult(
    ModifiableStat TotalStat,
    ModifiableStat TouchStat,
    ModifiableStat FlatFootedStat,
    int AppliedDexMod)
{
    public int Total => TotalStat.Total;
    public int Touch => TouchStat.Total;
    public int FlatFooted => FlatFootedStat.Total;
}

public static class ArmorClassCalculator
{
    public static AcCalculationResult Calculate(int dexterityScore, IEnumerable<ItemInstance> equippedItems, IEnumerable<ActiveEffect> effects)
    {
        // 1. Convert the raw Ability Score into its modifier
        int baseDexMod = AbilityScoreCalculator.CalculateModifier(dexterityScore);

        // 2. Calculate physical Max Dex limits from equipped gear
        int? maxDexLimit = null;
        foreach (var item in equippedItems)
        {
            if (item.BaseStats is ArmorItem armor && armor.MaxDexBonus.HasValue)
            {
                maxDexLimit = maxDexLimit.HasValue ? Math.Min(maxDexLimit.Value, armor.MaxDexBonus.Value) : armor.MaxDexBonus.Value;
            }
            else if (item.BaseStats is ShieldItem shield && shield.MaxDexBonus.HasValue)
            {
                maxDexLimit = maxDexLimit.HasValue ? Math.Min(maxDexLimit.Value, shield.MaxDexBonus.Value) : shield.MaxDexBonus.Value;
            }
        }

        // 3. Calculate the Penalty (if any) caused by the armor restricting movement
        int dexPenalty = 0;
        if (maxDexLimit.HasValue && baseDexMod > maxDexLimit.Value)
        {
            dexPenalty = maxDexLimit.Value - baseDexMod; // e.g., 3 max - 5 base = -2 penalty
        }

        // 4. Convert Dex into explicit StatModifiers that the StatCalculator can process
        var totalBaseMods = new List<StatModifier> { new StatModifier("Dexterity", baseDexMod, ModifierType.Ability, true) };
        var touchBaseMods = new List<StatModifier> { new StatModifier("Dexterity", baseDexMod, ModifierType.Ability, true) };

        // Add the Max Dex limit as a visible penalty to the audit log!
        if (dexPenalty < 0)
        {
            totalBaseMods.Add(new StatModifier("Max Dex Limit", dexPenalty, ModifierType.Penalty, true));
            touchBaseMods.Add(new StatModifier("Max Dex Limit", dexPenalty, ModifierType.Penalty, true));
        }

        // Flat-footed ignores positive Dexterity bonuses entirely
        var flatBaseMods = new List<StatModifier>();
        if (baseDexMod < 0)
        {
            // But it still applies negative Dexterity (clumsiness)
            flatBaseMods.Add(new StatModifier("Dexterity", baseDexMod, ModifierType.Ability, true));
        }

        // 5. Pre-filter the ActiveEffects into the specific rulesets for Touch and Flat-Footed
        var acEffects = effects.Where(e => e.TargetStatName.Equals(StatNames.AC, StringComparison.OrdinalIgnoreCase)).ToList();

        var touchEffects = acEffects.Where(e =>
            e.Type != ModifierType.Armor &&
            e.Type != ModifierType.Shield &&
            e.Type != ModifierType.NaturalArmor).ToList();

        var flatEffects = acEffects.Where(e => e.Type != ModifierType.Dodge).ToList();

        // 6. Let the central StatCalculator handle all the heavy lifting and stacking rules!
        var totalStat = StatCalculator.Calculate(StatNames.AC, 10, acEffects, totalBaseMods);
        var touchStat = StatCalculator.Calculate(StatNames.AC, 10, touchEffects, touchBaseMods);
        var flatStat = StatCalculator.Calculate(StatNames.AC, 10, flatEffects, flatBaseMods);

        return new AcCalculationResult(totalStat, touchStat, flatStat, baseDexMod + dexPenalty);
    }
}
