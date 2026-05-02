using FirstProjectWebAPI.Commands.CustomerDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace FirstProjectWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EcommerceProductController : ControllerBase
    {
        private readonly CustomerDetailsCommandsHandler _Handler;
        public EcommerceProductController(CustomerDetailsCommandsHandler handler)
        {
            _Handler = handler;
        }
        [HttpPost("CustomerInsert")]
        public async Task<IActionResult> CustomerInsert([FromBody] CustomerDetailsCommands command)
        {
            var result = await _Handler.InsertCustomerDetails(command);
            return Ok(result);
        }

    }
}
