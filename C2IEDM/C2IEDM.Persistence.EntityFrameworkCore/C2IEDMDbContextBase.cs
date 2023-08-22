using Microsoft.EntityFrameworkCore;
using C2IEDM.Domain.Entities.Geometry;

namespace C2IEDM.Persistence.EntityFrameworkCore;

public class C2IEDMDbContextBase : DbContext
{
    public DbSet<Location> Locations { get; set; }
    public DbSet<Point> Points { get; set; }
    public DbSet<AbsolutePoint> AbsolutePoints { get; set; }

    protected override void OnModelCreating(
        ModelBuilder builder)
    {
    }
}