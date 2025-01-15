using Microsoft.EntityFrameworkCore;
using PR.Domain.Entities.Smurfs;
using PR.Domain.Entities.C2IEDM.ObjectItems;
using PR.Domain.Entities.C2IEDM.ObjectItems.Organisations;
using PR.Domain.Entities.C2IEDM.Geometry.Locations;
using PR.Domain.Entities.C2IEDM.Geometry.Locations.Points;
using PR.Domain.Entities.C2IEDM.Geometry.Locations.Line;
using PR.Domain.Entities.C2IEDM.Geometry.Locations.Surfaces;
using PR.Domain.Entities.PR;
using PR.Persistence.EntityFrameworkCore.EntityConfigurations;
using PR.Persistence.EntityFrameworkCore.EntityConfigurations.C2IEDM.Geometry.Locations.Line;
using PR.Domain.Entities.C2IEDM.Geometry;

namespace PR.Persistence.EntityFrameworkCore
{
    public class PRDbContextBase : DbContext
    {
        public static bool Versioned { get; set; }

        // This constructor is necessary when making a migration with the Package Manager console, since we don't set the static property Versioned when
        // making a migration
        static PRDbContextBase()
        {
            Versioned = true;
        }

        public DbSet<Smurf> Smurfs { get; set; }

        public DbSet<Person> People { get; set; }
        public DbSet<PersonComment> PersonComments { get; set; }

        // C2IEDM - Geometry
        public DbSet<Location> Locations { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<AbsolutePoint> AbsolutePoints { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<LinePoint> LinePoints { get; set; }
        public DbSet<Surface> Surfaces { get; set; }
        public DbSet<Ellipse> Ellipses { get; set; }
        public DbSet<FanArea> FanAreas { get; set; }
        public DbSet<VerticalDistance> VerticalDistances { get; set; }

        // C2IEDM - ObjectItems
        public DbSet<ObjectItem> ObjectItems { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Unit> Units { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            Configure(modelBuilder);
        }

        public static void Configure(
            ModelBuilder modelBuilder)
        {
            ConfigureC2IEDM(modelBuilder);

            modelBuilder.ApplyConfiguration(new PersonConfiguration(Versioned));
            modelBuilder.ApplyConfiguration(new PersonCommentConfiguration(Versioned));

            if (Versioned)
            {
                modelBuilder.Entity<PersonComment>()
                    .HasOne(pc => pc.Person)
                    .WithMany()
                    .HasForeignKey(pc => pc.PersonArchiveID)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            }
            else
            {
                modelBuilder.Entity<PersonComment>()
                    .HasOne(pc => pc.Person)
                    .WithMany()
                    .HasForeignKey(pc => pc.PersonID)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }

        private static void ConfigureC2IEDM(
            ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LinePointConfiguration());

            modelBuilder.Entity<Location>().UseTptMappingStrategy();
            modelBuilder.Entity<ObjectItem>().UseTptMappingStrategy();

            modelBuilder.Entity<AbsolutePoint>()
                .HasOne(ap => ap.VerticalDistance)
                .WithMany()
                .HasForeignKey(ap => ap.VerticalDistanceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Line>()
                .HasMany(l => l.LinePoints)
                .WithOne(lp => lp.Line)
                .HasForeignKey(lp => lp.LineID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FanArea>()
                .HasOne(fa => fa.VertexPoint)
                .WithMany()
                .HasForeignKey(fa => fa.VertexPointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ellipse>()
                .HasOne(e => e.CentrePoint)
                .WithMany()
                .HasForeignKey(e => e.CentrePointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ellipse>()
                .HasOne(e => e.FirstConjugateDiameterPoint)
                .WithMany()
                .HasForeignKey(e => e.FirstConjugateDiameterPointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ellipse>()
                .HasOne(e => e.SecondConjugateDiameterPoint)
                .WithMany()
                .HasForeignKey(e => e.SecondConjugateDiameterPointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
