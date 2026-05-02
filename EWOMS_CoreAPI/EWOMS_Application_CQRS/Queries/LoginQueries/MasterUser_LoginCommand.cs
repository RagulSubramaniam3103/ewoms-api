using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Queries.LoginQueries
{
    public class MasterUser_LoginCommand
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
