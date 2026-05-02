using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands.UpdateUser
{
    public class MasterUserUpdateHandler
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<MasterUser> _userManager;
        public MasterUserUpdateHandler(ApplicationDbContext context, UserManager<MasterUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<object> HanlderUpdate(MasterUserUpdateCommand command)
        {
            var user = await _userManager.FindByIdAsync(command.UserId);

            if (user == null)
            {
                return new { Message = "User not found" };
            }

            // ✅ 1. Update Identity Table
            user.UserName = command.UserName;
            user.FullName = command.FullName;
            user.Email = command.Email;
            user.PhoneNumber = command.PhoneNumber;

            if (command.ProfileImage != null)
                user.ProfileImage = command.ProfileImage;

            user.IsPrivate = command.IsPrivate;

            var identityResult = await _userManager.UpdateAsync(user);

            if (!identityResult.Succeeded)
            {
                return new
                {
                    Message = "Update failed",
                    Errors = identityResult.Errors.Select(e => e.Description)
                };
            }

            var users = await _context.Master_MasterUserDetails
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (user == null)
            {
                return new { Message = "Admin record not found" };
            }

            users.UserName = command.UserName;
            users.FullName = command.FullName;
            users.Email = command.Email;
            users.PhoneNumber = command.PhoneNumber;
            users.PhoneNumber = command.PhoneNumber;

            users.Address1 = command.Address1;
            users.Address2 = command.Address2;
            users.City = command.City;
            users.State = command.State;
            users.PostalCode = command.PostalCode;
            users.Country = command.Country;

            await _context.SaveChangesAsync();

            return new
            {
                Message = "Manager profile updated successfully"
            };
        }
    }
}
