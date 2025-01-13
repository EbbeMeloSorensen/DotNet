using Microsoft.AspNetCore.Identity;
using PR.Domain.Entities;
using PR.Persistence.EntityFrameworkCore;

namespace PR.Web.Persistence
{
    public class Seed
    {
        public static async Task SeedData(
            DataContext context,
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

                Seeding.CreateDataForSeeding(
                    true, 
                    out var people, 
                    out var personComments);

                await context.People.AddRangeAsync(people);
                await context.PersonComments.AddRangeAsync(personComments);
                await context.SaveChangesAsync();
            }
        }
    }
}
