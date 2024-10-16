using Microsoft.AspNetCore.Identity;
using PR.Domain.Entities;

namespace PR.Web.Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context,
            UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any() && !context.People.Any())
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

                var now = DateTime.UtcNow;

                var people = new List<Person>
                {
                    new Person
                    {
                        FirstName = "Hugo",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Hannibal",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Ludvig",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Klaus",
                        Surname = "Berntsen",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Carl",
                        Surname = "Theodor Zahle",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Otto",
                        Surname = "Liebe",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Michael",
                        Surname = "Petersen Friis",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Niels",
                        Surname = "Neergaard",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Thomas",
                        Surname = "Madsen-Mygdal",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Thorvald",
                        Surname = "Stauning",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Erik",
                        Surname = "Scavenius",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Vilhelm",
                        Surname = "Buhl",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Knud",
                        Surname = "Kristensen",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Erik",
                        Surname = "Eriksen",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Hans",
                        Surname = "Hedtoft",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Hans Christian",
                        Surname = "Svane Hansen",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Viggo",
                        Surname = "Kampmann",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Hilmar",
                        Surname = "Baunsgaard",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Jens Otto",
                        Surname = "Krag",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Poul",
                        Surname = "Hartling",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Anker",
                        Surname = "Jørgensen",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Poul",
                        Surname = "Schlüter",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Poul",
                        Surname = "Nyrup",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Anders",
                        Surname = "Fogh Rasmussen",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Helle",
                        Surname = "Thorning Schmidt",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Lars",
                        Surname = "Løkke Rasmussen",
                        Created = now
                    },
                    new Person
                    {
                        FirstName = "Mette",
                        Surname = "Frederiksen",
                        Created = now
                    }
                };

                await context.People.AddRangeAsync(people);
                await context.SaveChangesAsync();
            }
        }
    }
}
