using Microsoft.AspNetCore.Identity;
using System;

namespace C2IEDM.Web.Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context,
            UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any() && !context.Locations.Any())
            {
                var users = new List<AppUser>
                {
                    new AppUser
                    {
                        DisplayName = "Bob",
                        UserName = "bob",
                        Email = "bob@test.com"
                    },
                    new AppUser
                    {
                        DisplayName = "Jane",
                        UserName = "jane",
                        Email = "jane@test.com"
                    },
                    new AppUser
                    {
                        DisplayName = "Tom",
                        UserName = "tom",
                        Email = "tom@test.com"
                    },
                };

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                }

                //var now = DateTime.UtcNow;

                //var people = new List<Person>
                //{
                //    new Person
                //    {
                //        FirstName = "Hugo",
                //        Dead = true,
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Hannibal",
                //        Dead = false,
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Ludvig",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Klaus",
                //        Surname = "Berntsen",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Carl",
                //        Surname = "Theodor Zahle",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1909-1910 og 1913-1920",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Otto",
                //        Surname = "Liebe",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1920",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Michael",
                //        Surname = "Petersen Friis",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1920",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Niels",
                //        Surname = "Neergaard",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1920-1924",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Thomas",
                //        Surname = "Madsen-Mygdal",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1926-1929",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Thorvald",
                //        Surname = "Stauning",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1924-1926 og 1929-1942",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Erik",
                //        Surname = "Scavenius",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1942-1943",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Vilhelm",
                //        Surname = "Buhl",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1942 og 1945",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Knud",
                //        Surname = "Kristensen",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1945-1947",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Erik",
                //        Surname = "Eriksen",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1950-1953",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Hans",
                //        Surname = "Hedtoft",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1947-1950 og 1953-1955",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Hans Christian",
                //        Surname = "Svane Hansen",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1955-1960",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Viggo",
                //        Surname = "Kampmann",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1960-1962",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Hilmar",
                //        Surname = "Baunsgaard",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1968-1971",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Jens Otto",
                //        Surname = "Krag",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1962-1968 og 1975-1982",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Poul",
                //        Surname = "Hartling",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1973-1975",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Anker",
                //        Surname = "Jørgensen",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1972-1973 og 1975-1982",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Poul",
                //        Surname = "Schlüter",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1982-1993",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Poul",
                //        Surname = "Nyrup",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "1993-2001",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Anders",
                //        Surname = "Fogh Rasmussen",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "2001-2009",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Helle",
                //        Surname = "Thorning Schmidt",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "2011-2015",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Lars",
                //        Surname = "Løkke Rasmussen",
                //        Birthday = new DateTime(1956, 4, 27, 0, 0, 0, DateTimeKind.Utc),
                //        Description = "2009-2011 og 2015-2019",
                //        Created = now
                //    },
                //    new Person
                //    {
                //        FirstName = "Mette",
                //        Surname = "Frederiksen",
                //        Nickname = "Slette Mette",
                //        Address = "Kartoffelrækkerne",
                //        ZipCode = "2100",
                //        City = "Copenhagen",
                //        Birthday = new DateTime(1975, 7, 24, 0, 0, 0, DateTimeKind.Utc),
                //        Category = "Politik",
                //        Description = "2019-",
                //        Dead = false,
                //        Created = now
                //    }
                //};

                //await context.People.AddRangeAsync(people);
                await context.SaveChangesAsync();
            }
        }
    }
}
