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
                //CreateVersionedDataForSeeding(out people);
                CreateBitemporalDataForSeeding(out people);
            }
            else
            {
                CreateNonversionedDataForSeeding(out people);
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
                Latitude = 11,
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
                Latitude = 10,
                Longitude = 55
            };

            var chewbacca_0_1 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000005"),
                Created = chewbaccaIsCorrected,
                Superseded = maxDate,
                Start = chewbaccaIsIntroduced,
                End = maxDate,
                FirstName = "Chewbacca",
                Latitude = 10,
                Longitude = 55
            };

            var rey_0_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000006"),
                Created = now,
                Superseded = maxDate,
                Start = reyIsIntroduced,
                End = reyBecomesReySkywalker,
                FirstName = "Rey",
                Latitude = 12,
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
                Latitude = 12,
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

        private static void CreateVersionedDataForSeeding(
            out List<Person> people)
        {
            var maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            // Changes
            var quiGonDies = new DateTime(2001, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var padmeDies = new DateTime(2003, 11, 3, 0, 0, 0, DateTimeKind.Utc);
            var anakinBecomesDarthVader = new DateTime(2003, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var obiWanDies = new DateTime(2004, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var darthVaderDies = new DateTime(2006, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var hansSoloDies = new DateTime(2007, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var lukeDies = new DateTime(2008, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var leiaDies = new DateTime(2009, 9, 1, 0, 0, 0, DateTimeKind.Utc);
            var palpatineDies = new DateTime(2009, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var kyloRenDies = new DateTime(2009, 10, 2, 0, 0, 0, DateTimeKind.Utc);
            var reyTakesSurnameSkywalker = new DateTime(2009, 10, 1, 0, 0, 0, DateTimeKind.Utc);

            var padme_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000001"),
                Created = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = padmeDies,
                FirstName = "Padme",
                Surname = "Amidala"
            };

            var padme_1 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000001"),
                Created = padmeDies,
                Superseded = new DateTime(2003, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                FirstName = "Padme",
                Surname = "Amidala"
            };

            var obiWan_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000002"),
                Created = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = obiWanDies,
                FirstName = "Obi-Wan",
                Surname = "Kenobi",
                Nickname = "Ben"
            };

            var obiWan_1 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000002"),
                Created = obiWanDies,
                Superseded = new DateTime(2004, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                FirstName = "Obi-Wan",
                Surname = "Kenobi",
                Nickname = "Ben"
            };

            var quiGon_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000003"),
                Created = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = quiGonDies,
                FirstName = "Qui-Gon",
                Surname = "Jinn"
            };

            var quiGon_1 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000003"),
                Created = quiGonDies,
                Superseded = new DateTime(2001, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                FirstName = "Qui-Gon",
                Surname = "Jinn"
            };

            var palpatine_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000004"),
                Created = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = palpatineDies,
                FirstName = "Palpatine",
                Address = "Coruscant",
                ZipCode = "66"
            };

            var palpatine_1 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000004"),
                Created = palpatineDies,
                Superseded = new DateTime(2009, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                FirstName = "Palpatine",
                Address = "Coruscant",
                ZipCode = "66"
            };

            var anakin_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000005"),
                Created = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = anakinBecomesDarthVader,
                FirstName = "Anakin",
                Surname = "Skywalker"
            };

            var anakin_1 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000005"),
                Created = anakinBecomesDarthVader,
                Superseded = darthVaderDies,
                FirstName = "Darth",
                Surname = "Vader"
            };

            var anakin_2 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000005"),
                Created = darthVaderDies,
                Superseded = new DateTime(2006, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                FirstName = "Darth",
                Surname = "Vader"
            };

            var r2d2 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000006"),
                Created = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = maxDate,
                FirstName = "R2D2"
            };

            var c3po = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000007"),
                Created = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = maxDate,
                FirstName = "C3PO"
            };

            var luke_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000008"),
                Created = new DateTime(2003, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = lukeDies,
                FirstName = "Luke",
                Surname = "Skywalker",
                Birthday = new DateTime(2003, 11, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            var luke_1 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000008"),
                Created = lukeDies,
                Superseded = new DateTime(2008, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                FirstName = "Luke",
                Surname = "Skywalker",
                Birthday = new DateTime(2003, 11, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            var leia_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000009"),
                Created = new DateTime(2003, 11, 2, 0, 0, 0, DateTimeKind.Utc),
                Superseded = leiaDies,
                FirstName = "Leia",
                Surname = "Organa",
                Birthday = new DateTime(2003, 11, 2, 0, 0, 0, DateTimeKind.Utc),
                Description = "Princess"
            };

            var leia_1 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000009"),
                Created = leiaDies,
                Superseded = new DateTime(2009, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                FirstName = "Leia",
                Surname = "Organa",
                Birthday = new DateTime(2003, 11, 2, 0, 0, 0, DateTimeKind.Utc),
                Description = "Princess"
            };

            var hanSolo_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000010"),
                Created = new DateTime(2004, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = hansSoloDies,
                FirstName = "Han",
                Surname = "Solo",
                Description = "Smuggler"
            };

            var hanSolo_1 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000010"),
                Created = hansSoloDies,
                Superseded = new DateTime(2007, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                FirstName = "Han",
                Surname = "Solo",
                Description = "Smuggler"
            };

            var chewbacca = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000011"),
                Created = new DateTime(2004, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = maxDate,
                FirstName = "Chewbacca",
                Nickname = "Chewie",
                Category = "Wookie"
            };

            var lando = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000012"),
                Created = new DateTime(2005, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = maxDate,
                FirstName = "Lando",
                Surname = "Calrissian",
                Address = "Bespin",
                City = "Cloud City"
            };

            var rey_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000013"),
                Created = new DateTime(2007, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = reyTakesSurnameSkywalker,
                FirstName = "Rey",
                Address = "Tatooine"
            };

            var rey_1 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000013"),
                Created = reyTakesSurnameSkywalker,
                Superseded = maxDate,
                FirstName = "Rey",
                Surname = "Skywalker"
            };

            var kylo_0 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000014"),
                Created = new DateTime(2007, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Superseded = kyloRenDies,
                FirstName = "Kylo",
                Surname = "Ren"
            };

            var kylo_1 = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000014"),
                Created = kyloRenDies,
                Superseded = new DateTime(2009, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                FirstName = "Kylo",
                Surname = "Ren"
            };

            people = new List<Person>
            {
                padme_0,
                padme_1,
                obiWan_0,
                obiWan_1,
                quiGon_0,
                quiGon_1,
                palpatine_0,
                palpatine_1,
                r2d2,
                anakin_0,
                anakin_1,
                anakin_2,
                c3po,
                luke_0,
                luke_1,
                leia_0,
                leia_1,
                hanSolo_0,
                hanSolo_1,
                chewbacca,
                lando,
                rey_0,
                rey_1,
                kylo_0,
                kylo_1
            };
        }

        private static void CreateNonversionedDataForSeeding(
            out List<Person> people)
        {
            var rey = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000001"),
                FirstName = "Rey",
                Surname = "Skywalker"
            };

            var finn = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000002"),
                FirstName = "Finn"
            };

            var poeDameron = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000003"),
                FirstName = "Poe",
                Surname = "Dameron"
            };

            var chewbacca = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000004"),
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
