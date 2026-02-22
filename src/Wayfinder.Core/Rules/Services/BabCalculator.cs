using Wayfinder.Core.DomainModels.Characters;
using Wayfinder.Core.Enums;
using Wayfinder.Core.Services;

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
        private readonly IClassFactory _classFactory;

        public BabCalculator(IClassFactory classFactory)
        {
            _classFactory = classFactory;
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
                var currentClass = _classFactory.GetClass(group.Key);
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
