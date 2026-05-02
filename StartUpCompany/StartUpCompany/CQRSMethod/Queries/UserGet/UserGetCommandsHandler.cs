using Microsoft.AspNetCore.Identity;
using StartUpCompany.MainModel.Data_DB;
using StartUpCompany.MainModel;
using Microsoft.EntityFrameworkCore;

namespace StartUpCompany.CQRSMethod.Queries.UserGet
{
    public class UserGetCommandsHandler
    {
        private readonly UserManager<MasterUsers> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataDBContext _context;
        public UserGetCommandsHandler(UserManager<MasterUsers> userManager, RoleManager<IdentityRole> roleManager, DataDBContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<object> Handle(UserGetCommands user, CancellationToken cancellation)
        {
            var existinguser = await _userManager.FindByEmailAsync(user.Email);
            if (existinguser != null)
            {
                var passcheck = await _userManager.CheckPasswordAsync(existinguser, user.Password);
                if (passcheck == false)
                {
                    return new
                    {
                        Message = "Invalid Email and Password"
                    };
                }
                var returnrole = await _userManager.GetRolesAsync(existinguser);
                var userrole = returnrole.FirstOrDefault();
                if (userrole != null)
                {
                    switch (userrole)
                    {
                        case "Admin":
                            var resultdata = await _context.MasterAdmin.FirstOrDefaultAsync(x => x.UserId == existinguser.Id);
                            if (resultdata != null)
                            {
                                return new
                                {
                                    AdminDetails = new
                                    {
                                        Userid = existinguser.Id,
                                        UserEmail = existinguser.Email,
                                        UserPassword = user.Password,
                                        UserRole = userrole
                                    },
                                    MasterAdmin = new
                                    {
                                        UserCarrer = resultdata.IsCareerStart,
                                        UserPreviousSchool = resultdata.PreviousSchool,
                                        UserJoining = resultdata.AdminJoiningDate,
                                        UserPhoneno = resultdata.AdminPhone,
                                        UserAddress1 = resultdata.Address1,
                                        UserAddress2 = resultdata.Address2,
                                        UserVillage = resultdata.Village,
                                        UserPincode = resultdata.Pincode,
                                        UserCity = resultdata.City,
                                        UserState = resultdata.State,
                                        UserCountry = resultdata.Country,
                                    }
                                };
                            }
                            else
                            {
                                return new
                                {
                                    Message = "No User"
                                };
                            }
                        case "Student":
                            var resultdata1 = await _context.MasterUsers.FirstOrDefaultAsync(x => x.UserId == existinguser.Id);
                            if (resultdata1 != null)
                            {
                                return new
                                {
                                    AdminDetails = new
                                    {
                                        Userid = existinguser.Id,
                                        UserEmail = existinguser.Email,
                                        UserPassword = user.Password,
                                        UserRole = userrole
                                    },
                                    MasterAdmin = new
                                    {
                                        UserUniqueCode = resultdata1.StudentUniqueCode,
                                        UserCarrer = resultdata1.IsFreshStudent,
                                        UserPreviousSchool = resultdata1.StudPreviousSchool,
                                        UserJoining = resultdata1.StudentJoining,
                                        UserDescription = resultdata1.StudDescription,
                                        UserPhoneno = resultdata1.StudPhone,
                                        UserAddress1 = resultdata1.Address1,
                                        UserAddress2 = resultdata1.Address2,
                                        UserVillage = resultdata1.Village,
                                        UserPincode = resultdata1.Pincode,
                                        UserCity = resultdata1.City,
                                        UserState = resultdata1.State,
                                        UserCountry = resultdata1.Country,
                                    }
                                };
                            }
                            else
                            {
                                return new
                                {
                                    Message = "No User"
                                };
                            }
                        case "Staff":
                            var resultdata2 = await _context.MasterStaff.FirstOrDefaultAsync(x => x.UserId == existinguser.Id);
                            if (resultdata2 != null)
                            {
                                return new
                                {
                                    AdminDetails = new
                                    {
                                        Userid = existinguser.Id,
                                        UserEmail = existinguser.Email,
                                        UserPassword = user.Password,
                                        UserRole = userrole
                                    },
                                    MasterAdmin = new
                                    {
                                        UserUniqueCode = resultdata2.StaffUniqueCode,
                                        UserCarrer = resultdata2.IsCareerStart,
                                        UserPreviousSchool = resultdata2.StaffPreviousSchool,
                                        UserJoining = resultdata2.Staffjoining,
                                        UserPhoneno = resultdata2.StaffPhone,
                                        UserAddress1 = resultdata2.Address1,
                                        UserAddress2 = resultdata2.Address2,
                                        UserVillage = resultdata2.Village,
                                        UserPincode = resultdata2.Pincode,
                                        UserCity = resultdata2.City,
                                        UserState = resultdata2.State,
                                        UserCountry = resultdata2.Country,
                                    }
                                };
                            }
                            else
                            {
                                return new
                                {
                                    Message = "No User"
                                };
                            }
                    }
                }
            }
            return new
            {
                Message = "No User"
            };
        }
    }
}
