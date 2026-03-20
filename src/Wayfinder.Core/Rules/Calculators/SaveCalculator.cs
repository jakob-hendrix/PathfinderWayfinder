using Wayfinder.Core.Enums;
using Wayfinder.Core.Models.Characters;

namespace Wayfinder.Core.Rules.Calculators
{
    public static class SaveCalculator
    {
        public static int Calculate(IEnumerable<HydratedClassLevel> levels, SaveType saveType, int abilityScore)
        {

            int totalSave = 0;
            int abilityModifier = AbilityScoreCalculator.CalculateModifier(abilityScore);

            // Account for class level progression
            if (levels != null && levels.Any())
            {

                // Group class from class level by class name
                var classGroups = levels
                    .Where(l => l.ClassName != null)
                    .GroupBy(l => l.ClassName);

                foreach (var group in classGroups)
                {
                    var currentClass = group.First().ClassDefinition;
                    int levelCount = group.Count();

                    var rate = saveType switch
                    {
                        SaveType.Fortitude => currentClass!.FortitudeRate,
                        SaveType.Reflex => currentClass!.ReflexRate,
                        SaveType.Will => currentClass!.WillRate,
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
            }

            return totalSave + abilityModifier;
        }
    }
}
