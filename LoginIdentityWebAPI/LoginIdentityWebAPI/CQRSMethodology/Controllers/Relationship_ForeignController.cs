using CQRSMethodology.Application.Commands.CreateCustomer;
using LoginIdentityWebAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoginIdentityWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Relationship_ForeignController : ControllerBase
    {
        private readonly CreateCustomerHandler _Handler;
        public Relationship_ForeignController(CreateCustomerHandler Handle)
        {
            _Handler = Handle;
        }

        [HttpPost("InsertCustomerDetails")]
        public async Task<IActionResult> InsertCustomerDetails([FromQuery] CreateCustomerCommand details)
        {
            var result = await _Handler.Handlinginsertcustomer_Records(details);
            if (result == "OK")
            {
                return Ok("Customer details inserted successfully.");
            }
            else
            {
                var returnmessage = new
                {
                    Message = result
                };
                return BadRequest(returnmessage);
            }
        }
    }
}
