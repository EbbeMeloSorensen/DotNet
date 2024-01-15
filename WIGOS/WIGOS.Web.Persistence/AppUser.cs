using Microsoft.AspNetCore.Identity;

namespace WIGOS.Web.Persistence
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = null!;
    }
}