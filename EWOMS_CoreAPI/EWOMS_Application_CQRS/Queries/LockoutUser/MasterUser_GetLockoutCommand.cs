using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Queries.LockoutUser
{
    public class MasterUser_GetLockoutCommand
    {
        public DateTime LockoutEndDate { get; set; }
    }
}
