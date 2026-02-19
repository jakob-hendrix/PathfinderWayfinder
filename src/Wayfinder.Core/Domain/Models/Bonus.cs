using Wayfinder.Core.Domain.Constants;

namespace Wayfinder.Core.Domain.Models
{
    public class Bonus
    {
        public BonusType Type { get; set; }
        public int Value { get; set; } = 0;
        public string Source { get; set; } = "Unknown";

        // add a bool to force stacking?
        public bool IsStackable
        {
            get
            {
                switch (this.Type)
                {
                    case BonusType.Circumstance or BonusType.Dodge or BonusType.Untyped:
                        return true;
                    default:
                        return false;
                }
            }
        }
    }
}
