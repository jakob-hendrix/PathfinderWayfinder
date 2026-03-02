using Wayfinder.Core.Models.Characters;

namespace Wayfinder.Core.Rules.Calculators
{
    //public interface IAbilityScoreCalculator
    //{
    //    int CalculateCurrentValue(int baseScore, List<ClassLevel> levels);
    //    int CalculateBonus(int score);
    //}

    //public static class AbilityScoreCalculator : IAbilityScoreCalculator
    public static class AbilityScoreCalculator
    {
        public static int CalculateModifier(int score)
        {
            return (int)Math.Floor((Math.Max(0, score) - 10) / 2.0);
        }

        public static int CalculateCurrentValue(int baseScore, List<HydratedClassLevel> levels)
        {
            // TODO: add logic
            // added scores from level ups (4th, 8th, 12th, 16th, 20th)
            // bonues score from items
            // bonuses/malus from buff/conditions
            return baseScore;
        }
    }
}
