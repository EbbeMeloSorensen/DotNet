using PR.Domain.Entities;
using System;

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
                CreateBitemporalDataForSeeding(out people);
            }
            else
            {
                CreateCurrentDataForSeeding(out people);
            }
        }

        private static void CreateBitemporalDataForSeeding(
            out List<Person> people)
        {
            var now = DateTime.UtcNow;

            var maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            var chewbaccaIsEnteredIncorrectly = now.Date.AddDays(-1);
            var chewbaccaIsCorrected = chewbaccaIsEnteredIncorrectly.AddDays(1) - TimeSpan.FromHours(1);

            var padmeIsIntroduced = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var quigonIsIntroduced = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var obiwanIsIntroduced = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var anakinIsIntroduced = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var quigonDies = new DateTime(2001, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var padmeDies = new DateTime(2003, 10, 3, 0, 0, 0, DateTimeKind.Utc);
            var anakinBecomesDarthVader = new DateTime(2003, 10, 4, 0, 0, 0, DateTimeKind.Utc);
            var chewbaccaIsIntroduced = new DateTime(2004, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var obiWanDies = new DateTime(2004, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var darthVaderDies = new DateTime(2006, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var reyIsIntroduced = new DateTime(2007, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var reyBecomesReySkywalker = new DateTime(2009, 10, 1, 0, 0, 0, DateTimeKind.Utc);

            var padme_0_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000001"),
                Created = now,
                Superseded = maxDate,
                Start = padmeIsIntroduced,
                End = padmeDies,
                FirstName = "Padme Amidala",
                Latitude = 11,
                Longitude = 57
            };

            var quigon_0_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000002"),
                Created = now,
                Superseded = maxDate,
                Start = quigonIsIntroduced,
                End = quigonDies,
                FirstName = "Quigon Jinn",
                Latitude = 10,
                Longitude = 57
            };

            var obiWan_0_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000003"),
                Created = now,
                Superseded = maxDate,
                Start = obiwanIsIntroduced,
                End = obiWanDies,
                FirstName = "Obi Wan Kenobi",
                Latitude = 10,
                Longitude = 58
            };

            var anakin_0_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000004"),
                Created = now,
                Superseded = maxDate,
                Start = anakinIsIntroduced,
                End = anakinBecomesDarthVader,
                FirstName = "Anakin Skywalker",
                Latitude = 10,
                Longitude = 56
            };

            var anakin_1_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000004"),
                Created = now,
                Superseded = maxDate,
                Start = anakinBecomesDarthVader,
                End = darthVaderDies,
                FirstName = "Darth Vader",
                Latitude = 10.2,
                Longitude = 56
            };

            var chewbacca_0_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000005"),
                Created = chewbaccaIsEnteredIncorrectly,
                Superseded = chewbaccaIsCorrected,
                Start = chewbaccaIsIntroduced,
                End = maxDate,
                FirstName = "Chewie",
                Latitude = 9,
                Longitude = 56
            };

            var chewbacca_0_1 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000005"),
                Created = chewbaccaIsCorrected,
                Superseded = maxDate,
                Start = chewbaccaIsIntroduced,
                End = maxDate,
                FirstName = "Chewbacca",
                Latitude = 9,
                Longitude = 56
            };

            var rey_0_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000006"),
                Created = now,
                Superseded = maxDate,
                Start = reyIsIntroduced,
                End = reyBecomesReySkywalker,
                FirstName = "Rey",
                Latitude = 10,
                Longitude = 56
            };

            var rey_1_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000006"),
                Created = now,
                Superseded = maxDate,
                Start = reyBecomesReySkywalker,
                End = maxDate,
                FirstName = "Rey Skywalker",
                Latitude = 10.2,
                Longitude = 56
            };

            people = new List<Person>
            {
                padme_0_0,
                quigon_0_0,
                obiWan_0_0,
                anakin_0_0,
                anakin_1_0,
                chewbacca_0_0,
                chewbacca_0_1,
                rey_0_0,
                rey_1_0
            };
        }

        private static void CreateCurrentDataForSeeding(
            out List<Person> people)
        {
            var chewbacca = new Person
            {
                ArchiveID = new Guid("00000000-0000-0000-0000-000000000005"),
                ID = new Guid("12345678-0000-0000-0000-000000000005"),
                FirstName = "Chewbacca",
                Latitude = 9,
                Longitude = 56
            };

            var rey = new Person
            {
                ArchiveID = new Guid("00000000-0000-0000-0000-000000000006"),
                ID = new Guid("12345678-0000-0000-0000-000000000006"),
                FirstName = "Rey Skywalker",
                Latitude = 10.2,
                Longitude = 56
            };

            people = new List<Person>
            {
                rey,
                chewbacca
            };
        }
    }
}
