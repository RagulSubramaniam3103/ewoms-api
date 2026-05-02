using EWOMS_ClassLibrary.DataIntegration;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Queries.LoginQueries
{
    public class MasterUser_LockoutHandler
    {
        private readonly UserManager<MasterUser> _userManager;
        public MasterUser_LockoutHandler(UserManager<MasterUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<string> Handler(MasterUser_LockoutCommand masterUser_Lockout)
        {
            var user = await _userManager.FindByEmailAsync(masterUser_Lockout.Email);
            if (user != null)
            {
                if (user.LockoutEnd > DateTime.UtcNow)
                {
                    if (masterUser_Lockout.ReleaseLockout)
                    {
                        await _userManager.SetLockoutEndDateAsync(user, null);
                        await _userManager.ResetAccessFailedCountAsync(user);
                        return "Your account lockout has been released. You can now try logging in again.";
                    }
                    else
                    {
                        return $"Your account is locked until {user.LockoutEnd}. Please try again later.";
                    }
                }
                else
                {
                    return "Your account is not locked. Please try logging in.";
                }
            }
            else
            {
                return "User not found.";
            }
        }
    }
}
