using Microsoft.AspNetCore.Identity;

namespace Glossary.Web.Persistence
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = null!;
    }
}