using Microsoft.EntityFrameworkCore;
using Craft.Logging;
using PR.Domain.Entities;

namespace PR.Persistence.EntityFrameworkCore.SqlServer
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
            var people = new List<Person>
            {
                new Person
                {
                    FirstName = "Kasper"
                },
                new Person
                {
                    FirstName = "Jesper"
                },
                new Person
                {
                    FirstName = "Jonathan"
                }
            };

            context.AddRange(people);
            context.SaveChanges();
        }
    }
}
