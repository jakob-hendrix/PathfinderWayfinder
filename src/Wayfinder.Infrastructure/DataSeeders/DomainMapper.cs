using Wayfinder.Core.Data.Definitions;
using Wayfinder.Core.DomainModels.Characters.RaceModels;
using Wayfinder.Core.Extensions;
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
                Id = SetId(dto.Id, dto.Name),
                Name = dto.Name,
                Weight = dto.Weight,
                Cost = dto.Cost,
                ItemType = dto.ItemType,
                Description = dto.Description,
                URL = dto.URL,
                Properties = dto.Properties
            };
        }

        public RaceDefinition MapRaceToDomain(RaceYamlDto dto)
        {
            // Map alternative traits first, so the subraces can reference them
            var mappedAltTraits = dto.AlternativeRacialTraits.Select(altDto => new AlternativeRacialTrait
            {
                //Id = SetId(altDto.Id, altDto.Name),
                Name = altDto.Name,
                Description = altDto.Description,
                ReplacesRacialTraits = altDto.ReplacesTraitNames
            }).ToList();

            return new RaceDefinition
            {
                Id = SetId(dto.Id, dto.Name),
                Name = dto.Name,
                CreatureType = dto.CreatureType,
                Subtypes = dto.Subtypes,

                DefaultRacialTraits = dto.DefaultRacialTraits.Select(t => new RacialTrait
                {
                    //Id = SetId(t.Id, t.Name),
                    Name = t.Name,
                    Description = t.Description
                }).ToList(),

                AlternativeRacialTraits = mappedAltTraits,

                Subraces = dto.Subraces.Select(s => new Subrace
                {
                    Id = SetId(s.Id, s.Name),
                    Name = s.Name,
                    Description = s.Description,

                    // Validate and link traits by name
                    RacialTraits = s.AlternativeTraitNames
                        .Select(traitName => mappedAltTraits.FirstOrDefault(alt => alt.Name.Equals(traitName, StringComparison.OrdinalIgnoreCase))
                            ?? throw new InvalidOperationException($"Validation Error in Race '{dto.Name}': Subrace '{s.Name}' references missing alternative trait '{traitName}'."))
                        .ToList()
                }).ToList()
            };
        }

        /// <summary>
        /// Used to set an id field with either a provided id, or the name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private string SetId(string id, string name) => string.IsNullOrEmpty(id) ? name.GenerateIdFromName() : id;

        private AlternativeRacialTrait MapAlternativeRacialTrait(AlternativeRacialTraitYamlDto dto)
        {
            return new AlternativeRacialTrait
            {
                //Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                ReplacesRacialTraits = dto.ReplacesTraitNames
            };
        }
    }
}
