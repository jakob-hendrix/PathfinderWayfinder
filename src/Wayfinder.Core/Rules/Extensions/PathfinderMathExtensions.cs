namespace Wayfinder.Core.Rules.Extensions
{
    public static class PathfinderMathExtensions
    {
        public static int ToModifier(this int score)
        {
            return (int)Math.Floor((Math.Max(0, score) - 10) / 2.0);
        }
    }
}
