using Wayfinder.Core.Enums;

namespace Wayfinder.Core.DomainModels.Stats;

public record StatModifier(
    string SourceName,   // e.g., "Dexterity", "Ring of Protection +2"
    int Value,           // e.g., 3, 2
    ModifierType Type,   // e.g., Ability, Deflection
    bool IsApplied       // True if it stacked, False if crossed out by a better bonus
);
