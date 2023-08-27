using Microsoft.AspNetCore.Identity;

namespace C2IEDM.Web.Persistence;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; } = null!;
}