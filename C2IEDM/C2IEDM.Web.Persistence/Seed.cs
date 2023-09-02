using Microsoft.AspNetCore.Identity;
using C2IEDM.Domain.Entities;
using C2IEDM.Domain.Entities.Geometry;

namespace C2IEDM.Web.Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context,
            UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
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

                await context.SaveChangesAsync();
            }

            if (!context.Locations.Any())
            {
                var absolutePoints = Enumerable
                    .Range(0, 10)
                    .Select(_ => new AbsolutePoint
                    {
                        LatitudeCoordinate = _ * 0.1,
                        LongitudeCoordinate = 7.913
                    });

                var line1 = new Line();
                var line2 = new Line();

                var lines = new List<Line> { line1, line2 };

                var linePoints = new List<LinePoint>
                {
                    new LinePoint
                    {
                        Line = line1,
                        Point = absolutePoints.Skip(0).First(),
                        Index = 0,
                        SequenceQuantity = 0
                    },
                    new LinePoint
                    {
                        Line = line1,
                        Point = absolutePoints.Skip(1).First(),
                        Index = 1,
                        SequenceQuantity = 1
                    },
                    new LinePoint
                    {
                        Line = line1,
                        Point = absolutePoints.Skip(2).First(),
                        Index = 2,
                        SequenceQuantity = 2
                    },
                    new LinePoint
                    {
                        Line = line1,
                        Point = absolutePoints.Skip(3).First(),
                        Index = 3,
                        SequenceQuantity = 3
                    },
                    new LinePoint
                    {
                        Line = line2,
                        Point = absolutePoints.Skip(4).First(),
                        Index = 0,
                        SequenceQuantity = 0
                    },
                    new LinePoint
                    {
                        Line = line2,
                        Point = absolutePoints.Skip(5).First(),
                        Index = 1,
                        SequenceQuantity = 1
                    },
                    new LinePoint
                    {
                        Line = line2,
                        Point = absolutePoints.Skip(6).First(),
                        Index = 2,
                        SequenceQuantity = 2
                    },
                    new LinePoint
                    {
                        Line = line2,
                        Point = absolutePoints.Skip(7).First(),
                        Index = 3,
                        SequenceQuantity = 3
                    },
                    new LinePoint
                    {
                        Line = line2,
                        Point = absolutePoints.Skip(8).First(),
                        Index = 4,
                        SequenceQuantity = 4
                    }
                };

                await context.Locations.AddRangeAsync(absolutePoints);
                await context.Lines.AddRangeAsync(lines);
                await context.LinePoints.AddRangeAsync(linePoints);
                await context.SaveChangesAsync();
            }

            if (!context.People.Any())
            {
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

                var people = new List<Person>
                {
                    luke,
                    leia,
                    anakin
                };

                await context.People.AddRangeAsync(people);
                await context.SaveChangesAsync();
            }
        }
    }
}
