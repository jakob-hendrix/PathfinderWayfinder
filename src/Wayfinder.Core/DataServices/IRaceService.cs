using Wayfinder.Core.Domain.Models;

namespace Wayfinder.Core.DataServices
{
    public interface IRaceService
    {
        Task<List<Race>> GetAllRacesAsync();
    }
}
