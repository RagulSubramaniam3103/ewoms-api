using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EWOMS_Application_CQRS.Commands.UpdateUser
{
    public class MasterAdminUpdateHandler
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<MasterUser> _userManager;
        public MasterAdminUpdateHandler(ApplicationDbContext context, UserManager<MasterUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<object> HanlderUpdate(MasterAdminUpdateCommand command)
        {
            var user = await _userManager.FindByIdAsync(command.UserId);

            if (user == null)
            {
                return new { Message = "User not found" };
            }

            // ? 1. Update Identity Table
            user.UserName = command.UserName;
            user.FullName = command.FullName;
            user.Email = command.Email;
            user.PhoneNumber = command.PhoneNumber;
            user.IsPrivate = command.IsPrivate;

            if (command.ProfileImage != null)
                user.ProfileImage = command.ProfileImage;

            var identityResult = await _userManager.UpdateAsync(user);

            if (!identityResult.Succeeded)
            {
                return new
                {
                    Message = "Update failed",
                    Errors = identityResult.Errors.Select(e => e.Description)
                };
            }

            var admin = await _context.Master_Admins
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (admin == null)
            {
                return new { Message = "Admin record not found" };
            }

            admin.UserName = command.UserName;
            admin.FullName = command.FullName;
            admin.Email = command.Email;
            admin.PhoneNumber = command.PhoneNumber;
            user.PhoneNumber = command.PhoneNumber;
            user.IsPrivate = command.IsPrivate;

            admin.Address1 = command.Address1;
            admin.Address2 = command.Address2;
            admin.City = command.City;
            admin.State = command.State;
            admin.PostalCode = command.PostalCode;
            admin.Country = command.Country;

            await _context.SaveChangesAsync();

            return new
            {
                Message = "Admin profile updated successfully"
            };
        }
    }
}
