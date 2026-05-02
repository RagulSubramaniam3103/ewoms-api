using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands.UserDeletion
{
    public class MasterUser_DeleteFullDataCommand
    {
        public string UserId { get; set; }
        public string? AdminId { get; set; }
        public string? Reason { get; set; }
    }
}
