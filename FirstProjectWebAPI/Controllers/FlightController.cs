using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstProjectWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        [HttpGet("GetFlight")]
        public IActionResult Flightdetails()
        {
            var result = new
            {
                Message = "TestMessage"
            };
            return Ok(result);
        }
    }
}
