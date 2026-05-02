using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FirstProjectWebAPI.MainData.ModelsMigration
{
    public class CustomerDetails :IdentityUser
    {
        [Required]
        public string CustomerName { get; set; }
        public ICollection<CustomerAddress> customerAddresses { get; set; }
    }
}
