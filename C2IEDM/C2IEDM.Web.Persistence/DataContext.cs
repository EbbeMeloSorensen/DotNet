using C2IEDM.Domain.Entities;
using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Domain.Entities.Geometry.CoordinateSystems;
using C2IEDM.Domain.Entities.Geometry.Locations;
using C2IEDM.Domain.Entities.Geometry.Locations.GeometricVolumes;
using C2IEDM.Domain.Entities.Geometry.Locations.Line;
using C2IEDM.Domain.Entities.Geometry.Locations.Points;
using C2IEDM.Domain.Entities.Geometry.Locations.Surfaces;
using C2IEDM.Persistence.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Web.Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Person> People { get; set; }

        public DbSet<AbsolutePoint> AbsolutePoints { get; set; }
        public DbSet<ConeVolume> ConeVolumes { get; set; }
        public DbSet<CoordinateSystem> CoordinateSystems { get; set; }
        public DbSet<CorridorArea> CorridorAreas { get; set; }
        public DbSet<Ellipse> Ellipses { get; set; }
        public DbSet<FanArea> FanAreas { get; set; }
        public DbSet<GeometricVolume> GeometricVolumes { get; set; }
        public DbSet<LinePoint> LinePoints { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<OrbitArea> OrbitAreas { get; set; }
        public DbSet<PointReference> PointReferences { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<PolyArcArea> PolyArcAreas { get; set; }
        public DbSet<PolygonArea> PolygonAreas { get; set; }
        public DbSet<RelativePoint> RelativePoints { get; set; }
        public DbSet<SphereVolume> SphereVolumes { get; set; }
        public DbSet<Surface> Surfaces { get; set; }
        public DbSet<SurfaceVolume> SurfaceVolumes { get; set; }
        public DbSet<TrackArea> TrackAreas { get; set; }
        public DbSet<VerticalDistance> VerticalDistances { get; set; }

        protected override void OnModelCreating(
            ModelBuilder builder)
        {
            C2IEDMDbContextBase.Configure(builder);

            base.OnModelCreating(builder);
        }
    }
}
