using System.Data.Entity;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Persistence.EntityFramework.EntityConfigurations;

namespace DMI.StatDB.Persistence.EntityFramework
{
    [DbConfigurationType(
        "Craft.Persistence.EntityFramework.MSSQLServerDbConfiguration, Craft.Persistence.EntityFramework")]
    public class StatDbContext : DbContext
    {
        public virtual DbSet<Station> Stations { get; set; }

        public StatDbContext()
            : base(ConnectionStringProvider.GetConnectionString())
        {
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new StationConfiguration());
        }
    }
}
