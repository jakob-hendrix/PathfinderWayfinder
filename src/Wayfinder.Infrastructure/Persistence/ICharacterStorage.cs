using Wayfinder.Core.Domain.Models;

namespace Wayfinder.Infrastructure.Persistence
{
    public interface ICharacterStorage
    {
        Task SaveAsync(CharacterEntity character);
        Task<CharacterEntity> LoadAsync(string filePath);
    }
}
