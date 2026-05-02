using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_ClassLibrary.DataIntegration
{
    public class Master_UserPasswordLog
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
