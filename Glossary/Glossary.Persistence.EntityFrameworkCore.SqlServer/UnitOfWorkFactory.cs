using Craft.Logging;
using Microsoft.EntityFrameworkCore;
using Glossary.Domain.Entities;

namespace Glossary.Persistence.EntityFrameworkCore.SqlServer
{
    public class UnitOfWorkFactory : UnitOfWorkFactoryBase
    {
        static UnitOfWorkFactory()
        {
            using var context = new GlossaryDbContext();
            context.Database.EnsureCreated();

            if (context.People.Any()) return;

            //SeedDatabase(context);
        }

        public override void Initialize(ILogger logger)
        {
            if (!string.IsNullOrEmpty(Host) &&
                !string.IsNullOrEmpty(Port) &&
                !string.IsNullOrEmpty(Database) &&
                !string.IsNullOrEmpty(User) &&
                !string.IsNullOrEmpty(Password))
            {
                ConnectionStringProvider.Initialize(Host, Database, Schema, Password);
            }
        }

        public override Task<bool> CheckRepositoryConnection()
        {
            throw new NotImplementedException();
        }

        public override IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork(new GlossaryDbContext());
        }

        private static void SeedDatabase(DbContext context)
        {
            var person1 = new Record
            {
                Term = "Uffe",
                Created = new DateTime(2022, 1, 1, 3, 3, 3).ToUniversalTime()
            };

            var person2 = new Record
            {
                Term = "Ebbe",
                Created = new DateTime(2022, 1, 1, 3, 3, 3).ToUniversalTime()
            };

            var personAssociation = new RecordAssociation
            {
                SubjectRecord = person1,
                ObjectRecord = person2,
                Description = "is the brother of",
                Created = new DateTime(2022, 1, 1, 3, 3, 3).ToUniversalTime()
            };

            var people = new List<Record>
            {
                new Record
                {
                    Term = "Uffe",
                    Created = new DateTime(2022, 1, 1, 3, 3, 3).ToUniversalTime()
                },
                new Record
                {
                    Term = "Tina",
                    Created = new DateTime(2022, 1, 1, 3, 3, 4).ToUniversalTime()
                },
                new Record
                {
                    Term = "Ebbe",
                    Created = new DateTime(2022, 1, 1, 3, 3, 5).ToUniversalTime()
                },
                new Record
                {
                    Term = "Ana Tayze",
                    Source = "Danshøjvej 33",
                    Category = "Familie",
                    Description = "Min kone",
                    Created = new DateTime(2022, 1, 1, 3, 3, 6).ToUniversalTime()
                }
            };

            //context.AddRange(people);
            context.Add(person1);
            context.Add(person2);
            context.Add(personAssociation);
            context.SaveChanges();
        }
    }
}