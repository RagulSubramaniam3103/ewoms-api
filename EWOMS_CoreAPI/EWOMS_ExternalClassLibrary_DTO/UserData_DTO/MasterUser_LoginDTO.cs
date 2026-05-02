using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_ExternalClassLibrary_DTO.UserData_DTO
{
    public class MasterUser_LoginDTO
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
    public class MasterUser_LockoutDTO
    {
        public string? Email { get; set; }
        public bool ReleaseLockout { get; set; }
    }
}
