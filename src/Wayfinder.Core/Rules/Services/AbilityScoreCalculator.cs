using Wayfinder.Core.Domain.Models;

namespace Wayfinder.Tests.Core
{
    public interface IAbilityScoreCalculator
    {
        int Calculate(int baseScore, List<ClassLevel> levels);
    }

    public class AbilityScoreCalculator : IAbilityScoreCalculator
    {
        public int Calculate(int baseScore, List<ClassLevel> levels)
        {
            // TODO: add logic
            // added scores from level ups
            // bonues score from items
            // bonuses/malus from buff/conditions
            return baseScore;
        }
    }
}
