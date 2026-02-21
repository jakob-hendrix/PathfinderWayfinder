using Wayfinder.Core.Domain.Models.Characters;

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
            // added scores from level ups (4th, 8th, 12th, 16th, 20th)
            // bonues score from items
            // bonuses/malus from buff/conditions
            return baseScore;
        }
    }
}
