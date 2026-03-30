using Wayfinder.Core.Constants;

namespace Wayfinder.Core.Rules.Calculators;

public static class SizeCalculator
{
    public static SizeCategory CalculateSize(IEnumerable<ActiveEffect> effects)
    {
        var sizeEffects = effects.Where(e => e.TargetStatName.Equals("Size", StringComparison.OrdinalIgnoreCase)).ToList();

        // 1. Find the Base Size String
        var baseEffect = sizeEffects.FirstOrDefault(e => e.Type == ModifierType.Base);
        SizeCategory currentSize = SizeCategory.Medium; // Safe default

        if (baseEffect != null && !string.IsNullOrWhiteSpace(baseEffect.StringValue))
        {
            if (Enum.TryParse<SizeCategory>(baseEffect.StringValue, true, out var parsedSize))
            {
                currentSize = parsedSize;
            }
        }

        // 2. Apply Size Alterations (e.g., Enlarge Person = Value: 1)
        int sizeShifts = sizeEffects
            .Where(e => e.Type != ModifierType.Base)
            .Sum(e => e.Value);

        if (sizeShifts != 0)
        {
            int newSizeInt = (int)currentSize + sizeShifts;

            // Clamp it so a Tiny creature shrinking 4 times doesn't crash the app
            newSizeInt = Math.Clamp(newSizeInt, (int)SizeCategory.Fine, (int)SizeCategory.Colossal);

            currentSize = (SizeCategory)newSizeInt;
        }

        return currentSize;
    }
}
