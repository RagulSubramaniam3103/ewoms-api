using LoginIdentityWebAPI.UserControlled;
using Microsoft.AspNetCore.Identity;

namespace LoginIdentityWebAPI.Data
{
    public class ApplicationUser : IdentityUser<int>
    {
        public UserMainDetails? UserMainDetails { get; set; }
    }
}
