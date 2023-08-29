using Microsoft.AspNetCore.Identity;
using System;
using C2IEDM.Domain.Entities.Geometry;

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

                var now = DateTime.UtcNow;

                var location1 = new AbsolutePoint
                {
                    LatitudeCoordinate = 1.2,
                    LongitudeCoordinate = 3.4
                };

                var location2 = new AbsolutePoint
                {
                    LatitudeCoordinate = 2.3,
                    LongitudeCoordinate = 4.5
                };

                var locations = new List<Location>
                {
                    location1,
                    location2
                };

                await context.Locations.AddRangeAsync(locations);
                await context.SaveChangesAsync();
            }
        }
    }
}
