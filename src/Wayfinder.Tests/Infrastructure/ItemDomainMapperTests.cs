using NUnit.Framework;
using Wayfinder.Infrastructure.DTOs;
using Wayfinder.Infrastructure.Mappers;

namespace Wayfinder.Tests.Infrastructure;

[TestFixture]
public class ItemDomainMapperTests
{
    private ItemDomainMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new ItemDomainMapper();
    }

    // --- HELPER METHOD ---
    private ItemYamlDto CreateBaseItemDto(string type)
    {
        return new ItemYamlDto
        {
            Id = "test-item",
            Name = "Test Item",
            Weight = 5.0,
            Cost = 100,
            ItemType = type,
            Description = "A basic test item."
        };
    }

    // ==========================================
    // SUCCESS PATHS
    // ==========================================

    [Test]
    public void Map_ValidAdventuringGear_ReturnsFullyHydratedItem()
    {
        // Arrange
        var dto = CreateBaseItemDto("AdventuringGear");

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.Warnings, Is.Empty);
            Assert.That(result.HydratedItem, Is.Not.Null);
            Assert.That(result.HydratedItem!.Name, Is.EqualTo("Test Item"));
        });
    }

    [Test]
    public void Map_ValidArmor_ReturnsFullyHydratedItem()
    {
        // Arrange
        var dto = CreateBaseItemDto("Armor");
        dto.Properties = new Dictionary<string, string>
        {
            { "ACP", "-2" },
            { "AC", "4" },
            { "Category", "Light" }
        };

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.HydratedItem, Is.Not.Null);
        });
    }

    [Test]
    public void Map_ValidShield_ReturnsFullyHydratedItem()
    {
        // Arrange
        var dto = CreateBaseItemDto("Shield");
        dto.Properties = new Dictionary<string, string>
        {
            { "ACP", "-1" },
            { "AC", "1" },
            { "Category", "Light" }
        };

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.HydratedItem, Is.Not.Null);
        });
    }

    [Test]
    public void Map_ValidWeapon_ReturnsFullyHydratedItem()
    {
        // Arrange
        var dto = CreateBaseItemDto("Weapon");
        dto.Properties = new Dictionary<string, string>
        {
            { "Category", "OneHanded" },
            { "Proficiency", "Martial" },
            { "DamageType", "S, P" } // Testing multiple damage types and abbreviations
        };

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.HydratedItem, Is.Not.Null);
        });
    }

    // ==========================================
    // FATAL ERRORS (Fails the Result)
    // ==========================================

    [Test]
    public void Map_MissingName_ReturnsInvalidResult()
    {
        var dto = CreateBaseItemDto("AdventuringGear");
        dto.Name = "";

        var result = _mapper.Map(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Count.EqualTo(1));
            Assert.That(result.Errors[0], Does.Contain("no Name"));
            Assert.That(result.HydratedItem, Is.Null);
        });
    }

    [Test]
    public void Map_InvalidItemTypeEnum_ReturnsInvalidResult()
    {
        var dto = CreateBaseItemDto("FakeItemType");

        var result = _mapper.Map(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Count.EqualTo(1));
            Assert.That(result.Errors[0], Does.Contain("invalid Type"));
            Assert.That(result.HydratedItem, Is.Null);
        });
    }

    // --- ARMOR SPECIFIC VALIDATION TESTS ---

    [Test]
    public void Map_ArmorWithNullProperties_ReturnsInvalidResult()
    {
        var dto = CreateBaseItemDto("Armor");
        dto.Properties = null!;

        var result = _mapper.Map(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors[0], Does.Contain("no Properties defined"));
    }

    [Test]
    public void Map_ArmorMissingACP_ReturnsInvalidResult()
    {
        var dto = CreateBaseItemDto("Armor");
        dto.Properties = new Dictionary<string, string> { { "AC", "4" } }; // Missing ACP

        var result = _mapper.Map(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors[0], Does.Contain("missing the 'ACP' property"));
    }

    // --- SHIELD SPECIFIC VALIDATION TESTS ---

    [Test]
    public void Map_ShieldMissingACP_ReturnsInvalidResult()
    {
        var dto = CreateBaseItemDto("Shield");
        dto.Properties = new Dictionary<string, string> { { "Category", "Heavy" } }; // Missing ACP

        var result = _mapper.Map(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors[0], Does.Contain("missing the 'ACP' property"));
    }

    [Test]
    public void Map_ShieldInvalidCategory_ReturnsInvalidResult()
    {
        var dto = CreateBaseItemDto("Shield");
        dto.Properties = new Dictionary<string, string>
        {
            { "ACP", "-2" },
            { "Category", "FakeShieldType" } // Invalid Enum
        };

        var result = _mapper.Map(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors[0], Does.Contain("invalid Category"));
    }

    // --- WEAPON SPECIFIC VALIDATION TESTS ---

    [Test]
    public void Map_WeaponMissingCategory_ReturnsInvalidResult()
    {
        var dto = CreateBaseItemDto("Weapon");
        dto.Properties = new Dictionary<string, string>
        {
            { "Proficiency", "Simple" }
            // Category missing
        };

        var result = _mapper.Map(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors[0], Does.Contain("missing the 'Category' property"));
    }

    [Test]
    public void Map_WeaponInvalidCategory_ReturnsInvalidResult()
    {
        var dto = CreateBaseItemDto("Weapon");
        dto.Properties = new Dictionary<string, string>
        {
            { "Category", "SuperHeavy" } // Invalid Enum
        };

        var result = _mapper.Map(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors[0], Does.Contain("invalid Category"));
    }

    [Test]
    public void Map_WeaponInvalidProficiency_ReturnsInvalidResult()
    {
        var dto = CreateBaseItemDto("Weapon");
        dto.Properties = new Dictionary<string, string>
        {
            { "Category", "Light" },
            { "Proficiency", "SuperMaster" } // Invalid Enum
        };

        var result = _mapper.Map(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors[0], Does.Contain("invalid Proficiency"));
    }

    [Test]
    public void Map_WeaponInvalidDamageType_ReturnsInvalidResult()
    {
        var dto = CreateBaseItemDto("Weapon");
        dto.Properties = new Dictionary<string, string>
        {
            { "Category", "OneHanded" },
            { "Proficiency", "Simple" },
            { "DamageType", "B, X" } // 'X' is invalid
        };

        var result = _mapper.Map(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors[0], Does.Contain("invalid DamageType: 'X'"));
    }
}
