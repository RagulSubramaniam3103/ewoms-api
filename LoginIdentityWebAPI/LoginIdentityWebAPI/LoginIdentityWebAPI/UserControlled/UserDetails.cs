
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginIdentityWebAPI.UserControlled
{
    public class UserDetails
    {
        [Key]
        [ForeignKey(nameof(UserMainDetails))]
        public int EmpId { get; set; }

        [Required] public string? EmpRole { get; set; }
        [Required] public string? EmpDepartment { get; set; }
        [Required] public string? EmpPosition { get; set; }
        [Required] public string? EmpStatus { get; set; }
        [Required] public DateTime EmpJoiningDate { get; set; }
        [Required] public DateTime? EmpStatusDate { get; set; }
        [Required] public decimal EmpSalary { get; set; }
        [Required] public string? EmpAddress { get; set; }
        [Required] public string? EmpCity { get; set; }
        [Required] public string? EmpState { get; set; }
        [Required] public string? EmpCountry { get; set; }
        [Required] public string? EmpZipCode { get; set; }
        [Required] public DateTime EmpDOB { get; set; }
        [Required] public string? EmpGender { get; set; }
        [Required] public string? EmpMaritalStatus { get; set; }
        [Required] public string? EmpNationality { get; set; }

        public UserMainDetails? UserMainDetails { get; set; }
    }

}
