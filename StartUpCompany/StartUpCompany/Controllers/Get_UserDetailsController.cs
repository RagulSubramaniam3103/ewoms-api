using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StartUpCompany.CQRSMethod.Queries.UserAllDetails;
using StartUpCompany.CQRSMethod.Queries.UserGet;
using StartUpCompany.CQRSMethod.Queries.Users;

namespace StartUpCompany.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Get_UserDetailsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserQueryHandler _Handler;
        private readonly UserGetallQueryHandlerClass _HandlerGetall;
        public Get_UserDetailsController(IMediator mediator, UserQueryHandler handler, UserGetallQueryHandlerClass HandlerGetall)
        {
            _mediator = mediator;
            _Handler = handler;
            _HandlerGetall = HandlerGetall;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [EnableRateLimiting("Fixed")]
        [HttpPost("GetRegisteredParticularUser")]
        public async Task<IActionResult> GetUserDetails([FromBody] UserGetCommands user)
        {
            var result = await _mediator.Send(user);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [EnableRateLimiting("Fixed")]
        [HttpGet("GetAllRegisteredUser")]
        public async Task<IActionResult> GetAllUserDetails()
        {
            var returndata = await _Handler.Handle();
            if (returndata != null)
            {
                return Ok(returndata);
            }
            return Ok(new
            {
                Message = "No User Found"
            });
        }

        //[Authorize]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("Fixed")]
        [HttpPost("GetAllUserDetails_Masters")]
        public async Task<IActionResult> GetUsersDetails([FromBody] UserGetallQueryCommandClass UserDetails)
        {
            var returndata = await _HandlerGetall.Handle(UserDetails);
            if (returndata != null)
            {
                return Ok(returndata);
            }
            return Ok(new
            {
                Message = "No User Found"
            });
        }
    }
}
