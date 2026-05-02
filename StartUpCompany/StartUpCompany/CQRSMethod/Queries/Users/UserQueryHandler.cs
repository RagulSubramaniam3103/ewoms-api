using Microsoft.AspNetCore.Identity;
using StartUpCompany.MainModel.Data_DB;
using StartUpCompany.MainModel;

namespace StartUpCompany.CQRSMethod.Queries.Users
{
    public class UserQueryHandler
    {
        private readonly UserManager<MasterUsers> _userManager;
        private readonly DataDBContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserQueryHandler(UserManager<MasterUsers> userManager, DataDBContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }
        public async Task<List<UserQueryCommands>> Handle()
        {
            var data = _userManager.Users.ToList();

            var finaldata = new List<UserQueryCommands>();

            foreach (var user in data)
            {
                var roleget = await _userManager.GetRolesAsync(user);

                finaldata.Add(new UserQueryCommands
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserRole = roleget.FirstOrDefault()
                });
            }

            return finaldata;
        }
    }
}
