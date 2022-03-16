using Microsoft.EntityFrameworkCore;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer.EntityConfigurations;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer
{
    public class StatDbContext : DbContext
    {
        public DbSet<Station> Stations { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString();
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StationConfiguration());
            modelBuilder.ApplyConfiguration(new PositionConfiguration());

            modelBuilder.Entity<Position>()
                .HasOne(p => p.Station)
                .WithMany(s => s.Positions);
        }
    }
}
