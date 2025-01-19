using PR.Domain.Entities.PR;
using System;
using PR.Domain.Entities.Smurfs;

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

            CreateDataForSeeding(
                PRDbContextBase.Versioned, 
                out var people, 
                out var personComments,
                out var smurfs);

            context.Smurfs.AddRange(smurfs);

            context.People.AddRange(people);
            context.PersonComments.AddRange(personComments);

            context.SaveChanges();
        }

        public static void CreateDataForSeeding(
            bool versioned,
            out List<Person> people,
            out List<PersonComment> personComments,
            out List<Smurf> smurfs)
        {
            if (versioned)
            {
                CreateBitemporalDataForSeeding(out people, out personComments);
            }
            else
            {
                CreateCurrentDataForSeeding(out people, out personComments);
            }

            smurfs = new List<Smurf>
            {
                new Smurf{ Name = "Gammelsmolf"},
                new Smurf{ Name = "Smolfine"},
            };
        }

        private static void CreateBitemporalDataForSeeding(
            out List<Person> people,
            out List<PersonComment> personComments)
        {
            var now = DateTime.UtcNow;
            var maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            var chewbaccaIsEnteredIncorrectly = now.Date.AddDays(-1);
            var chewbaccaIsCorrected = chewbaccaIsEnteredIncorrectly.AddDays(1) - TimeSpan.FromHours(1);
            var commentOnReyIsEnteredIncorrectly = now.Date.AddDays(-2);
            var commentOnReyIsCorrected = chewbaccaIsEnteredIncorrectly.AddDays(1) - TimeSpan.FromHours(2);

            var anakinIsIntroduced = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var anakinBecomesDarthVader = new DateTime(2003, 10, 4, 0, 0, 0, DateTimeKind.Utc);
            var chewbaccaIsIntroduced = new DateTime(2004, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var darthVaderDies = new DateTime(2006, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var reyIsIntroduced = new DateTime(2007, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var reyBecomesReySkywalker = new DateTime(2009, 10, 1, 0, 0, 0, DateTimeKind.Utc);

            var anakin_1_1 = new Person
            {
                ID = new Guid("00000004-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000004-0001-0001-0000-000000000000"),
                Created = now,
                Superseded = maxDate,
                Start = anakinIsIntroduced,
                End = anakinBecomesDarthVader,
                FirstName = "Anakin Skywalker",
                Latitude = 10,
                Longitude = 56
            };

            var anakin_2_1 = new Person
            {
                ID = new Guid("00000004-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000004-0002-0001-0000-000000000000"),
                Created = now,
                Superseded = maxDate,
                Start = anakinBecomesDarthVader,
                End = darthVaderDies,
                FirstName = "Darth Vader",
                Latitude = 10.2,
                Longitude = 56
            };

            var chewbacca_1_1 = new Person
            {
                ID = new Guid("00000005-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000005-0001-0001-0000-000000000000"),
                Created = chewbaccaIsEnteredIncorrectly,
                Superseded = chewbaccaIsCorrected,
                Start = chewbaccaIsIntroduced,
                End = maxDate,
                FirstName = "Chewing Gum",
                Latitude = 9,
                Longitude = 56
            };

            var chewbacca_1_2 = new Person
            {
                ID = new Guid("00000005-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000005-0001-0002-0000-000000000000"),
                Created = chewbaccaIsCorrected,
                Superseded = maxDate,
                Start = chewbaccaIsIntroduced,
                End = maxDate,
                FirstName = "Chewbacca",
                Latitude = 9,
                Longitude = 56
            };

            var rey_1_1 = new Person
            {
                ID = new Guid("00000006-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000006-0001-0001-0000-000000000000"),
                Created = now,
                Superseded = maxDate,
                Start = reyIsIntroduced,
                End = reyBecomesReySkywalker,
                FirstName = "Rey",
                Latitude = 10,
                Longitude = 56
            };

            var rey_2_1 = new Person
            {
                ID = new Guid("00000006-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000006-0002-0001-0000-000000000000"),
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
                anakin_1_1,
                anakin_2_1,
                chewbacca_1_1,
                chewbacca_1_2,
                rey_1_1,
                rey_2_1
            };

            var rey_comment_1_1 = new PersonComment
            {
                ID = new Guid("00000001-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000001-0001-0001-0000-000000000000"),
                PersonID = new Guid("00000006-0000-0000-0000-000000000000"),
                PersonArchiveID = new Guid("00000006-0001-0001-0000-000000000000"),
                Created = commentOnReyIsEnteredIncorrectly,
                Superseded = commentOnReyIsCorrected,
                Text = "She starts out as a ravager"
            };

            var rey_comment_1_2 = new PersonComment
            {
                ID = new Guid("00000001-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000001-0001-0002-0000-000000000000"),
                PersonID = new Guid("00000006-0000-0000-0000-000000000000"),
                PersonArchiveID = new Guid("00000006-0001-0001-0000-000000000000"),
                Created = commentOnReyIsCorrected,
                Superseded = maxDate,
                Text = "She starts out as a scavenger"
            };

            var chewbacca_comment_1_1 = new PersonComment
            {
                ID = new Guid("00000002-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000002-0001-0001-0000-000000000000"),
                PersonID = new Guid("00000006-0000-0000-0000-000000000000"),
                PersonArchiveID = new Guid("00000006-0001-0001-0000-000000000000"),
                Created = now,
                Superseded = maxDate,
                Text = "He likes his crossbow"
            };

            personComments = new List<PersonComment>
            {
                rey_comment_1_1,
                rey_comment_1_2,
                chewbacca_comment_1_1
            };
        }

        private static void CreateCurrentDataForSeeding(
            out List<Person> people,
            out List<PersonComment> personComments)
        {
            var chewbacca = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000005"),
                FirstName = "Chewbacca",
                Latitude = 9,
                Longitude = 56
            };

            var rey = new Person
            {
                ID = new Guid("12345678-0000-0000-0000-000000000006"),
                FirstName = "Rey Skywalker",
                Latitude = 10.2,
                Longitude = 56
            };

            people = new List<Person>
            {
                rey,
                //chewbacca
            };

            var rey_comment = new PersonComment
            {
                ID = new Guid("12345678-1234-0000-0000-000000000006"),
                PersonID = new Guid("12345678-0000-0000-0000-000000000006"),
                Text = "She starts out as a scavenger"
            };

            personComments = new List<PersonComment>
            {
                rey_comment
            };
        }
    }
}
