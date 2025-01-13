using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PR.Persistence.EntityFrameworkCore;
using PR.Domain.Entities.PR;

namespace PR.Web.Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(
            DbContextOptions options) : base(options)
        {
        }

        public DbSet<Person> People { get; set; }
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