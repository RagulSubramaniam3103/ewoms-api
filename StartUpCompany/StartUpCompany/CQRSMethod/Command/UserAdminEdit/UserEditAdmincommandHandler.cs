using MediatR;
using Microsoft.AspNetCore.Identity;
using StartUpCompany.MainModel.Data_DB;
using StartUpCompany.MainModel;
using Microsoft.EntityFrameworkCore;

namespace StartUpCompany.CQRSMethod.Command.UserAdminEdit
{
    public class UserEditAdmincommandHandler : IRequestHandler<UserEditAdminCommand, object>
    {
        private readonly UserManager<MasterUsers> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataDBContext _context;
        public UserEditAdmincommandHandler(RoleManager<IdentityRole> roleManager, UserManager<MasterUsers> userManager, DataDBContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        public async Task<object> Handle(UserEditAdminCommand userEdit, CancellationToken cancellationToken)
        {
            var existingAdmin = await _context.MasterAdmin.FirstOrDefaultAsync(x => x.AdminEmail == userEdit.Email);
            if (existingAdmin == null)
            {
                return new
                {
                    Message = "No user Edited"
                };
            }
            existingAdmin.IsCareerStart = userEdit.IsCareerStart;
            existingAdmin.PreviousSchool = userEdit.PreviousSchool;
            existingAdmin.AdminJoiningDate = userEdit.AdminJoiningDate;
            existingAdmin.AdminPhone = userEdit.AdminPhone;
            existingAdmin.Address1 = userEdit.Address1;
            existingAdmin.Address2 = userEdit.Address2;
            existingAdmin.Village = userEdit.Village;
            existingAdmin.City = userEdit.City;
            existingAdmin.State = userEdit.State;
            existingAdmin.Country = userEdit.Country;
            existingAdmin.Pincode = userEdit.Pincode;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return new
                {
                    Message = "Updated Successfully"
                };
            }
            else
            {
                return new
                {
                    Message = "User Not Updated"
                };
            }
        }
    }
}
