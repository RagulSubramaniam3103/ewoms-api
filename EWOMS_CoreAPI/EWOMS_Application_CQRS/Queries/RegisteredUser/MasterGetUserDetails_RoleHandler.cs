using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Queries.RegisteredUser
{
    public class MasterGetUserDetails_RoleHandler
    {
        private readonly UserManager<MasterUser> _userManager;
        private readonly ApplicationDbContext _context;
        public MasterGetUserDetails_RoleHandler(UserManager<MasterUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<object> Handle(MasterGetUserDetails_RoleCommand Userrole)
        {
            string getroles = Userrole.UserRole_Filter.ToString();
            var getallregisteruser = await _userManager.Users.ToListAsync();
            if (getallregisteruser != null)
            {
                var userdetails = new List<object>();
                foreach (var user in getallregisteruser)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains(getroles))
                    {
                        userdetails.Add(new
                        {
                            user.Id,
                            user.UserName,
                            user.Email,
                            user.PhoneNumber,
                            Roles = roles,
                            user.ProfileImage
                        });
                    }
                }
                return userdetails;
            }
            else
            {
                return new
                {
                    Message = "No User Found with the specified role."
                };
            }
        }
    }
}
