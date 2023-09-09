using Microsoft.EntityFrameworkCore;
using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Persistence.EntityFrameworkCore.EntityConfigurations;
using C2IEDM.Domain.Entities.Geometry.Locations;
using C2IEDM.Domain.Entities.Geometry.Locations.Surfaces;
using C2IEDM.Domain.Entities.Geometry.Locations.Points;
using C2IEDM.Domain.Entities.Geometry.Locations.GeometricVolumes;
using C2IEDM.Domain.Entities.Geometry.CoordinateSystems;

namespace C2IEDM.Persistence.EntityFrameworkCore;

public class C2IEDMDbContextBase : DbContext
{
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
        Configure(builder);
    }

    public static void Configure(
        ModelBuilder builder)
    {
        builder.ApplyConfiguration(new PersonConfiguration());
        builder.ApplyConfiguration(new LinePointConfiguration());

        builder.Entity<Location>().UseTptMappingStrategy();
        builder.Entity<CoordinateSystem>().UseTptMappingStrategy();

        builder.Entity<AbsolutePoint>()
            .HasOne(ap => ap.VerticalDistance)
            .WithMany()
            .HasForeignKey(ap => ap.VerticalDistanceId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ConeVolume>()
            .HasOne(cv => cv.VertexPoint)
            .WithMany()
            .HasForeignKey(cv => cv.VertexPointId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ConeVolume>()
            .HasOne(cv => cv.DefiningSurface)
            .WithMany()
            .HasForeignKey(cv => cv.DefiningSurfaceId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CorridorArea>()
            .HasOne(ca => ca.CenterLine)
            .WithMany()
            .HasForeignKey(ca => ca.CenterLineId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Ellipse>()
            .HasOne(e => e.CentrePoint)
            .WithMany()
            .HasForeignKey(e => e.CentrePointId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Ellipse>()
            .HasOne(e => e.FirstConjugateDiameterPoint)
            .WithMany()
            .HasForeignKey(e => e.FirstConjugateDiameterPointId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Ellipse>()
            .HasOne(e => e.SecondConjugateDiameterPoint)
            .WithMany()
            .HasForeignKey(e => e.SecondConjugateDiameterPointId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FanArea>()
            .HasOne(fa => fa.VertexPoint)
            .WithMany()
            .HasForeignKey(fa => fa.VertexPointId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GeometricVolume>()
            .HasOne(gv => gv.LowerVerticalDistance)
            .WithMany()
            .HasForeignKey(gv => gv.LowerVerticalDistanceId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GeometricVolume>()
            .HasOne(gv => gv.UpperVerticalDistance)
            .WithMany()
            .HasForeignKey(gv => gv.UpperVerticalDistanceId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Line>()
            .HasMany(l => l.LinePoints)
            .WithOne(lp => lp.Line)
            .HasForeignKey(lp => lp.LineId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PointReference>()
            .HasOne(pr => pr.OriginPoint)
            .WithMany()
            .HasForeignKey(pr => pr.OriginPointId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PointReference>()
            .HasOne(pr => pr.XVectorPoint)
            .WithMany()
            .HasForeignKey(pr => pr.XVectorPointId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PointReference>()
            .HasOne(pr => pr.YVectorPoint)
            .WithMany()
            .HasForeignKey(pr => pr.YVectorPointId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PolygonArea>()
            .HasOne(pa => pa.BoundingLine)
            .WithMany()
            .HasForeignKey(pa => pa.BoundingLineId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RelativePoint>()
            .HasOne(rp => rp.CoordinateSystem)
            .WithMany()
            .HasForeignKey(rp => rp.CoordinateSystemId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SphereVolume>()
            .HasOne(sv => sv.CentrePoint)
            .WithMany()
            .HasForeignKey(sv => sv.CentrePointId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SurfaceVolume>()
            .HasOne(sv => sv.DefiningSurface)
            .WithMany()
            .HasForeignKey(sv => sv.DefiningSurfaceId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<OrbitArea>()
            .HasOne(oa => oa.FirstPoint)
            .WithMany()
            .HasForeignKey(oa => oa.FirstPointId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<OrbitArea>()
            .HasOne(oa => oa.SecondPoint)
            .WithMany()
            .HasForeignKey(oa => oa.SecondPointId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PolyArcArea>()
            .HasOne(paa => paa.DefiningLine)
            .WithMany()
            .HasForeignKey(paa => paa.DefiningLineId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PolyArcArea>()
            .HasOne(paa => paa.BearingOriginPoint)
            .WithMany()
            .HasForeignKey(paa => paa.BearingOriginPointId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrackArea>()
            .HasOne(ta => ta.BeginPoint)
            .WithMany()
            .HasForeignKey(ta => ta.BeginPointId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrackArea>()
            .HasOne(ta => ta.EndPoint)
            .WithMany()
            .HasForeignKey(ta => ta.EndPointId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}