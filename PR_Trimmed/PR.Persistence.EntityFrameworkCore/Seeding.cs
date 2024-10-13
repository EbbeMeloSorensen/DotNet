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
            var anakinBecomesDarthVader = new DateTime(2014, 5, 20, 3, 6, 9, DateTimeKind.Utc);
            var darthVaderDies = new DateTime(2016, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var benSoloBecomesKyloRen = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var hanSoloDies = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var lukeSkywalkerDies = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var delay = 0;

            var anakin_0 = new Person
            {
                ObjectId = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"),
                Created = baseTime + new TimeSpan(delay++),
                Superseded = anakinBecomesDarthVader,
                FirstName = "Anakin",
                Surname = "Skywalker"
            };

            var anakin_1 = new Person
            {
                ObjectId = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"),
                Created = anakinBecomesDarthVader,
                Superseded = darthVaderDies,
                FirstName = "Darth",
                Surname = "Vader"
            };

            var hanSolo_0 = new Person
            {
                ObjectId = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF01"),
                Created = baseTime + new TimeSpan(delay++),
                Superseded = hanSoloDies,
                FirstName = "Han",
                Surname = "Solo"
            };

            var luke_0 = new Person
            {
                ObjectId = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF02"),
                Created = baseTime + new TimeSpan(delay++),
                Superseded = lukeSkywalkerDies,
                FirstName = "Luke",
                Surname = "Skywalker"
            };

            var leia_0 = new Person
            {
                ObjectId = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF03"),
                Created = baseTime + new TimeSpan(delay++),
                Superseded = maxDate,
                FirstName = "Leia",
                Surname = "Organa"
            };

            var benSolo_0 = new Person
            {
                ObjectId = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF04"),
                Created = baseTime + new TimeSpan(delay++),
                Superseded = benSoloBecomesKyloRen,
                FirstName = "Ben",
                Surname = "Solo"
            };

            var benSolo_1 = new Person
            {
                ObjectId = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF04"),
                Created = benSoloBecomesKyloRen,
                Superseded = maxDate,
                FirstName = "Kylo",
                Surname = "Ren"
            };

            var people = new List<Person>
            {
                anakin_0,
                anakin_1,
                hanSolo_0,
                luke_0,
                leia_0,
                benSolo_0,
                benSolo_1
            };

            context.AddRange(people);
            context.SaveChanges();
        }
    }
}
