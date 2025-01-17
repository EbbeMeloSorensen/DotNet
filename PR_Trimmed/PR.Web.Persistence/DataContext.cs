using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PR.Persistence.EntityFrameworkCore;
using PR.Domain.Entities.PR;
using PR.Domain.Entities.Smurfs;
using PR.Domain.Entities.C2IEDM.ObjectItems;

namespace PR.Web.Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(
            DbContextOptions options) : base(options)
        {
        }

        public DbSet<Smurf> Smurfs { get; set; }

        // C2IEDM - ObjectItems
        public DbSet<ObjectItem> ObjectItems { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Unit> Units { get; set; }


        public DbSet<Domain.Entities.PR.Person> People { get; set; }
        public DbSet<PersonComment> PersonComments { get; set; }

        protected override void OnModelCreating(
            ModelBuilder builder)
        {
            PRDbContextBase.Versioned = true;
            PRDbContextBase.Configure(builder);

            base.OnModelCreating(builder);
        }
    }
}