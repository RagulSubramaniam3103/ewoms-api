using LoginIdentityWebAPI.Data;
using LoginIdentityWebAPI.UserControlled;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoginIdentityWebAPI.Services
{
    public class Bridgeservices : IBridgeservices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDBContext _context;
        public Bridgeservices(UserManager<ApplicationUser> userManager, AppDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<bool> RegisterUserAsync(UserMainDetails userMain)
        {
            if (userMain == null)
                return false;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = new ApplicationUser
                {
                    UserName = userMain.EmpEmail,
                    Email = userMain.EmpEmail,
                    PhoneNumber = userMain.EmpPhoneNumber
                };
                var identityResult = await _userManager.CreateAsync(user, userMain.EmpPassword);
                userMain.ApplicationUserId = user.Id;

                await _context.UserMainDetails.AddAsync(userMain);
                await _context.SaveChangesAsync(); 
                
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }


    }
}
