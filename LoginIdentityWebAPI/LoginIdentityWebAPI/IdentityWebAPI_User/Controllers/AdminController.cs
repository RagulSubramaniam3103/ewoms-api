using IdentityWebAPI_User.MainModel;
using IdentityWebAPI_User.MainModel.UserEndModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityWebAPI_User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<CustomerDetails> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(UserManager<CustomerDetails> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomers([FromQuery] CustomerRegister customerregister)
        {
            var existingrecords = await _userManager.FindByEmailAsync(customerregister.Email);
            if (existingrecords != null)
            {
                return BadRequest(new { Message = "User Already Exists" });
            }
            var user = new CustomerDetails
            {
                UserName = customerregister.UserName,
                CustomerName = customerregister.CustomerName,
                Email = customerregister.Email
            };
            var result = await _userManager.CreateAsync(user, customerregister.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "User Creation Failed", Errors = result.Errors });
            }
            else
            {
                if (!await _roleManager.RoleExistsAsync(customerregister.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(customerregister.Role));
                }
                await _userManager.AddToRoleAsync(user, customerregister.Role);
                return Ok(new
                {
                    Message = "User created successfully",
                    Username = user.UserName,
                    Role = customerregister.Role
                });
            }
        }
        [HttpGet("AdminDashboard")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult AdminDashboard()
        {
            return Ok(new
            {
                Message = "Admin Dashboard"
            });
        }

        [HttpGet("ResetLockedEmail")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ResetLockoutbyEmail(string emailid)
        {
            var existinguser = await _userManager.FindByEmailAsync(emailid);

            if (existinguser == null)
            {
                return BadRequest(new { Message = "Invalid Email" });
            }

            var islocked = await _userManager.IsLockedOutAsync(existinguser);

            if (islocked)
            {
                await _userManager.SetLockoutEndDateAsync(existinguser, null);

                var result = await _userManager.ResetAccessFailedCountAsync(existinguser);

                if (result.Succeeded)
                {
                    return Ok(new { Message = "Access Failed Count Reset Successfully" });
                }
            }

            return BadRequest(new
            {
                Message = "Failed to Reset Access Failed Count"
            });
        }
    }
}
