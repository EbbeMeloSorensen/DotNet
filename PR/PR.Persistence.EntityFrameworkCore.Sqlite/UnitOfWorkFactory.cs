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
            // Star Wars
            // https://cdn.shopify.com/s/files/1/1835/6621/files/star-wars-family-tree-episde-9.png?v=1578255836

            var now = DateTime.UtcNow;
            var delay = 0;

            var luke = new Person
            {
                FirstName = "Luke",
                Surname = "Skywalker",
                Category = "Jedi",
                Address = "Tatooine",
                Created = now + new TimeSpan(delay++)
            };

            var leia = new Person
            {
                FirstName = "Leia",
                Surname = "Organa",
                Description = "Princess",
                Address = "Alderaan",
                Created = now + new TimeSpan(delay++),
            };

            var anakin = new Person
            {
                FirstName = "Anakin",
                Surname = "Skywalker",
                Nickname = "Darth Vader",
                Category = "Jedi, Sith",
                Created = now + new TimeSpan(delay++),
            };

            var obiWan = new Person
            {
                FirstName = "Obi-Wan",
                Surname = "Kenobi",
                Category = "Jedi",
                Created = now + new TimeSpan(delay++),
            };

            var padme = new Person
            {
                FirstName = "Padme",
                Surname = "Amidala",
                Address = "Naboo",
                Description = "Princess",
                Created = now + new TimeSpan(delay++),
            };

            var benSolo = new Person
            {
                FirstName = "Ben",
                Surname = "Solo",
                Nickname = "Kylo Ren",
                Category = "Jedi/Sith",
                Created = now + new TimeSpan(delay++),
            };

            var hanSolo = new Person
            {
                FirstName = "Han",
                Surname = "Solo",
                Description = "Smuggler",
                Created = now + new TimeSpan(delay++),
            };

            var quiGon = new Person
            {
                FirstName = "Qui-Gon",
                Surname = "Jinn",
                Category = "Jedi",
                Created = now + new TimeSpan(delay++),
            };

            var countDooku = new Person
            {
                FirstName = "Count Dooku",
                Nickname = "Darth Tyranus",
                Category = "Sith",
                Created = now + new TimeSpan(delay++),
            };

            var palpatine = new Person
            {
                FirstName = "Emperor Palpatine",
                Nickname = "Darth Sidious",
                Category = "Sith",
                Created = now + new TimeSpan(delay++),
            };

            var rey = new Person
            {
                FirstName = "Rey",
                Surname = "Skywalker",
                Category = "Jedi",
                Created = now + new TimeSpan(delay++),
            };

            var maul = new Person
            {
                FirstName = "Darth Maul",
                Category = "Sith",
                Created = now + new TimeSpan(delay++),
            };

            var plagueis = new Person
            {
                FirstName = "Darth Plagueis",
                Category = "Sith",
                Created = now + new TimeSpan(delay++),
            };

            var yoda = new Person
            {
                FirstName = "Yoda",
                Category = "Jedi",
                Created = now + new TimeSpan(delay++),
            };

            var lando = new Person
            {
                FirstName = "Lando",
                Surname = "Calrissian",
                Description = "Smuggler",
                Created = now + new TimeSpan(delay++),
            };

            var chewbacca = new Person
            {
                FirstName = "Chewbacca",
                Nickname = "Chewie",
                Address = "Kashyyyk",
                Description = "Wookie",
                Created = now + new TimeSpan(delay++),
            };

            var bobaFett = new Person
            {
                FirstName = "Boba",
                Surname = "Fett",
                Address = "Mandalore",
                Description = "Bounty Hunter",
                Created = now + new TimeSpan(delay++),
            };

            var people = new List<Person>
            {
                luke,
                leia,
                anakin,
                obiWan,
                padme,
                benSolo,
                hanSolo,
                quiGon,
                countDooku,
                palpatine,
                rey,
                maul,
                plagueis,
                yoda,
                lando,
                chewbacca,
                bobaFett
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
                    ObjectPerson = obiWan,
                    Created = now + new TimeSpan(delay + 8),
                },
                new()
                {
                    SubjectPerson = obiWan,
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
                new()
                {
                    SubjectPerson = maul,
                    Description = "is an apprentice of",
                    ObjectPerson = palpatine,
                    Created = now + new TimeSpan(delay + 16)
                },
                new()
                {
                    SubjectPerson = palpatine,
                    Description = "is an apprentice of",
                    ObjectPerson = plagueis,
                    Created = now + new TimeSpan(delay + 17)
                },
                new()
                {
                    SubjectPerson = countDooku,
                    Description = "is an apprentice of",
                    ObjectPerson = yoda,
                    Created = now + new TimeSpan(delay + 18)
                },
                new()
                {
                    SubjectPerson = luke,
                    Description = "is an apprentice of",
                    ObjectPerson = yoda,
                    Created = now + new TimeSpan(delay + 19)
                },
                new()
                {
                    SubjectPerson = lando,
                    Description = "is a friend of",
                    ObjectPerson = hanSolo,
                    Created = now + new TimeSpan(delay + 20)
                },
                new()
                {
                    SubjectPerson = chewbacca,
                    Description = "is a friend of",
                    ObjectPerson = hanSolo,
                    Created = now + new TimeSpan(delay + 21)
                },
                new()
                {
                    SubjectPerson = bobaFett,
                    Description = "is employed by",
                    ObjectPerson = anakin,
                    Created = now + new TimeSpan(delay + 22)
                },
            };

            context.AddRange(people);
            context.AddRange(personAssociations);
            context.SaveChanges();
        }
    }
}
