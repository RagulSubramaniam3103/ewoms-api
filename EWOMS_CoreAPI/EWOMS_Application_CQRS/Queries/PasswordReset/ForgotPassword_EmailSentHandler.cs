using EWOMS_ClassLibrary.DataIntegration;
using EWOMS_ClassLibrary.EmailSending;
using EWOMS_ClassLibrary.JWTToken;
using EWOMS_ExternalClassLibrary_DTO.UserData_DTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Queries.PasswordReset
{
    public class ForgotPassword_EmailSentHandler
    {
        private readonly UserManager<MasterUser> _userManager;
        private readonly ForgotPasswordTokenGenerate _forgotPasswordToken;
        private readonly IEmailService _emailService;
        public ForgotPassword_EmailSentHandler(UserManager<MasterUser> userManager, ForgotPasswordTokenGenerate forgotPasswordToken, IEmailService emailService)
        {
            _userManager = userManager;
            _forgotPasswordToken = forgotPasswordToken;
            _emailService = emailService;
        }
        public async Task<object> Handler(ForgotPassword_EmailSentCommand forgotPassword_EmailSentCommand)
        {
            var existinguser = await _userManager.FindByEmailAsync(forgotPassword_EmailSentCommand.Email);
            if (existinguser != null)
            {
                var emailclass = new Master_EmailSent
                {
                    Email = existinguser.Email,
                };
//                var generatedtoken = await _forgotPasswordToken.HandleToken(emailclass);
                var token = await _userManager.GeneratePasswordResetTokenAsync(existinguser);
                var encodedToken = Uri.EscapeDataString(token);
                var resetLink = $"http://localhost:4200/forgot-password?email={existinguser.Email}&token={encodedToken}";

                var body = $@"
                    <div style='font-family: Arial, sans-serif; line-height:1.6;'>
                        <h2 style='color:#2c3e50;'>EWOMS Password Reset</h2>
    
                        <p>Hello,</p>
    
                        <p>We received a request to reset your password.</p>
    
                        <p>
                            <a href='{resetLink}' 
                               style='background-color:#007bff;color:#fff;padding:10px 15px;
                                      text-decoration:none;border-radius:5px;display:inline-block;'>
                               Reset Password
                            </a>
                        </p>
    
                        <p>This link will expire shortly for security reasons.</p>
    
                        <p>If you did not request this, please ignore this email.</p>
    
                        <br/>
                        <p>Regards,<br/><b>EWOMS Team</b></p>
                    </div>
                    ";
                await _emailService.SendEmailAsync(existinguser.Email, "Reset Password", resetLink);
                return new
                {
                    Message = "Email Sent to Email ID"
                };
            }
            else
            {
                return new
                {
                    Message = "Email Not Sent to Email ID"
                };
            }
            
        }
    }
}
