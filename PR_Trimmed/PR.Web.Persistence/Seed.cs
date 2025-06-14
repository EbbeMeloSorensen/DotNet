﻿using Microsoft.AspNetCore.Identity;
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
                    var result = await userManager.CreateAsync(user, "Super-long-very-secure-secret-key-that-is-at-least-64-bytes-in-length!!!!");

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine(error.Description);
                        }
                    }
                }

                Seeding.CreateDataForSeeding(
                    true, 
                    out var people, 
                    out var personComments,
                    out var smurfs);

                await context.People.AddRangeAsync(people);
                await context.PersonComments.AddRangeAsync(personComments);
                await context.Smurfs.AddRangeAsync(smurfs);
                await context.SaveChangesAsync();
            }
        }
    }
}
