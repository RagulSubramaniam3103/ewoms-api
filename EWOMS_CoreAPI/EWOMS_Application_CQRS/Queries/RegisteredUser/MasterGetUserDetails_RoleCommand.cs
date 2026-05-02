using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Queries.RegisteredUser
{
    public class MasterGetUserDetails_RoleCommand
    {
        public string? UserRole_Filter { get; set; }
    }
}
