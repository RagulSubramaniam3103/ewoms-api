using EWOMS_ClassLibrary.DataIntegration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Queries.LockoutUser
{
    public class MasterUser_GetLockoutHandler
    {
        private readonly UserManager<MasterUser> _userManager;
        public MasterUser_GetLockoutHandler(UserManager<MasterUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<object> Handler(MasterUser_GetLockoutCommand GetLockoutCommand)
        {
            var getlockoutdetails = new List<object>();
            //var getalluser = await _userManager.Users.Where(x=>x.LockoutEnd != null).Where(x => x.LockoutEnd > DateTime.UtcNow).ToListAsync();
            var now = DateTimeOffset.UtcNow;

            var getalluser = await _userManager.Users
                .Where(x => x.LockoutEnd.HasValue &&
                            x.LockoutEnd > now)
                .ToListAsync();

            if (getalluser.Any())
            {
                foreach (var user in getalluser)
                {
                    getlockoutdetails.Add(new
                    {
                        user.Id,
                        user.UserName,
                        user.Email,
                        user.PhoneNumber,
                        LockoutEndDate = user.LockoutEnd
                    });
                }
                return getlockoutdetails;
            }
            else
            {
                return new
                {
                    Message = "No locked out users found."
                };
            }
        }
    }
}
