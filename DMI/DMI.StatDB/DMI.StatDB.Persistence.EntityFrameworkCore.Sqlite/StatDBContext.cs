using Microsoft.EntityFrameworkCore;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Persistence.EntityFrameworkCore.Sqlite.EntityConfigurations;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.Sqlite
{
    public class StatDBContext : DbContext
    {
        public DbSet<Station> Stations { get; set; }
        public DbSet<Position> Positions { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString();
            optionsBuilder.UseSqlite(connectionString);
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StationConfiguration());
            modelBuilder.ApplyConfiguration(new PositionConfiguration());

            modelBuilder.Entity<Position>()
                .HasOne(p => p.Station)
                .WithMany(s => s.Positions)
                .HasForeignKey(p => p.StatID);
        }
    }
}
