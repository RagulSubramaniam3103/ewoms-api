using LoginIdentityWebAPI.Data;
using System.ComponentModel.DataAnnotations;

namespace LoginIdentityWebAPI.UserControlled
{
    public class UserMainDetails
    {
        [Key]
        public int EmpId { get; set; }

        [Required]
        public string? EmpFullName { get; set; }

        [Required]
        public string? EmpEmail { get; set; }

        public string? EmpPhoneNumber { get; set; }

        [Required]
        public string? EmpPassword { get; set; }

        public int? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        public UserDetails? UserDetails { get; set; }
    }

}
