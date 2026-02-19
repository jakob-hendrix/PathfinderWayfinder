using Microsoft.EntityFrameworkCore;
using Wayfinder.Core.Domain.Models;

namespace Wayfinder.Infrastructure.Persistence
{
    public class WayfinderDbContext : DbContext
    {
        public WayfinderDbContext(DbContextOptions<WayfinderDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Race>().OwnsMany(r => r.AbilityModifiers, a =>
            {
                a.Property(p => p.Ability).HasConversion<string>();
            });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Race> Races => Set<Race>();
        public DbSet<CharacterEntity> Characters => Set<CharacterEntity>();
    }
}
