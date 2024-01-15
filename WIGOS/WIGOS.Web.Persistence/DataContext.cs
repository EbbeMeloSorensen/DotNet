using WIGOS.Domain.Entities;
using WIGOS.Domain.Entities.Geometry;
using WIGOS.Domain.Entities.Geometry.CoordinateSystems;
using WIGOS.Domain.Entities.Geometry.Locations;
using WIGOS.Domain.Entities.Geometry.Locations.GeometricVolumes;
using WIGOS.Domain.Entities.Geometry.Locations.Line;
using WIGOS.Domain.Entities.Geometry.Locations.Points;
using WIGOS.Domain.Entities.Geometry.Locations.Surfaces;
using WIGOS.Domain.Entities.ObjectItems;
using WIGOS.Domain.Entities.ObjectItems.Organisations;
using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using WIGOS.Domain.Entities.WIGOS.GeospatialLocations;
using WIGOS.Persistence.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Web.Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Person> People { get; set; }

        // Locations
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
        public DbSet<Domain.Entities.Geometry.Locations.Points.Point> Points { get; set; }
        public DbSet<PolyArcArea> PolyArcAreas { get; set; }
        public DbSet<PolygonArea> PolygonAreas { get; set; }
        public DbSet<RelativePoint> RelativePoints { get; set; }
        public DbSet<SphereVolume> SphereVolumes { get; set; }
        public DbSet<Surface> Surfaces { get; set; }
        public DbSet<SurfaceVolume> SurfaceVolumes { get; set; }
        public DbSet<TrackArea> TrackAreas { get; set; }
        public DbSet<VerticalDistance> VerticalDistances { get; set; }

        // Object Items
        public DbSet<ObjectItem> ObjectItems { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Unit> Units { get; set; }

        // WIGOS
        public DbSet<AbstractEnvironmentalMonitoringFacility> AbstractEnvironmentalMonitoringFacilities { get; set; }
        public DbSet<ObservingFacility> ObservingFacilities { get; set; }

        public DbSet<GeospatialLocation> GeoSpatialLocations { get; set; }
        public DbSet<Domain.Entities.WIGOS.GeospatialLocations.Point> Points_WIGOS { get; set; }

        protected override void OnModelCreating(
            ModelBuilder builder)
        {
            WIGOSDbContextBase.Configure(builder);

            base.OnModelCreating(builder);
        }
    }
}
