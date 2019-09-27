using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Data
{
    public class BusLocationDataContext : DbContext
    {
        public BusLocationDataContext(DbContextOptions<BusLocationDataContext> options)
            : base(options) { }

        public DbSet<Bus> Buses { get; set; }

        public DbSet<BusLocation> BusLocations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bus>().ToTable("Bus");
            modelBuilder.Entity<BusLocation>().ToTable("BusLocation");
        }
    }
}