using Wayfinder.Core.Constants;
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

        public static int CalculateCurrentValue(int baseScore, AbilityScore scoreType, List<HydratedClassLevel>? levels)
        {
            int totalScore = baseScore;

            // added scores from level ups (4th, 8th, 12th, 16th, 20th)

            if (levels != null)
            {
                // Just count the hydrated levels that explicitly increased THIS specific score
                int classLevelBumps = levels.Count(l =>
                    l.GrantsAbilityScoreIncrease &&
                    l.IncreasedAbilityScore == scoreType);

                totalScore += classLevelBumps;
            }


            // bonues score from items
            // bonuses/malus from buff/conditions
            return totalScore;
        }
    }
}
