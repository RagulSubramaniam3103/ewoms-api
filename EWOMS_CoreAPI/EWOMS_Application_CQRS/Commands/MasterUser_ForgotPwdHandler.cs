using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using EWOMS_ClassLibrary.JWTToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands
{
    public class MasterUser_ForgotPwdHandler
    {
        private readonly UserManager<MasterUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ForgotPasswordTokenGenerate _tokenService;
        public MasterUser_ForgotPwdHandler(UserManager<MasterUser> userManager, ApplicationDbContext dbContext, ForgotPasswordTokenGenerate tokenService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _tokenService = tokenService;
        }
        public async Task<object> Handler(MasterUser_ForgotPwdCommand ForgotPwdCommand)
        {
            if (ForgotPwdCommand.Password != ForgotPwdCommand.ConfirmPassword)
                return new
                {
                    Status = "Error",
                    Message = "Password and Confirm Password do not match.",
                    Timestamp = DateTime.Now,
                };
            var existinguser = await _userManager.FindByEmailAsync(ForgotPwdCommand.Email);
            if (existinguser == null)
                return new
                {
                    Status = "Error",
                    Message = "Email does not exist.",
                    Timestamp = DateTime.Now,
                };
            var oldpasswordcheck = await _userManager.CheckPasswordAsync(existinguser, ForgotPwdCommand.OldPassword);
            if (!oldpasswordcheck)
                return new
                {
                    Status = "Error",
                    Message = "Invalid Old Password.",
                    Timestamp = DateTime.Now,
                };

            var getlastpassword = await _dbContext.Master_UserPasswordLogs.Where(x => x.UserId == existinguser.Id).OrderByDescending(x => x.CreatedDate).Take(5).ToListAsync();

            if (getlastpassword != null)
                foreach (var passwordget in getlastpassword)
                {
                    var issamepassword = _userManager.PasswordHasher.VerifyHashedPassword(existinguser, passwordget.PasswordHash, ForgotPwdCommand.Password);
                    if (issamepassword == PasswordVerificationResult.Success)
                        return new
                        {
                            Status = "Error",
                            Message = "You cannot reuse your previous Password.",
                            Timestamp = DateTime.Now,
                        };
                }

            var decodedToken = Uri.UnescapeDataString(ForgotPwdCommand.PasswordToken);

            //var resettoken = await _userManager.GeneratePasswordResetTokenAsync(existinguser);
            var resetpassword = await _userManager.ResetPasswordAsync(existinguser, decodedToken, ForgotPwdCommand.Password);
            if (getlastpassword != null)
            {
                var userpasswordlog = new Master_UserPasswordLog
                {
                    UserId = existinguser.Id,
                    PasswordHash = existinguser.PasswordHash,
                    CreatedDate = DateTime.UtcNow
                };

                var createlogpassword = await _dbContext.Master_UserPasswordLogs.AddAsync(userpasswordlog);
                await _dbContext.SaveChangesAsync();
            }
            if (resetpassword.Succeeded)
                return new
                {
                    Status = "Success",
                    Message = "Password reset successfully.",
                    Timestamp = DateTime.Now,
                };
            else
                return new
                {
                    Status = "Error",
                    Message = "Error resetting password.",
                    Timestamp = DateTime.Now,
                };
        }
    }
}
