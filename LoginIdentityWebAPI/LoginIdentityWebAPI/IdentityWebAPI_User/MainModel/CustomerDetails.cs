using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityWebAPI_User.MainModel
{
    public class CustomerDetails:IdentityUser
    {
        [Required]
        public string CustomerName { get; set; } = string.Empty;
    }
}
