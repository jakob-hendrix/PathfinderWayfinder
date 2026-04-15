using NUnit.Framework;
using Wayfinder.Core.Constants;
using Wayfinder.Core.DomainModels.Stats;
using Wayfinder.Core.Models.Characters;
using Wayfinder.Core.Models.Items;
using Wayfinder.Core.Rules.Calculators;

namespace Wayfinder.Tests.Core.Rules;

[TestFixture]
public class ArmorClassCalculatorTests
{
    private ActiveEffect CreateEffect(ModifierType type, int value, string sourceName = "Test Source") =>
        new ActiveEffect
        {
            SourceId = Guid.NewGuid(),
            SourceName = sourceName,
            TargetStatName = StatNames.AC,
            Type = type,
            Value = value
        };

    private ItemInstance CreateEquippedArmor(int? maxDex)
    {
        var armor = new ArmorItem { Name = "Test Armor", MaxDexBonus = maxDex };
        return new ItemInstance(new ItemEntity { State = ItemState.Equipped }, armor);
    }

    [Test]
    public void Calculate_NakedCommoner_Returns10()
    {
        var result = ArmorClassCalculator.Calculate(10, Array.Empty<ItemInstance>(), new List<ActiveEffect>());

        Assert.Multiple(() =>
        {
            Assert.That(result.Total, Is.EqualTo(10));
            Assert.That(result.Touch, Is.EqualTo(10));
            Assert.That(result.FlatFooted, Is.EqualTo(10));
        });
    }

    [Test]
    public void Calculate_WithDexCap_ProperlyLimitsDexterityAndLogsPenalty()
    {
        // 18 Dex = +4 Mod, but wearing an item with Max Dex +1
        var equippedItems = new List<ItemInstance> { CreateEquippedArmor(1) };
        var result = ArmorClassCalculator.Calculate(18, equippedItems, new List<ActiveEffect>());

        Assert.Multiple(() =>
        {
            Assert.That(result.AppliedDexMod, Is.EqualTo(1), "Dexterity modifier should be capped by MaxDexLimit.");
            Assert.That(result.Total, Is.EqualTo(11));

            // Verify our cool new UI audit log requirement!
            var penaltyMod = result.TotalStat.Modifiers.FirstOrDefault(m => m.SourceName == "Max Dex Limit");
            Assert.That(penaltyMod, Is.Not.Null);
            Assert.That(penaltyMod!.Value, Is.EqualTo(-3)); // +4 Base, but capped at +1 means a -3 Penalty
            Assert.That(penaltyMod.Type, Is.EqualTo(ModifierType.Penalty));
        });
    }

    [Test]
    public void Calculate_NegativeDex_AppliesToFlatFooted()
    {
        // 8 Dex = -1 Mod
        var result = ArmorClassCalculator.Calculate(8, Array.Empty<ItemInstance>(), new List<ActiveEffect>());

        Assert.That(result.FlatFooted, Is.EqualTo(9), "Negative Dexterity penalties still apply when flat-footed.");
    }

    [Test]
    public void Calculate_ArmorBonuses_DoNotStack()
    {
        var effects = new List<ActiveEffect>
        {
            CreateEffect(ModifierType.Armor, 4, "Chain Shirt"),
            CreateEffect(ModifierType.Armor, 2, "Leather Armor")
        };

        var result = ArmorClassCalculator.Calculate(10, Array.Empty<ItemInstance>(), effects);

        Assert.That(result.Total, Is.EqualTo(14), "Identical modifier types (Armor) should not stack.");
    }

    [Test]
    public void Calculate_DodgeBonuses_DoStack()
    {
        var effects = new List<ActiveEffect>
        {
            CreateEffect(ModifierType.Dodge, 1, "Dodge Feat"),
            CreateEffect(ModifierType.Dodge, 1, "Haste")
        };

        var result = ArmorClassCalculator.Calculate(10, Array.Empty<ItemInstance>(), effects);

        Assert.That(result.Total, Is.EqualTo(12), "Dodge bonuses from different sources stack.");
    }

    [Test]
    public void Calculate_FullCombatLoadout_CalculatesCorrectly()
    {
        var equippedItems = new List<ItemInstance> { CreateEquippedArmor(3) }; // Acts like a Breastplate (Max Dex 3)
        var effects = new List<ActiveEffect>
        {
            CreateEffect(ModifierType.Armor, 6, "Breastplate"),
            CreateEffect(ModifierType.Shield, 2, "Heavy Shield"),
            CreateEffect(ModifierType.Deflection, 1, "Ring of Protection"),
            CreateEffect(ModifierType.Dodge, 1, "Dodge Feat")
        };

        // 16 Dex = +3 Mod, capped at +3
        var result = ArmorClassCalculator.Calculate(16, equippedItems, effects);

        Assert.Multiple(() =>
        {
            Assert.That(result.Total, Is.EqualTo(23));
            Assert.That(result.Touch, Is.EqualTo(15));
            Assert.That(result.FlatFooted, Is.EqualTo(19));
        });
    }

    [Test]
    public void Calculate_IgnoresNonAcEffects()
    {
        var effects = new List<ActiveEffect>
        {
            CreateEffect(ModifierType.Armor, 4, "Chain Shirt"),
            new ActiveEffect
            {
                SourceId = Guid.NewGuid(),
                SourceName = "Bull's Strength",
                TargetStatName = "Strength", // Modifying Strength, not AC!
                Type = ModifierType.Enhancement,
                Value = 4
            }
        };

        var result = ArmorClassCalculator.Calculate(10, Array.Empty<ItemInstance>(), effects);

        Assert.That(result.Total, Is.EqualTo(14), "The calculator must strictly filter out effects not targeting ArmorClass.");
    }
}
