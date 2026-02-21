using Wayfinder.Core.DataServices;
using Wayfinder.Core.Domain.Constants;
using Wayfinder.Core.Domain.Models.Characters;

namespace Wayfinder.Core.Rules.Services
{
    public interface IBabCalculator
    {
        int Calculate(IEnumerable<ClassLevel> levels);
    }

    /// <summary>
    /// Returns the current total BAB based on class levels.
    /// - The default method totals each class then rounds down, before adding to the total
    /// - The Fractional method totals all levels together, before rounding down at the end.
    /// </summary>
    public class BabCalculator : IBabCalculator
    {
        private readonly IClassRegistry _classRegistry;

        public BabCalculator(IClassRegistry classRegistry)
        {
            _classRegistry = classRegistry;
        }

        public int Calculate(IEnumerable<ClassLevel> levels)
        {
            if (levels == null || !levels.Any()) return 0;

            int totalBab = 0;

            // Group class from class level by class name
            var classGroups = levels
                .Where(l => l.Class != null)
                .GroupBy(l => l.Class!.Name);

            foreach (var group in classGroups)
            {
                var currentClass = _classRegistry.GetClass(group.Key);
                int classLevelCount = group.Count();
                int classBab = 0;

                // NOTE: when we implement Fractional calculations, this
                // will change. Original calc are rounded down per class
                switch (currentClass.BabRate)
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
