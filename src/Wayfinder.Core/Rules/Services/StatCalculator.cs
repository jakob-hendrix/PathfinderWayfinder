using Wayfinder.Core.DomainModels.Common;

namespace Wayfinder.Core.Rules.Services
{
    public interface IStatCalculator
    {
        int CalculateStat(int baseStat, IEnumerable<Bonus> bonuses);
    }

    public class StatCalculator : IStatCalculator
    {
        public int CalculateStat(int baseStat, IEnumerable<Bonus> bonuses)
        {
            // Only some bonus types stack. 
            var stackableBonusesSum = bonuses
                .Where(b => b.IsStackable)
                .Sum(b => b.Value);

            // Otherwise we take the largest bonus of each type
            var unstackableBonusesSum = bonuses
                .Where(b => !b.IsStackable)
                .GroupBy(c => c.Type)
                .Select(d => d.Max(e => e.Value))
                .Sum();

            return baseStat + stackableBonusesSum + unstackableBonusesSum;
        }
    }
}
