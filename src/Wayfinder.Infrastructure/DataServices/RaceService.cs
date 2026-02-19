using Microsoft.EntityFrameworkCore;
using Wayfinder.Core.DataServices;
using Wayfinder.Core.Domain.Models;
using Wayfinder.Infrastructure.Persistence;

namespace Wayfinder.Infrastructure.DataServices
{
    public class RaceService : IRaceService
    {
        private readonly WayfinderDbContext _context;

        public RaceService(WayfinderDbContext context)
        {
            _context = context;
        }

        public async Task<List<Race>> GetAllRacesAsync()
        {
            return await _context.Races
                .Include(r => r.AbilityModifiers)
                .ToListAsync();
        }
    }
}
