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
        var dto = CreateBaseItemDto("AdventuringGear"); // Assuming PathfinderEnumMapper maps this

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
            { "ArmorBonus", "4" }
        };

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.HydratedItem, Is.Not.Null);
            Assert.That(result.HydratedItem!.Properties, Contains.Key("ACP"));
        });
    }

    // ==========================================
    // FATAL ERRORS (Fails the Result)
    // ==========================================

    [Test]
    public void Map_MissingName_ReturnsInvalidResult()
    {
        // Arrange
        var dto = CreateBaseItemDto("AdventuringGear");
        dto.Name = ""; // Invalid!

        // Act
        var result = _mapper.Map(dto);

        // Assert
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
        // Arrange
        var dto = CreateBaseItemDto("FakeItemType"); // PathfinderEnumMapper should throw on this

        // Act
        var result = _mapper.Map(dto);

        // Assert
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
        // Arrange
        var dto = CreateBaseItemDto("Armor");
        dto.Properties = null!; // Explicitly nulling it out

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Count.EqualTo(1));
            Assert.That(result.Errors[0], Does.Contain("no Properties defined"));
            Assert.That(result.HydratedItem, Is.Null);
        });
    }

    [Test]
    public void Map_ArmorMissingACP_ReturnsInvalidResult()
    {
        // Arrange
        var dto = CreateBaseItemDto("Armor");
        dto.Properties = new Dictionary<string, string>
        {
            { "ArmorBonus", "4" },
            // ACP deliberately missing
            { "MaxDex", "2" }
        };

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False, "Armor without ACP must fail the mapping.");
            Assert.That(result.Errors, Has.Count.EqualTo(1));
            Assert.That(result.Errors[0], Does.Contain("missing the 'ACP' property"));
            Assert.That(result.HydratedItem, Is.Null);
        });
    }

    // ==========================================
    // WARNINGS (Graceful Degradation)
    // ==========================================

    [Test]
    public void Map_UnhandledItemType_LogsWarningButSurvives()
    {
        // Arrange
        // Assuming "Weapon" is a valid enum in PathfinderEnumMapper, 
        // but we haven't written a `ValidateWeaponProperties` case for it in the Mapper yet.
        var dto = CreateBaseItemDto("Weapon");

        // Act
        var result = _mapper.Map(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.True, "The item should survive even if there are no specific property checks.");
            Assert.That(result.Errors, Is.Empty);

            // It should log a warning so the developer knows to add a validator later
            Assert.That(result.Warnings, Has.Count.EqualTo(1));
            Assert.That(result.Warnings[0], Does.Contain("No specific property validator implemented"));

            Assert.That(result.HydratedItem, Is.Not.Null);
            Assert.That(result.HydratedItem!.ItemType, Is.EqualTo("Weapon"));
        });
    }
}
