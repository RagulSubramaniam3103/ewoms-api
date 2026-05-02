using Microsoft.AspNetCore.Identity;
using StartUpCompany.MainModel;
using StartUpCompany.Services.GenerateToken;
using StartUpCompany.Services.UserControlled;

namespace StartUpCompany.CQRSMethod.Queries.UserControlled
{
    public class UserControlled_QueryCommandHandler
    {
        private readonly IUserLoginServices _userLoginServices;
        private readonly UserManager<MasterUsers> _userManager;
        private readonly UserControlled_Login _userControlledLogin;
        public UserControlled_QueryCommandHandler(IUserLoginServices userLoginServices, UserManager<MasterUsers> userManager,
            UserControlled_Login userControlledLogin)
        {
            _userLoginServices = userLoginServices;
            _userManager = userManager;
            _userControlledLogin = userControlledLogin;
        }
        public async Task<object> Handler(UserControlled_QueryCommand userctlcmd)
        {
            var result = await _userLoginServices.UserLogin(userctlcmd);
            if (result != null)
            {
                var success = (result as LoginCommandResponse)?.Success ?? false;
                if (success)
                {
                    var generatedtoken = await _userControlledLogin.GenerateToken(result as LoginCommandResponse);
                    var resultwithToken = new
                    {
                        UserDetails = result,
                        token = generatedtoken
                    };
                    return resultwithToken;
                }
            }
            return result as LoginCommandResponse;
        }
    }
}
