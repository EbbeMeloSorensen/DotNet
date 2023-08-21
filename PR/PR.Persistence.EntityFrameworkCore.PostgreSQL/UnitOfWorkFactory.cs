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
            // Star Wars
            // https://cdn.shopify.com/s/files/1/1835/6621/files/star-wars-family-tree-episde-9.png?v=1578255836

            var now = DateTime.UtcNow;
            var delay = 0;

            var luke = new Person
            {
                FirstName = "Luke",
                Surname = "Skywalker",
                Created = now + new TimeSpan(delay++)
            };

            var leia = new Person
            {
                FirstName = "Leia",
                Surname = "Organa",
                Created = now + new TimeSpan(delay++),
            };

            var anakin = new Person
            {
                FirstName = "Anakin",
                Surname = "Skywalker",
                Nickname = "Darth Vader",
                Created = now + new TimeSpan(delay++),
            };

            var obiVan = new Person
            {
                FirstName = "Obi Van",
                Surname = "Kenobi",
                Created = now + new TimeSpan(delay++),
            };

            var padme = new Person
            {
                FirstName = "Padme",
                Surname = "Amidala",
                Created = now + new TimeSpan(delay++),
            };

            var benSolo = new Person
            {
                FirstName = "Ben",
                Surname = "Solo",
                Nickname = "Kylo Ren",
                Created = now + new TimeSpan(delay++),
            };

            var hanSolo = new Person
            {
                FirstName = "Han",
                Surname = "Solo",
                Created = now + new TimeSpan(delay++),
            };

            var quiGon = new Person
            {
                FirstName = "Qui-Gon",
                Surname = "Jinn",
                Created = now + new TimeSpan(delay++),
            };

            var countDooku = new Person
            {
                FirstName = "Count Dooku",
                Nickname = "Darth Tyranus",
                Created = now + new TimeSpan(delay++),
            };

            var palpatine = new Person
            {
                FirstName = "Emperor Palpatine",
                Nickname = "Darth Sidious",
                Created = now + new TimeSpan(delay++),
            };

            var rey = new Person
            {
                FirstName = "Rey",
                Surname = "Skywalker",
                Created = now + new TimeSpan(delay++),
            };

            var people = new List<Person>
            {
                luke,
                leia,
                anakin,
                obiVan,
                padme,
                benSolo,
                hanSolo,
                quiGon,
                countDooku,
                palpatine,
                rey
            };

            var personAssociations = new List<PersonAssociation>
            {
                new()
                {
                    SubjectPerson = anakin,
                    Description = "is married with",
                    ObjectPerson = padme,
                    Created = now + new TimeSpan(delay),
                },
                new()
                {
                    SubjectPerson = anakin,
                    Description = "is a parent of",
                    ObjectPerson = luke,
                    Created = now + new TimeSpan(delay + 1),
                },
                new()
                {
                    SubjectPerson = anakin,
                    Description = "is a parent of",
                    ObjectPerson = leia,
                    Created = now + new TimeSpan(delay + 2),
                },
                new()
                {
                    SubjectPerson = padme,
                    Description = "is a parent of",
                    ObjectPerson = luke,
                    Created = now + new TimeSpan(delay + 3),
                },
                new()
                {
                    SubjectPerson = padme,
                    Description = "is a parent of",
                    ObjectPerson = leia,
                    Created = now + new TimeSpan(delay + 4),
                },
                new()
                {
                    SubjectPerson = leia,
                    Description = "is married with",
                    ObjectPerson = hanSolo,
                    Created = now + new TimeSpan(delay + 5),
                },
                new()
                {
                    SubjectPerson = leia,
                    Description = "is a parent of",
                    ObjectPerson = benSolo,
                    Created = now + new TimeSpan(delay + 6),
                },
                new()
                {
                    SubjectPerson = hanSolo,
                    Description = "is a parent of",
                    ObjectPerson = benSolo,
                    Created = now + new TimeSpan(delay + 7),
                },
                new()
                {
                    SubjectPerson = luke,
                    Description = "is an apprentice of",
                    ObjectPerson = obiVan,
                    Created = now + new TimeSpan(delay + 8),
                },
                new()
                {
                    SubjectPerson = obiVan,
                    Description = "is an apprentice of",
                    ObjectPerson = quiGon,
                    Created = now + new TimeSpan(delay + 9),
                },
                new()
                {
                    SubjectPerson = quiGon,
                    Description = "is an apprentice of",
                    ObjectPerson = countDooku,
                    Created = now + new TimeSpan(delay + 10)
                },
                new()
                {
                    SubjectPerson = countDooku,
                    Description = "is an apprentice of",
                    ObjectPerson = palpatine,
                    Created = now + new TimeSpan(delay + 11)
                },
                new()
                {
                    SubjectPerson = anakin,
                    Description = "is an apprentice of",
                    ObjectPerson = palpatine,
                    Created = now + new TimeSpan(delay + 12)
                },
                new()
                {
                    SubjectPerson = palpatine,
                    Description = "is a grandparent of",
                    ObjectPerson = rey,
                    Created = now + new TimeSpan(delay + 13)
                },
                new()
                {
                    SubjectPerson = rey,
                    Description = "is an apprentice of",
                    ObjectPerson = luke,
                    Created = now + new TimeSpan(delay + 14)
                },
                new()
                {
                    SubjectPerson = rey,
                    Description = "is an apprentice of",
                    ObjectPerson = leia,
                    Created = now + new TimeSpan(delay + 15)
                },
            };

            context.AddRange(people);
            context.AddRange(personAssociations);
            context.SaveChanges();
        }
    }
}