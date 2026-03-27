using Wayfinder.Core.DataDefinitions;
using Wayfinder.Core.Extensions;
using Wayfinder.Infrastructure.DTOs;

namespace Wayfinder.Infrastructure.DataSeeders
{
    public class DomainMapper
    {
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

        /// <summary>
        /// Used to set an id field with either a provided id, or the name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private string SetId(string id, string name) => string.IsNullOrEmpty(id) ? name.GenerateIdFromName() : id;
    }
}
