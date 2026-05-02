using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using EWOMS_ClassLibrary.JWTToken;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands
{
    public class MasterForgotpassprofilehandler
    {
        private readonly UserManager<MasterUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        public MasterForgotpassprofilehandler(UserManager<MasterUser> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }
        public async Task<object> Handler(MasterForgotpassprofilecommand ForgotPwdCommand)
        {
            var existinguser = await _userManager.FindByEmailAsync(ForgotPwdCommand.Email);
            if (existinguser != null)
            {
                if(ForgotPwdCommand.ConfirmPassword == ForgotPwdCommand.Password)
                {
                    var changepassword = await _userManager.ChangePasswordAsync(existinguser, ForgotPwdCommand.OldPassword, ForgotPwdCommand.Password);
                    if(changepassword != null)
                    {
                        var returndata = new
                        {
                            Message = "Password Reset Successfully"
                        };
                        return returndata;
                    }
                    else
                    {
                        var returndata = new
                        {
                            Message = "Password Not Reset"
                        };
                        return returndata;
                    }
                }
                else
                {
                    var returndata = new
                    {
                        Message = "Password Not Matched"
                    };
                    return returndata;
                }
            }
            else
            {
                var returndata = new
                {
                    Message = "No User Found"
                };
                return returndata;
            }
        }
    }
}
