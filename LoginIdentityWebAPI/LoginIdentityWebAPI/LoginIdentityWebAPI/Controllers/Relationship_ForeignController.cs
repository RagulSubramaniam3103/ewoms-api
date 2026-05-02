using LoginIdentityWebAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoginIdentityWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Relationship_ForeignController : ControllerBase
    {
        private readonly AppDBContext _context;
        public Relationship_ForeignController(AppDBContext context)
        {
            _context = context;
        }

        [HttpPost("InsertCustomerDetails")]
        public async Task<IActionResult> InsertCustomerDetails([FromQuery] CustomerDetails details)
        {
            _context.CustomerDetails.Add(details);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                var return_result = new
                {
                    CustomerId = details.CustomerId,
                    CustomerName = details.CustomerName,
                    Message = "Customer Details Inserted Successfully"
                };
                return Ok(return_result);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
