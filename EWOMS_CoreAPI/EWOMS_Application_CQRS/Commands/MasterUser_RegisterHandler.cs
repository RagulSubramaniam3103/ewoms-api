using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using EWOMS_ExternalClassLibrary_DTO.UserData_DTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands
{
    public class MasterUser_RegisterHandler
    {
        private readonly UserManager<MasterUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public MasterUser_RegisterHandler(
            UserManager<MasterUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<string> Handler(MasterUser_RegisterCommand _masteruser)
        {
            if (string.IsNullOrEmpty(_masteruser.Email))
                return "Email is required.";

            var validRoles = new[] { "Admin", "User", "Manager" };
            var role = _masteruser.UserRoles?.ToString();

            if (string.IsNullOrEmpty(role) || !validRoles.Contains(role))
                return "Invalid Role Selected";

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existinguser = await _userManager.FindByEmailAsync(_masteruser.Email);
                if (existinguser != null)
                    return "Email already exists.";

                var user = new MasterUser
                {
                    UserName = _masteruser.UserName,
                    FullName = _masteruser.FullName,
                    Email = _masteruser.Email,
                    CreatedUser = DateTime.UtcNow,
                    ProfileImage = _masteruser.ProfileImage
                };

                var result = await _userManager.CreateAsync(user, _masteruser.Password);

                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return "User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description));
                }

                await _userManager.AddToRoleAsync(user, role);

                var userpasswordlog = new Master_UserPasswordLog
                {
                    UserId = user.Id,
                    PasswordHash = user.PasswordHash,
                    CreatedDate = DateTime.UtcNow
                };

                await _context.Master_UserPasswordLogs.AddAsync(userpasswordlog);

                if (role == "Admin")
                {
                    var adminuser = new MasterAdmin
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Email = user.Email
                    };

                    await _context.Master_Admins.AddAsync(adminuser);
                }
                else if (role == "Manager")
                {
                    var managerUser = new MasterManager
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber 
                    };

                    await _context.Master_MasterManager.AddAsync(managerUser);
                }

                else if (role == "User")
                {
                    var normalUser = new MasterUserDetails
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber 
                    };

                    await _context.Master_MasterUserDetails.AddAsync(normalUser);
                }


                var notification = new MasterNotification
                {
                    Title = "New User Registration",
                    Message = $"{role} '{user.UserName}' has been registered.",
                    CreatedDate = DateTime.UtcNow,
                    IsRead = false
                };

                await _context.masterNotifications.AddAsync(notification);

                await _context.SaveChangesAsync();


                await transaction.CommitAsync();


                return "User Created Successfully";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return "An error occurred: " + ex.Message;
            }
        }
    }
}