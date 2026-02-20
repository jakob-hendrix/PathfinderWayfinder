using Wayfinder.Core.Domain.Constants;

namespace Wayfinder.Core.Domain.Models
{
    public class CharacterClass
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int HitDie { get; set; }

        public BabProgressionRate BabRate { get; set; }
        public SaveProgressionRate FortitudeRate { get; set; }
        public SaveProgressionRate WisdomRate { get; set; }
        public SaveProgressionRate ReflexRate { get; set; }

    }
}
