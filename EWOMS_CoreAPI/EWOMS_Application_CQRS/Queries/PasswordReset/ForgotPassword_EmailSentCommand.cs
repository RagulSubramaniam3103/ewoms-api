using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Queries.PasswordReset
{
    public class ForgotPassword_EmailSentCommand
    {
        public string? Email { get; set; }
    }
}
