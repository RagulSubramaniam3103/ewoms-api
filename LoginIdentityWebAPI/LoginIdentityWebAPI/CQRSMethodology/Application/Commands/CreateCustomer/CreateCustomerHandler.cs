using CQRSMethodology.Data.MigrationApplication;
using LoginIdentityWebAPI.Data;

namespace CQRSMethodology.Application.Commands.CreateCustomer
{
    public class CreateCustomerHandler
    {
        private readonly ApplicationDBContext _context;
        public CreateCustomerHandler(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<string> Handlinginsertcustomer_Records(CreateCustomerCommand command)
        {
            var customer = new CustomerDetails
            {
                CustomerName = command.CustomerName,
                CustomerEmail = command.CustomerEmail,
                CustomerPhone = command.CustomerPhone
            };

            var existingrecords = _context.CustomerDetails.FirstOrDefault(c => c.CustomerEmail == command.CustomerEmail);

            if(existingrecords == null)
            {
                _context.CustomerDetails.Add(customer);
                var resultdata = await _context.SaveChangesAsync();
                if (resultdata > 0)
                    return "Ok";
                else
                    return "Failed";
            }
            else
            {
                return "Email Already Had Registered";
            }
        }

    }
}
