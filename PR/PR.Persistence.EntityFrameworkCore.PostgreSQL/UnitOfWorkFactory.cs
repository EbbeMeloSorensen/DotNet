using Craft.Logging;
using Microsoft.EntityFrameworkCore;
using PR.Domain.Entities;

namespace PR.Persistence.EntityFrameworkCore.PostgreSQL
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
            if (!string.IsNullOrEmpty(Host) &&
                !string.IsNullOrEmpty(Port) &&
                !string.IsNullOrEmpty(Database) &&
                !string.IsNullOrEmpty(Schema) &&
                !string.IsNullOrEmpty(User) &&
                !string.IsNullOrEmpty(Password))
            {
                ConnectionStringProvider.Initialize(Host, int.Parse(Port), Database, Schema, User, Password);
            }
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
            var person1 = new Person
            {
                FirstName = "Uffe",
                Surname = "Sørensen",
                Created = new DateTime(2022, 1, 1, 3, 3, 3).ToUniversalTime()
            };

            var person2 = new Person
            {
                FirstName = "Ebbe",
                Surname = "Melo Sørensen",
                Created = new DateTime(2022, 1, 1, 3, 3, 3).ToUniversalTime()
            };

            var personAssociation = new PersonAssociation
            {
                SubjectPerson = person1,
                ObjectPerson = person2,
                Description = "is the brother of",
                Created = new DateTime(2022, 1, 1, 3, 3, 3).ToUniversalTime()
            };

            var people = new List<Person>
            {
                new Person
                {
                    FirstName = "Uffe",
                    Surname = "Sørensen",
                    Created = new DateTime(2022, 1, 1, 3, 3, 3).ToUniversalTime()
                },
                new Person
                {
                    FirstName = "Tina",
                    Surname = "Gosman",
                    Created = new DateTime(2022, 1, 1, 3, 3, 4).ToUniversalTime()
                },
                new Person
                {
                    FirstName = "Ebbe",
                    Created = new DateTime(2022, 1, 1, 3, 3, 5).ToUniversalTime()
                },
                new Person
                {
                    FirstName = "Ana Tayze",
                    Surname = "Melo Sørensen",
                    Nickname = "Hamos",
                    Address = "Danshøjvej 33",
                    ZipCode = "2500",
                    City = "Valby",
                    Birthday = new DateTime(1980, 6, 13).ToUniversalTime(),
                    Category = "Familie",
                    Description = "Min kone",
                    Dead = false,
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