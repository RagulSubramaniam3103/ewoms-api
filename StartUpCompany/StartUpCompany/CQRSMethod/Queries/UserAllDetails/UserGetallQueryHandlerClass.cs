using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StartUpCompany.CQRSMethod.Queries.Usersabstract;
using StartUpCompany.FactoryDI;
using StartUpCompany.MainModel;
using StartUpCompany.MainModel.Data_DB;

namespace StartUpCompany.CQRSMethod.Queries.UserAllDetails
{
    public class UserGetallQueryHandlerClass
    {
        private readonly UserManager<MasterUsers> _userManager;
        private readonly DataDBContext _context;
        private readonly IFAbstractAllUserDetails _fAbstractAllUserDetails;

        public UserGetallQueryHandlerClass(UserManager<MasterUsers> userManager, DataDBContext context, IFAbstractAllUserDetails fAbstractAllUserDetails)
        {
            _userManager = userManager;
            _context = context;
            _fAbstractAllUserDetails = fAbstractAllUserDetails;
        }

        public async Task<object> Handle(UserGetallQueryCommandClass getuser)
        {
            if (Enum.TryParse<UserRole>(getuser.UserRole, true, out var role))
            {
                var handler = _fAbstractAllUserDetails.GetHandler(role);
                handler.UserRole.SetUserRole(role);
                var userdetails = await handler.ExecuteDetails();
                return userdetails;
            }
            return new
            {
                Message = "No Role Found"
            };
        }
    }
}
