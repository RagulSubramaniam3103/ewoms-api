using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_ClassLibrary.JWTToken
{
    public class MasterUser_TokenDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }
        public string ProfileImageBase64 { get; set; }
    }
}
