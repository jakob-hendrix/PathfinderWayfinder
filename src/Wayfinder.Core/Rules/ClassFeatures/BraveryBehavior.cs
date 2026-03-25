using Wayfinder.Core.Constants;
using Wayfinder.Core.DomainModels.Stats;
using Wayfinder.Core.Logic.Features;

public class BraveryBehavior : IFeatureBehavior
{
    public string FeatureName => "Bravery";

    public IEnumerable<ActiveEffect> GenerateEffects(int rank)
    {
        // Bravery gives a +1 per rank bonus to Will saves against Fear.
        yield return new ActiveEffect
        {
            SourceName = $"Bravery (Rank {rank})",
            TargetStatName = StatNames.Will,
            Value = rank, // The math is just a 1-to-1 of the rank
            Type = ModifierType.Untyped,
            IsConditional = true,
            ConditionDescription = "vs. Fear"
        };
    }
}
