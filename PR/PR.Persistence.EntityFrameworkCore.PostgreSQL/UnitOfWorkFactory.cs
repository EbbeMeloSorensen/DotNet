using Microsoft.EntityFrameworkCore;
using Craft.Logging;
using PR.Domain.Entities;

namespace PR.Persistence.EntityFrameworkCore.PostgreSQL
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        static UnitOfWorkFactory()
        {
            using var context = new PRDbContext();
            context.Database.EnsureCreated();

            if (context.People.Any()) return;

            SeedDatabase(context);
        }

        public void Initialize(ILogger logger)
        {
        }

        public Task<bool> CheckRepositoryConnection()
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            throw new NotImplementedException();
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
