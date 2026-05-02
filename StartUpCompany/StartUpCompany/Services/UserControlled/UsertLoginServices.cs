using Microsoft.AspNetCore.Identity;
using StartUpCompany.CQRSMethod.Queries.UserControlled;
using StartUpCompany.CQRSMethod.Queries.Usersabstract;
using StartUpCompany.MainModel;

namespace StartUpCompany.Services.UserControlled
{
    public class UsertLoginServices: IUserLoginServices
    {
        private readonly UserManager<MasterUsers> _userManager;
        private readonly SignInManager<MasterUsers> _signInManager;
        public UsertLoginServices(UserManager<MasterUsers> userManager, SignInManager<MasterUsers> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<object> UserLogin(UserControlled_QueryCommand userctlcmd)
        {
            var existsuser = await _userManager.FindByEmailAsync(userctlcmd.UserLogin);
            if (existsuser != null)
            {
                var userRole = await _userManager.GetRolesAsync(existsuser);
                var userRoleString = userRole.FirstOrDefault() ?? "No Role";
                var authenticatecheck = await _signInManager.CheckPasswordSignInAsync(existsuser, userctlcmd.UserPassword, true);
                if (authenticatecheck.Succeeded)
                {
                    return new LoginCommandResponse
                    {
                        Success = true,
                        UserId = existsuser.Id,
                        UserLogin = existsuser.Email,
                        UserPassword = userctlcmd.UserPassword,
                        UserRole = userRoleString,
                        Message = "Login successful"
                    };
                }
            }
            return new LoginCommandResponse
            {
                Success = false,
                UserId = null,
                UserLogin = userctlcmd.UserLogin,
                UserPassword = userctlcmd.UserPassword,
                UserRole = null,
                Message = "User not found or password is incorrect"
            };
        }
    }
}
