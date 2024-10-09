using Microsoft.EntityFrameworkCore;
using PR.Persistence.EntityFrameworkCore;

namespace PR.Persistenec.EntityFrameworkCore.InMemory
{
    public class PRDbContext : PRDbContextBase
    {
        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Dummy");
        }
    }
}