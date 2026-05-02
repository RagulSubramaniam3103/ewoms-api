using MediatR;
using Microsoft.AspNetCore.Identity;
using StartUpCompany.MainModel.Data_AutoMapper.UsersEdit;
using StartUpCompany.MainModel.Data_DB;
using StartUpCompany.MainModel;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace StartUpCompany.CQRSMethod.Command.User_AutoMapperEdit
{
    public class UserAutoMapperCommandHandler : IRequestHandler<DataUserEdit, object>
    {
        private readonly UserManager<MasterUsers> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataDBContext _context;
        private readonly IMapper _mapper;
        public UserAutoMapperCommandHandler(UserManager<MasterUsers> userManager, RoleManager<IdentityRole> roleManager, DataDBContext context, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _mapper = mapper;
        }
        public async Task<object> Handle(DataUserEdit dataUser, CancellationToken cancellationToken)
        {
            var existinguser = await _userManager.FindByEmailAsync(dataUser.Email);
            if (existinguser != null)
            {
                var role = await _userManager.GetRolesAsync(existinguser);
                if (role != null)
                {
                    if (role.Contains("Admin"))
                    {
                        var resultadmin = await _context.MasterAdmin.FirstOrDefaultAsync(x => x.AdminEmail == dataUser.Email);
                        if (resultadmin != null)
                        {
                            _mapper.Map(dataUser, resultadmin);
                            _context.MasterAdmin.Update(resultadmin);
                            await _context.SaveChangesAsync();
                            return new
                            {
                                Message = "User Updated Successfully"
                            };
                        }
                    }
                    else if (role.Contains("Staff"))
                    {
                        var resultstaff = await _context.MasterStaff.FirstOrDefaultAsync(x => x.StaffEmail == dataUser.Email);
                        if (resultstaff != null)
                        {
                            _mapper.Map(dataUser, resultstaff);
                            _context.MasterStaff.Update(resultstaff);
                            await _context.SaveChangesAsync();
                            return new
                            {
                                Message = "User Updated Successfully"
                            };
                        }
                    }
                    else if (role.Contains("Student"))
                    {
                        var resultstudent = await _context.MasterUsers.FirstOrDefaultAsync(x => x.StudEmail == dataUser.Email);
                        if (resultstudent != null)
                        {
                            _mapper.Map(dataUser, resultstudent);
                            _context.MasterUsers.Update(resultstudent);
                            await _context.SaveChangesAsync();
                            return new
                            {
                                Message = "User Updated Successfully"
                            };
                        }
                    }
                }
            }
            return new
            {
                Message = "Not Updated"
            };
        }
    }
}
