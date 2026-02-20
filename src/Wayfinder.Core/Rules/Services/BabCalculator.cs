using Wayfinder.Core.Domain.Constants;
using Wayfinder.Core.Domain.Models;

namespace Wayfinder.Core.Rules.Services
{
    public interface IBabCalculator
    {
        int Calculate(IEnumerable<ClassLevel> levels);
    }

    public class BabCalculator : IBabCalculator
    {
        public int Calculate(IEnumerable<ClassLevel> levels)
        {
            if (levels == null || !levels.Any()) return 0;

            return levels.GroupBy(l => l.Class.Name)
                .Select(group =>
                {
                    var currentClass = group.First().Class;
                    int classLevelCount = group.Count();
                    return currentClass?.BabRate switch
                    {
                        BabProgressionRate.Fast => classLevelCount,
                        BabProgressionRate.Medium => (int)Math.Floor(classLevelCount * 0.75),
                        BabProgressionRate.Slow => (int)Math.Floor(classLevelCount * 0.5),
                        _ => 0
                    };
                }).Sum();
        }
    }
}
