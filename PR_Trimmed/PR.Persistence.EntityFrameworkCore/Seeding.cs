using PR.Domain.Entities;

namespace PR.Persistence.EntityFrameworkCore
{
    public static class Seeding
    {
        // We're seeding the database with characters from the Star Wars Universe
        // https://cdn.shopify.com/s/files/1/1835/6621/files/star-wars-family-tree-episde-9.png?v=1578255836

        public static void SeedDatabase(
            PRDbContextBase context)
        {
            if (context.People.Any()) return;

            CreateDataForSeeding(PRDbContextBase.Versioned, out var people);

            context.People.AddRange(people);
            context.SaveChanges();
        }

        public static void CreateDataForSeeding(
            bool versioned,
            out List<Person> people)
        {
            if (versioned)
            {
                CreateVersionedDataForSeeding(out people);
            }
            else
            {
                CreateNonversionedDataForSeeding(out people);
            }
        }

        private static void CreateVersionedDataForSeeding(
            out List<Person> people)
        {
            var maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
            var anakinBecomesDarthVader = new DateTime(2003, 10, 1, 0, 0, 0, DateTimeKind.Utc);

            var padme = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000006"),
                Created = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = new DateTime(2003, 11, 3, 0, 0, 0, DateTimeKind.Utc),
                FirstName = "Padme",
                Surname = "Amidala",
            };

            var obiWan = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000007"),
                Created = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = new DateTime(2004, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                FirstName = "Obi-Wan",
                Surname = "Kenobi",
            };

            var quiGon = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000008"),
                Created = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = new DateTime(2001, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                FirstName = "Qui-Gon",
                Surname = "Jinn"
            };

            var palpatine = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000009"),
                Created = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = new DateTime(2009, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                FirstName = "Palpatine",
            };

            var jarjar = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000010"),
                Created = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = maxDate,
                FirstName = "Jar Jar",
                Surname = "Binks",
            };

            var anakin_0 = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000005"),
                Created = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = anakinBecomesDarthVader,
                FirstName = "Anakin",
                Surname = "Skywalker"
            };

            var anakin_1 = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000005"),
                Created = anakinBecomesDarthVader,
                Superseded = new DateTime(2006, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                FirstName = "Darth",
                Surname = "Vader"
            };

            var luke = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000011"),
                Created = new DateTime(2003, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = new DateTime(2008, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                FirstName = "Luke",
                Surname = "Skywalker"
            };

            var leia = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000012"),
                Created = new DateTime(2003, 11, 2, 0, 0, 0, DateTimeKind.Utc),
                Superseded = new DateTime(2008, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                FirstName = "Leia",
                Surname = "Organa"
            };

            var chewbacca = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000004"),
                Created = new DateTime(2004, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = maxDate,
                FirstName = "Chewbacca"
            };

            var rey = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000001"),
                Created = new DateTime(2007, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = maxDate,
                FirstName = "Rey",
                Surname = "Skywalker"
            };

            var finn = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000002"),
                Created = new DateTime(2007, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = maxDate,
                FirstName = "Finn"
            };

            var poeDameron = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000003"),
                Created = new DateTime(2007, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = maxDate,
                FirstName = "Poe",
                Surname = "Dameron"
            };

            people = new List<Person>
            {
                padme,
                obiWan,
                quiGon,
                palpatine,
                jarjar,
                anakin_0,
                anakin_1,
                luke,
                leia,
                chewbacca,
                rey,
                finn,
                poeDameron
            };
        }

        private static void CreateNonversionedDataForSeeding(
            out List<Person> people)
        {
            var rey = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000001"),
                FirstName = "Rey",
                Surname = "Skywalker"
            };

            var finn = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000002"),
                FirstName = "Finn"
            };

            var poeDameron = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000003"),
                FirstName = "Poe",
                Surname = "Dameron"
            };

            var chewbacca = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000004"),
                FirstName = "Chewbacca"
            };

            people = new List<Person>
            {
                rey,
                finn,
                poeDameron,
                chewbacca
            };
        }
    }
}
