using Wayfinder.Core.Models.Characters;

namespace Wayfinder.Infrastructure.Persistence
{
    public interface ICharacterStorage
    {
        Task SaveAsync(CharacterEntity character);
        Task<CharacterEntity> LoadAsync(string filePath);
    }
}
