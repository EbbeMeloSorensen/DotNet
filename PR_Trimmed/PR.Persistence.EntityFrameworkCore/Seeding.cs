using PR.Domain.Entities;

namespace PR.Persistence.EntityFrameworkCore
{
    public static class Seeding
    {
        public static void SeedDatabase(
            PRDbContextBase context)
        {
            //return;

            // Star Wars
            // https://cdn.shopify.com/s/files/1/1835/6621/files/star-wars-family-tree-episde-9.png?v=1578255836

            if (context.People.Any()) return;

            var baseTime = new DateTime(2011, 4, 3, 7, 9, 13, DateTimeKind.Utc);
            var maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            var delay = 0;

            var anakin_0 = new Person
            {
                Id = new Guid("12345678-0000-0000-0000-000000000001"),
                Created = baseTime + new TimeSpan(delay++),
                Superseded = maxDate,
                FirstName = "Anakin",
                Surname = "Skywalker"
            };

            var people = new List<Person>
            {
                anakin_0,
            };

            context.AddRange(people);
            context.SaveChanges();
        }
    }
}
