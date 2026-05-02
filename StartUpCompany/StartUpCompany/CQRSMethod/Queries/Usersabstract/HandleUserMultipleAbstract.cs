using Microsoft.AspNetCore.Identity;
using StartUpCompany.MainModel.Data_DB;
using StartUpCompany.MainModel;
using Microsoft.EntityFrameworkCore;

namespace StartUpCompany.CQRSMethod.Queries.Usersabstract
{
    public class HandleUserMultipleAbstract
    {

    }

    public class HanndlerUserAdminAbstract : AbstractUserIDRole
    {
        public readonly DataDBContext _context;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly UserManager<MasterUsers> _userManager;
        public HanndlerUserAdminAbstract(DataDBContext context, RoleManager<IdentityRole> roleManager, UserManager<MasterUsers> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public override async Task<object> ExecuteData()
        {
            UserId.UId = UserId.UId.ToLower();
            var user = await _userManager.FindByIdAsync(UserId.UId);
            if (user == null)
            {
                return new { Message = "User not found" };
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(UserRole.URole))
            {
                var adminlist = await _context.MasterAdmin.Where(x => x.UserId == UserId.UId).ToListAsync();
                foreach (var admin in adminlist)
                {
                    var finallist = new
                    {
                        Userid = user.Id,
                        Username = user.UserName,
                        UserEmail = user.Email,
                        UserRole = roles,
                        UserAddress1 = admin.Address1,
                        UserAddress2 = admin.Address2,
                        UserCity = admin.City,
                        UserState = admin.State,
                        UserPostalCode = admin.Pincode,
                        UserCountry = admin.Country,
                        UserPhoneno = admin.AdminPhone
                    };
                    return finallist;
                }

            }
            return new { Message = "User Found" };
        }
    }


    public class HanndlerUserStudentAbstract : AbstractUserIDRole
    {
        public readonly DataDBContext _context;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly UserManager<MasterUsers> _userManager;
        public HanndlerUserStudentAbstract(DataDBContext context, RoleManager<IdentityRole> roleManager, UserManager<MasterUsers> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public override async Task<object> ExecuteData()
        {
            UserId.UId = UserId.UId.ToLower();
            var user = await _userManager.FindByIdAsync(UserId.UId);
            if (user == null)
            {
                return new { Message = "User not found" };
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(UserRole.URole))
            {
                var studentlist = await _context.MasterUsers.Where(x => x.UserId == UserId.UId).ToListAsync();
                foreach (var admin in studentlist)
                {
                    var finallist = new
                    {
                        Userid = user.Id,
                        Username = user.UserName,
                        UserEmail = user.Email,
                        UserRole = roles,
                        UserAddress1 = admin.Address1,
                        UserAddress2 = admin.Address2,
                        UserCity = admin.City,
                        UserState = admin.State,
                        UserPostalCode = admin.Pincode,
                        UserCountry = admin.Country,
                        UserPhoneno = admin.StudPhone
                    };
                    return finallist;
                }

            }
            return new { Message = "User Found" };
        }
    }

    public class HanndlerUserStaffAbstract : AbstractUserIDRole
    {
        public readonly DataDBContext _context;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly UserManager<MasterUsers> _userManager;
        public HanndlerUserStaffAbstract(DataDBContext context, RoleManager<IdentityRole> roleManager, UserManager<MasterUsers> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public override async Task<object> ExecuteData()
        {
            UserId.UId = UserId.UId.ToLower();
            var user = await _userManager.FindByIdAsync(UserId.UId);
            if (user == null)
            {
                return new { Message = "User not found" };
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(UserRole.URole))
            {
                var stafflist = await _context.MasterStaff.Where(x => x.UserId == UserId.UId).ToListAsync();
                foreach (var admin in stafflist)
                {
                    var finallist = new
                    {
                        Userid = user.Id,
                        Username = user.UserName,
                        UserEmail = user.Email,
                        UserRole = roles,
                        UserAddress1 = admin.Address1,
                        UserAddress2 = admin.Address2,
                        UserCity = admin.City,
                        UserState = admin.State,
                        UserPostalCode = admin.Pincode,
                        UserCountry = admin.Country,
                        UserPhoneno = admin.StaffPhone
                    };
                    return finallist;
                }

            }
            return new { Message = "User Found" };
        }
    }
}
