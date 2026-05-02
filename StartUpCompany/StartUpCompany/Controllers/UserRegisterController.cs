using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StartUpCompany.CQRSMethod.Command.UserAdminEdit;
using StartUpCompany.CQRSMethod.Command.UserCreate;
using StartUpCompany.MainModel.Data_AutoMapper.UsersEdit;
using StartUpCompany.MainModel;
using StartUpCompany.CQRSMethod.Queries.UserControlled;

namespace StartUpCompany.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegisterController : ControllerBase
    {
        private readonly UserDetailsCommandsHandler _handler;
        private readonly IMediator _mediator;
        private readonly UserManager<MasterUsers> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserRegisterController(UserDetailsCommandsHandler handler, IMediator mediator, UserManager<MasterUsers> userManager, RoleManager<IdentityRole> roleManager)
        {
            _handler = handler;
            _mediator = mediator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterNewUser(UserDetailsCommands userDetails)
        {
            var result = await _handler.CreateNewUser(userDetails);
            if (result != "")
            {
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("EditUserAdmin")]
        public async Task<IActionResult> UserEditAdmin([FromBody] UserEditAdminCommand userEdit)
        {
            var existingrecordsEdit = await _mediator.Send(userEdit);
            if (existingrecordsEdit != null)
            {
                return Ok(existingrecordsEdit);
            }
            return BadRequest();
        }

        [HttpPost("EditUserDetails_AutoMapper")]
        public async Task<IActionResult> EditUserMapper([FromBody] DataUserEdit dataUser)
        {
            var edituserdetails = await _mediator.Send(dataUser);
            if (edituserdetails != null)
            {
                return Ok(edituserdetails);
            }
            return BadRequest();
        }
    }
}
