using Wayfinder.Core.Domain.Constants;
using Wayfinder.Core.Domain.Models;

namespace Wayfinder.Core.Rules.Services
{
    public interface ISaveCalculator
    {
        int Calculate(IEnumerable<ClassLevel> levels, SaveType saveType);
    }
    public class SaveCalculator : ISaveCalculator
    {
        public int Calculate(IEnumerable<ClassLevel> levels, SaveType saveType)
        {
            if (levels == null || !levels.Any()) return 0;

            int totalSave = 0;

            // Group class from class level by class name
            var classGroups = levels
                .Where(l => l.Class != null)
                .GroupBy(l => l.Class!.Name);

            foreach (var group in classGroups)
            {
                var currentClass = group.First().Class!;
                int levelCount = group.Count();

                var rate = saveType switch
                {
                    SaveType.Fortitude => currentClass.FortitudeRate,
                    SaveType.Reflex => currentClass.ReflexRate,
                    SaveType.Will => currentClass.WillRate,
                    _ => throw new ArgumentException("Invalid save type")

                };


                if (rate == SaveProgressionRate.Fast)
                {
                    // Fast progression is 1/2 per level, rounded down
                    // It also gains a bonus +2 once per class
                    //
                    // NOTE: for fractional save, the bonus +2 is applied
                    // only once per save, but base rules have it applied
                    // per class
                    totalSave += 2 + (int)Math.Floor(levelCount / 2.0);
                }
                else
                {
                    // Slow progression is 1/3 per level, rounded down
                    totalSave += (int)Math.Floor(levelCount / 3.0);
                }
            }

            return totalSave;
        }
    }
}
