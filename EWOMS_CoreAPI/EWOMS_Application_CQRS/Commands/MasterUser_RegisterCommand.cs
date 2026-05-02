using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using Microsoft.AspNetCore.Identity;

namespace EWOMS_Application_CQRS.Commands
{
    public class MasterUser_RegisterCommand
    {
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? UserRoles { get; set; }
        public byte[]? ProfileImage { get; set; }    
    }

    public enum UserRoles
    {
        Admin,
        Manager,
        Employee
    }
}
