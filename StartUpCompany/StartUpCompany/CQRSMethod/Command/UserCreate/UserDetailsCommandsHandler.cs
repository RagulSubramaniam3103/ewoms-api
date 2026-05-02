using Microsoft.AspNetCore.Identity;
using StartUpCompany.MainModel.Data_DB;
using StartUpCompany.MainModel;
using StartUpCompany.MainModel.Data_Admin_Staff;
using StartUpCompany.MainModel.Data_Student;

namespace StartUpCompany.CQRSMethod.Command.UserCreate
{
    public class UserDetailsCommandsHandler
    {
        private readonly UserManager<MasterUsers> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataDBContext _context;

        public UserDetailsCommandsHandler(UserManager<MasterUsers> userManager, RoleManager<IdentityRole> roleManager, DataDBContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<string> CreateNewUser(UserDetailsCommands userDetails)
        {
            var user = new MasterUsers
            {
                UserName = userDetails.UserName,
                Email = userDetails.Email
            };
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return "Email Already Exists";
            }
            else
            {
                await _userManager.CreateAsync(user, userDetails.Password);
                if (!await _roleManager.RoleExistsAsync(userDetails.UserType.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole(userDetails.UserType.ToString()));
                }
                await _userManager.AddToRoleAsync(user, userDetails.UserType.ToString());

                var userid = _userManager.FindByEmailAsync(user.Email).Result.Id;

                switch (userDetails.UserType)
                {
                    case UserType.Admin:
                        var admin = new MasterAdmin
                        {
                            UserId = userid,
                            AdminName = userDetails.UserName,
                            AdminEmail = userDetails.Email
                        };
                        _context.MasterAdmin.Add(admin);
                        break;
                    case UserType.Staff:
                        var staff = new MasterStaff
                        {
                            UserId = userid,
                            StaffName = userDetails.UserName,
                            StaffEmail = userDetails.Email
                        };
                        _context.MasterStaff.Add(staff);
                        break;
                    case UserType.Student:
                        var student = new MasterStudent
                        {
                            UserId = userid,
                            StudName = userDetails.UserName,
                            StudEmail = userDetails.Email
                        };
                        _context.MasterUsers.Add(student);
                        break;
                }

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return "User Created Successfully";
                return "";
            }
        }
    }
}
