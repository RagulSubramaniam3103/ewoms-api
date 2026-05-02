using IdentityWebAPI_User.Data;
using IdentityWebAPI_User.MainModel;
using IdentityWebAPI_User.MainModel.UserEndModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityWebAPI_User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<CustomerDetails> _userManager;
        public LoginController(UserManager<CustomerDetails> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
        }
        [HttpGet("Login")]
        public async Task<IActionResult> LoginCustomers(GenerateToken GenerateToken,[FromQuery] CustomerLogin cusomterlogin)
        {
            var existinguser = await _userManager.FindByEmailAsync(cusomterlogin.Email);
            if (existinguser == null)
            {
                return BadRequest(new { Message = "Invalid Email" });
            }

            if (await _userManager.IsLockedOutAsync(existinguser))
            {
                return BadRequest(new
                {
                    Message = "Your account is locked due to multiple failed login attempts. Try again later."
                });
            }


            var passwordcheck = await _userManager.CheckPasswordAsync(existinguser, cusomterlogin.Password);
            if (!passwordcheck)
            {
                await _userManager.AccessFailedAsync(existinguser);

                var attempts = await _userManager.GetAccessFailedCountAsync(existinguser);
                return BadRequest(new { Message = "Invalid Email or Password" });
            }
            var tokenservice = GenerateToken.TokenGenerate(existinguser, await _userManager.GetRolesAsync(existinguser));
            var finalresult = new
            {
                Username = existinguser.UserName,
                UserEmail = existinguser.Email,
                Token = tokenservice,
            };
            return Ok(finalresult);
        }
    }
}
