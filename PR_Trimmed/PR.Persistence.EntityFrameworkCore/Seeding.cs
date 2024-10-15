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

            if (PRDbContextBase.Versioned)
            {
                SeedVersionedDatabase(context);
            }
            else
            {
                SeedNonversionedDatabase(context);
            }
        }

        private static void SeedVersionedDatabase(
            PRDbContextBase context)
        {
            var maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
            var anakinBecomesDarthVader = new DateTime(2003, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var darthVaderDies = new DateTime(2006, 10, 1, 0, 0, 0, DateTimeKind.Utc);

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
                Superseded = darthVaderDies,
                FirstName = "Darth",
                Surname = "Vader"
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

            var people = new List<Person>
            {
                anakin_0,
                anakin_1,
                chewbacca,
                rey,
                finn,
                poeDameron
            };

            context.People.AddRange(people);
            context.SaveChanges();
        }

        private static void SeedNonversionedDatabase(
            PRDbContextBase context)
        {
            var maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            var delay = 0;

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

            var people = new List<Person>
            {
                rey,
                finn,
                poeDameron,
                chewbacca
            };

            context.People.AddRange(people);
            context.SaveChanges();
        }
    }
}
