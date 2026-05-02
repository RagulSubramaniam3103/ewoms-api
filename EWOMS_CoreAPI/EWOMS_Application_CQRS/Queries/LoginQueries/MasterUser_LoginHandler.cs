using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using EWOMS_ClassLibrary.JWTToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Queries.LoginQueries
{
    public class MasterUser_LoginHandler
    {
        private readonly UserManager<MasterUser> _userManager;
        private readonly SignInManager<MasterUser> _signInManager;
        private readonly LoginAuthentication_Token _generateToken;
        private readonly ApplicationDbContext _dbContext;

        public MasterUser_LoginHandler(
            UserManager<MasterUser> userManager,
            SignInManager<MasterUser> signInManager,
            LoginAuthentication_Token generateToken,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _generateToken = generateToken;
            _dbContext = dbContext;
        }

        public async Task<object> Handler(MasterUser_LoginCommand masterUser_Login)
        {
            var user = await _userManager.FindByEmailAsync(masterUser_Login.Email);

            if (user == null)
            {
                return new { Message = "User not found." };
            }

            var lastPasswordDate = await _dbContext.Master_UserPasswordLogs
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => x.CreatedDate)
                .FirstOrDefaultAsync();

            if (lastPasswordDate == default)
            {
                return new
                {
                    Message = "Password history not found. Please contact administration."
                };
            }

            var daysSinceChange = (DateTime.UtcNow - lastPasswordDate).TotalDays;

            if (daysSinceChange > 90)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddYears(100));

                return new
                {
                    Message = "Your password has expired. Please contact administration."
                };
            }

            var signInResult = await _signInManager.PasswordSignInAsync(
                user,
                masterUser_Login.Password,
                false,
                lockoutOnFailure: true
            );

            if (signInResult.IsLockedOut)
            {
                return new
                {
                    Message = "User is locked out. Please contact administration."
                };
            }

            if (!signInResult.Succeeded)
            {
                return new
                {
                    Message = "Invalid credentials."
                };
            }
            await _userManager.SetLockoutEndDateAsync(user, null);
            await _userManager.ResetAccessFailedCountAsync(user);

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            object roleSpecificData = null;

            if (role == "Admin")
            {
                roleSpecificData = await _dbContext.Master_Admins
                    .Where(x => x.UserId == user.Id)
                    .Select(x => new
                    {
                        x.UserName,
                        x.FullName,
                        x.Email,
                        x.PhoneNumber,
                        x.Address1,
                        x.Address2,
                        x.City,
                        x.State,
                        x.PostalCode,
                        x.Country
                    })
                    .FirstOrDefaultAsync();
            }
            else if (role == "Manager")
            {
                roleSpecificData = await _dbContext.Master_MasterManager
                    .Where(x => x.UserId == user.Id)
                    .Select(x => new
                    {
                        x.UserName,
                        x.FullName,
                        x.Email,
                        x.PhoneNumber,
                        x.Address1,
                        x.Address2,
                        x.City,
                        x.State,
                        x.PostalCode,
                        x.Country
                    })
                    .FirstOrDefaultAsync();
            }
            else if (role == "User")
            {
                roleSpecificData = await _dbContext.Master_MasterUserDetails
                    .Where(x => x.UserId == user.Id)
                    .Select(x => new
                    {
                        x.UserName,
                        x.FullName,
                        x.Email,
                        x.PhoneNumber,
                        x.Address1,
                        x.Address2,
                        x.City,
                        x.State,
                        x.PostalCode,
                        x.Country
                    })
                    .FirstOrDefaultAsync();
            }

            var userTokenData = new MasterUser_TokenDTO
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = role,
                ProfileImageBase64 = user.ProfileImage != null
                    ? Convert.ToBase64String(user.ProfileImage)
                    : null
            };

            var token = await _generateToken.GenerateToken(userTokenData);

            string warningMessage = null;

            if (daysSinceChange > 80)
            {
                warningMessage = $"Your password will expire in {(90 - daysSinceChange):0} days.";
            }
            return new
            {
                userId = user.Id,
                Token = token,
                Message = "Logged in successfully.",
                Role = role,
                Image = userTokenData.ProfileImageBase64,
                Data = roleSpecificData,
                PasswordExpiryWarning = warningMessage
            };
        }
    }
}