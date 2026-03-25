using Wayfinder.Core.Constants;
using Wayfinder.Core.Models.Characters;

namespace Wayfinder.Core.Rules.Calculators
{
    /// <summary>
    /// Returns the current total BAB based on class levels.
    /// - The default method totals each class then rounds down, before adding to the total
    /// - The Fractional method totals all levels together, before rounding down at the end.
    /// </summary>
    public static class BabCalculator
    {
        public static int Calculate(IEnumerable<HydratedClassLevel> levels)
        {
            if (levels == null || !levels.Any()) return 0;

            int totalBab = 0;

            // Group class from class level by class name
            var classGroups = levels
                .Where(l => l.ClassName != null)
                .GroupBy(l => l.ClassName);

            foreach (var group in classGroups)
            {
                var babRate = group.First()?.ClassDefinition?.BabRate;
                int classLevelCount = group.Count();
                int classBab = 0;

                // NOTE: when we implement Fractional calculations, this
                // will change. Original calc are rounded down per class
                switch (babRate)
                {
                    case BabProgressionRate.Fast:
                        // Fast rate = 1 BAB per level
                        classBab = classLevelCount;
                        break;
                    case BabProgressionRate.Medium:
                        // Medium rate = 3/4 BAB per level
                        classBab = (int)Math.Floor(classLevelCount * 0.75);
                        break;
                    case BabProgressionRate.Slow:
                        // Medium rate = 1/2 BAB per level
                        classBab = (int)Math.Floor(classLevelCount * 0.5);
                        break;
                    default:
                        break;
                }
                totalBab += classBab;
            }

            return totalBab;
        }
    }
}
