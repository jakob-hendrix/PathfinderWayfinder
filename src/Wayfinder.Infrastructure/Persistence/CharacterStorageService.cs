using Wayfinder.Core.Domain.Models;

namespace Wayfinder.Infrastructure.Persistence
{
    public class CharacterStorageService : ICharacterStorage
    {
        public Task<CharacterEntity> LoadAsync(string filePath) => throw new NotImplementedException();
        public Task SaveAsync(CharacterEntity character) => throw new NotImplementedException();
    }
}
