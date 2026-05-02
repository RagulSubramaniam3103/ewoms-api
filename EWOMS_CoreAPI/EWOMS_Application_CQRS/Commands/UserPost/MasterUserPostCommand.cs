using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands.UserPost
{
    public class MasterUserPostCommand
    {
        public string UserId { get; set; }
        public string Caption { get; set; }
        public byte[] Image { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
