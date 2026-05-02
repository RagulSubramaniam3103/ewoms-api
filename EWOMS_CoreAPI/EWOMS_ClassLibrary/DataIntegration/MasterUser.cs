using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_ClassLibrary.DataIntegration
{
    public class MasterUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime? CreatedUser { get; set; }
        public bool IsActive { get; set; }
        public byte[]? ProfileImage { get; set; }
        public bool IsPrivate { get; set; }
    }
}
