using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_ExternalClassLibrary_DTO.UserData_DTO
{
    public class Master_UserDTO
    {
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? UserRole { get; set; }
    }
}
