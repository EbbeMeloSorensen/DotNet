using Microsoft.EntityFrameworkCore;
using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Persistence.EntityFrameworkCore.EntityConfigurations;

namespace C2IEDM.Persistence.EntityFrameworkCore;

public class C2IEDMDbContextBase : DbContext
{
    public DbSet<Location> Locations { get; set; }
    public DbSet<Point> Points { get; set; }
    public DbSet<AbsolutePoint> AbsolutePoints { get; set; }
    public DbSet<VerticalDistance> VerticalDistances { get; set; }
    public DbSet<Line> Lines { get; set; }
    public DbSet<LinePoint> LinePoints { get; set; }

    protected override void OnModelCreating(
        ModelBuilder builder)
    {
        builder.ApplyConfiguration(new LinePointConfiguration());

        builder.Entity<Location>().UseTptMappingStrategy();
        
        builder.Entity<AbsolutePoint>()
            .HasOne(ap => ap.VerticalDistance)
            .WithMany()
            .HasForeignKey(ap => ap.VerticalDistanceId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Line>()
            .HasMany(l => l.LinePoints)
            .WithOne(lp => lp.Line)
            .HasForeignKey(lp => lp.LineId)
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

        builder.Entity<PolygonArea>()
            .HasOne(pa => pa.BoundingLine)
            .WithMany()
            .HasForeignKey(pa => pa.BoundingLineId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FanArea>()
            .HasOne(fa => fa.VertexPoint)
            .WithMany()
            .HasForeignKey(fa => fa.VertexPointId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}