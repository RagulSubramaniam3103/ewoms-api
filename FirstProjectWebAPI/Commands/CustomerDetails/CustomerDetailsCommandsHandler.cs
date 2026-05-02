using FirstProjectWebAPI.MainData.DBContextFiles;
using Microsoft.AspNetCore.Identity;
using FirstProjectWebAPI.MainData.ModelsMigration;

namespace FirstProjectWebAPI.Commands.CustomerDetails
{
    public class CustomerDetailsCommandsHandler
    {
            private readonly UserManager<MainData.ModelsMigration.CustomerDetails> _userManager;

            public CustomerDetailsCommandsHandler(
                UserManager<MainData.ModelsMigration.CustomerDetails> userManager)
            {
                _userManager = userManager;
            }

        public async Task<string> InsertCustomerDetails(CustomerDetailsCommands command)
        {
            var customer = new MainData.ModelsMigration.CustomerDetails
            {
                UserName = command.Email,
                Email = command.Email,
                CustomerName = command.CustomerName
            };

            var result = await _userManager.CreateAsync(customer, command.Password);

            if (!result.Succeeded)
            {
                return string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return "Customer Created Successfully";
        }
    }
}
