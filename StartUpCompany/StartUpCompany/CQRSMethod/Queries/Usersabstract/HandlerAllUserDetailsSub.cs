using Microsoft.AspNetCore.Identity;
using StartUpCompany.MainModel.Data_DB;
using StartUpCompany.MainModel;
using Microsoft.EntityFrameworkCore;

namespace StartUpCompany.CQRSMethod.Queries.Usersabstract
{

    public class HandlerAllUserDetailsSub
    {

    }
    public class HandlerAllUserDetails_Admin : AbstractUserDetails
    {
        public readonly DataDBContext _context;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly UserManager<MasterUsers> _userManager;
        public HandlerAllUserDetails_Admin(DataDBContext context, RoleManager<IdentityRole> roleManager, UserManager<MasterUsers> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public override async Task<object> ExecuteDetails()
        {
            UserRole.URole = UserRole.URole.ToLower();
            var resultList = new List<object>();
            var data = await _userManager.Users.ToListAsync();
            if (data != null)
            {
                foreach (var userlist in data)
                {
                    var userrole = await _userManager.GetRolesAsync(userlist);
                    var roles = userrole.FirstOrDefault();

                    if (Enum.TryParse<UserRole>(roles, true, out var role))
                    {
                        if (roles.ToLower() == UserRole.URole.ToLower())
                        {
                            var getadmindata = await _context.MasterAdmin.Where(x => x.UserId == userlist.Id).Select(x => new
                            {
                                x.Id,
                                x.AdminName,
                                x.AdminEmail,
                                x.AdminPhone,
                                x.City,
                                x.State
                            }).FirstOrDefaultAsync();
                            var finalvariable = new
                            {
                                UserId = userlist.Id,
                                UserName = userlist.UserName,
                                Email = userlist.Email,
                                Role = roles,
                                AdminData = getadmindata
                            };
                            resultList.Add(finalvariable);
                        }
                    }
                }
                return resultList;
            }

            return await _userManager.Users.ToListAsync();
        }
    }
    public class HandlerAllUserDetails_Student : AbstractUserDetails
    {
        public readonly DataDBContext _context;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly UserManager<MasterUsers> _userManager;
        public HandlerAllUserDetails_Student(DataDBContext context, RoleManager<IdentityRole> roleManager, UserManager<MasterUsers> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public override async Task<object> ExecuteDetails()
        {
            UserRole.URole = UserRole.URole.ToLower();
            var resultList = new List<object>();
            var data = await _userManager.Users.ToListAsync();
            if (data != null)
            {
                foreach (var userlist in data)
                {
                    var userrole = await _userManager.GetRolesAsync(userlist);
                    var roles = userrole.FirstOrDefault();

                    if (Enum.TryParse<UserRole>(roles, true, out var role))
                    {
                        if (roles.ToLower() == UserRole.URole.ToLower())
                        {
                            var getuserdata = await _context.MasterUsers.Where(x => x.UserId == userlist.Id).Select(x => new
                            {
                                x.UserId,
                                x.StudName,
                                x.StudEmail,
                                x.StudPhone,
                                x.City,
                                x.State
                            }).FirstOrDefaultAsync();
                            var finalvariable = new
                            {
                                UserId = userlist.Id,
                                UserName = userlist.UserName,
                                Email = userlist.Email,
                                Role = roles,
                                UserData = getuserdata
                            };
                            resultList.Add(finalvariable);
                        }
                    }
                }
                return resultList;
            }

            return await _userManager.Users.ToListAsync();
        }
    }
    public class HandlerAllUserDetails_Staff : AbstractUserDetails
    {
        public readonly DataDBContext _context;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly UserManager<MasterUsers> _userManager;
        public HandlerAllUserDetails_Staff(DataDBContext context, RoleManager<IdentityRole> roleManager, UserManager<MasterUsers> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public override async Task<object> ExecuteDetails()
        {
            UserRole.URole = UserRole.URole.ToLower();
            var resultList = new List<object>();
            var data = await _userManager.Users.ToListAsync();
            if (data != null)
            {
                foreach (var userlist in data)
                {
                    var userrole = await _userManager.GetRolesAsync(userlist);
                    var roles = userrole.FirstOrDefault();

                    if (Enum.TryParse<UserRole>(roles, true, out var role))
                    {
                        if (roles.ToLower() == UserRole.URole.ToLower())
                        {
                            var getstaffdata = await _context.MasterStaff.Where(x => x.UserId == userlist.Id).Select(x => new
                            {
                                x.UserId,
                                x.StaffName,
                                x.StaffEmail,
                                x.StaffPhone,
                                x.City,
                                x.State
                            }).FirstOrDefaultAsync();
                            var finalvariable = new
                            {
                                UserId = userlist.Id,
                                UserName = userlist.UserName,
                                Email = userlist.Email,
                                Role = roles,
                                StaffData = getstaffdata
                            };
                            resultList.Add(finalvariable);
                        }
                    }
                }
                return resultList;
            }

            return await _userManager.Users.ToListAsync();
        }
    }
}
