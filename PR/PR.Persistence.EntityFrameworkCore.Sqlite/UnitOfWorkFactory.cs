using Microsoft.EntityFrameworkCore;
using Craft.Logging;
using PR.Domain.Entities;

namespace PR.Persistence.EntityFrameworkCore.Sqlite
{
    public class UnitOfWorkFactory : UnitOfWorkFactoryBase
    {
        static UnitOfWorkFactory()
        {
            using var context = new PRDbContext();
            context.Database.EnsureCreated();

            if (context.People.Any()) return;

            SeedDatabase(context);
        }

        public override void Initialize(ILogger logger)
        {
        }

        public override Task<bool> CheckRepositoryConnection()
        {
            throw new NotImplementedException();
        }

        public override IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork(new PRDbContext());
        }

        private static void SeedDatabase(DbContext context)
        {
            var now = DateTime.UtcNow;

            var people = new List<Person>
            {
                new Person
                {
                    FirstName = "Kasper",
                    Created = now,
                },
                new Person
                {
                    FirstName = "Jesper",
                    Created = now + new TimeSpan(1),
                },
                new Person
                {
                    FirstName = "Jonathan",
                    Created = now + new TimeSpan(2),
                }
            };

            context.AddRange(people);
            context.SaveChanges();
        }
    }
}
