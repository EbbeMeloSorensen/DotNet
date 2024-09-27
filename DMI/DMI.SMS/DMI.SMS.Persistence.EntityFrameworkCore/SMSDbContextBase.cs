using DMI.SMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DMI.SMS.Persistence.EntityFrameworkCore
{
    public class SMSDbContextBase : DbContext
    {
        public DbSet<StationInformation> StationInformations { get; set; }
        public DbSet<SensorLocation> SensorLocations { get; set; }
        public DbSet<ElevationAngles> ElevationAngles { get; set; }
    }
}