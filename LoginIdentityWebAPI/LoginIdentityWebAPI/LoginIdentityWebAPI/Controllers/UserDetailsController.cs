using LoginIdentityWebAPI.Services;
using LoginIdentityWebAPI.UserControlled;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoginIdentityWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailsController : ControllerBase
    {
        private readonly IBridgeservices _bridgeservices;
        public UserDetailsController(IBridgeservices bridgeservices)
        {
            _bridgeservices = bridgeservices;
        }
        [HttpPost("RegisterMethod")]
        public async Task<IActionResult> Insertiondata([FromBody] UserMainDetails mainDetails)
        {
            var result = await _bridgeservices.RegisterUserAsync(mainDetails);
            return Ok();
        }
    }
}
