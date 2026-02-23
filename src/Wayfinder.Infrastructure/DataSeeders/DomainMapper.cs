using Wayfinder.Core.Data.Definitions;
using Wayfinder.Infrastructure.DTOs;

namespace Wayfinder.Infrastructure.DataSeeders
{
    public class DomainMapper
    {
        public ClassDefinition MapClassToDomain(ClassYamlDto dto)
        {
            return new ClassDefinition
            {
                Name = dto.Name,
                BabRate = dto.BabRate,
                HitDie = dto.HitDie,
                SkillPointsPerLevel = dto.SkillPointsPerLevel,
                FortitudeRate = dto.FortitudeRate,
                ReflexRate = dto.ReflexRate,
                WillRate = dto.WillRate,
                Levels = dto.Levels
            };
        }

        public ItemDefinition MapItemToDomain(ItemYamlDto dto)
        {
            return new ItemDefinition
            {
                Id = dto.Id,
                Name = dto.Name,
                Weight = dto.Weight,
                Cost = dto.Cost,
                ItemType = dto.ItemType,
                Description = dto.Description,
                URL = dto.URL,
                Properties = dto.Properties
            };
        }
    }
}
