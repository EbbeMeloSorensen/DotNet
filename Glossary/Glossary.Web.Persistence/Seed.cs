using Microsoft.AspNetCore.Identity;
using Glossary.Domain.Entities;

namespace Glossary.Web.Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context,
            UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any() && !context.Records.Any())
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

                var records = new List<Record>
                {
                    new Record
                    {
                        Term = "Hugo",
                        Created = now
                    },
                    new Record
                    {
                        Term = "Hannibal",
                        Created = now
                    },
                    new Record
                    {
                        Term = "Ludvig",
                        Created = now
                    }
                };

                await context.Records.AddRangeAsync(records);
                await context.SaveChangesAsync();
            }
        }
    }
}
