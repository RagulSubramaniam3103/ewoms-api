using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands.UserPost
{
    public class MasterUserDeletePostCommand
    {
        public int PostId { get; set; }
        public string AdminId { get; set; }
        public string? Reason { get; set; }
    }
}
