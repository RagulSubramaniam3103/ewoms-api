using System.ComponentModel.DataAnnotations;

namespace FirstProjectWebAPI.Commands.CustomerDetails
{
    public class CustomerDetailsCommands
    {
        [Required]
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
