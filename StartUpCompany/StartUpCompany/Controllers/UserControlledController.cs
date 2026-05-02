using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StartUpCompany.CQRSMethod.Queries.UserControlled;

namespace StartUpCompany.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserControlledController : ControllerBase
    {
        private readonly UserControlled_QueryCommandHandler _QueryCommandHandler;
        public UserControlledController(UserControlled_QueryCommandHandler QueryCommandHandler)
        {
            _QueryCommandHandler = QueryCommandHandler;
        }
        [HttpGet("UserLogin")]
        public async Task<IActionResult> UserLogin([FromQuery] UserControlled_QueryCommand userCtlCommand)
        {
            var result = await _QueryCommandHandler.Handler(userCtlCommand);
            return Ok(result);
        } 
    }
}
