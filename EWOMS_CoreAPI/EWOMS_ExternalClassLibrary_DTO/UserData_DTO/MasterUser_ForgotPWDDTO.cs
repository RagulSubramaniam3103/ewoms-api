using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_ExternalClassLibrary_DTO.UserData_DTO
{
    public class MasterUser_ForgotPWDDTO
    {
        public string? Email { get; set; }
        public string? OldPassword { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? PasswordToken { get; set; }
    }
}
