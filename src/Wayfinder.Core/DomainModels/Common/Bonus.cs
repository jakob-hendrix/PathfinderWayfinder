using Wayfinder.Core.Enums;

namespace Wayfinder.Core.DomainModels.Common
{
    public class Bonus
    {
        public BonusType Type { get; set; }
        public int Value { get; set; } = 0;
        public string Source { get; set; } = "Unknown";

        //public Bonus(BonusType type, int value, string source)
        //{
        //    Type = type;
        //    Value = value;
        //    Source = source;
        //}

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
