using Microsoft.AspNetCore.Identity;
using StartUpCompany.MainModel.Data_DB;
using StartUpCompany.MainModel;
using Microsoft.EntityFrameworkCore;

namespace StartUpCompany.CQRSMethod.Queries.Usersabstract
{
    public class HandlerUserAbstract : IUserAbstract
    {
        public readonly DataDBContext _context;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly UserManager<MasterUsers> _userManager;
        public HandlerUserAbstract(DataDBContext context, RoleManager<IdentityRole> roleManager, UserManager<MasterUsers> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public override async Task<object> ExecutObject()
        {
            var user = await _userManager.Users.Where(x => x.Id == UserId.UId).FirstOrDefaultAsync();
            if (user != null)
            {
                var roleget = await _userManager.GetRolesAsync(user);
                var role = roleget.FirstOrDefault();
                if (role == "Admin")
                {
                    var getadmin = await _context.MasterAdmin.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
                    var returnadmin = new
                    {
                        user.Id,
                        user.UserName,
                        user.Email,
                        Role = role,
                        getadmin.Address1,
                        getadmin.Address2,
                        getadmin.Village,
                        getadmin.State,
                        getadmin.Country,
                        getadmin.Pincode,
                        getadmin.AdminPhone,
                    };
                    return returnadmin;
                }
                else if (role == "Teacher")
                {
                    var getteacher = await _context.MasterStaff.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
                    var returnteacher = new
                    {
                        user.Id,
                        user.UserName,
                        user.Email,
                        Role = role,
                        getteacher.Address1,
                        getteacher.Address2,
                        getteacher.Village,
                        getteacher.State,
                        getteacher.Country,
                        getteacher.Pincode,
                        getteacher.StaffPhone,
                    };
                    return returnteacher;
                }
                else if (role == "Student")
                {
                    var getstudent = await _context.MasterUsers.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
                    var returnstudent = new
                    {
                        user.Id,
                        user.UserName,
                        user.Email,
                        Role = role,
                        getstudent.Address1,
                        getstudent.Address2,
                        getstudent.Village,
                        getstudent.State,
                        getstudent.Country,
                        getstudent.Pincode,
                        getstudent.StudPhone,
                    };
                    return returnstudent;
                }
                else
                {
                    var returnuser = new
                    {
                        Message = "No User Found"
                    };
                    return returnuser;
                }
            }
            else
            {
                var returnuser = new
                {
                    Message = "No User Found"
                };
                return returnuser;
            }
        }
    }
}
