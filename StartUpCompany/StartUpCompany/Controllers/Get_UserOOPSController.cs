using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StartUpCompany.FactoryDI;
using StartUpCompany.MainModel.Data_DB;
using StartUpCompany.MainModel;
using StartUpCompany.CQRSMethod.Queries.Usersabstract;

namespace StartUpCompany.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Get_UserOOPSController : ControllerBase
    {
        private readonly IUserAbstract _userAbstract;
        private readonly AbstractUserIDRole _abstractUserIDRole;
        private readonly DataDBContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<MasterUsers> _userManager;

        private readonly IFAbstractUserDetails _fAbstractUserDetails;
        public Get_UserOOPSController(IUserAbstract userAbstract, AbstractUserIDRole abstractUserIDRole, DataDBContext context, RoleManager<IdentityRole> roleManager,
            UserManager<MasterUsers> userManager, IFAbstractUserDetails fAbstractUserDetails)
        {
            _userAbstract = userAbstract;
            _abstractUserIDRole = abstractUserIDRole;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _fAbstractUserDetails = fAbstractUserDetails;
        }
        [HttpGet("Abstractclass")]
        public async Task<IActionResult> Abstractclass([FromQuery] string Userid)
        {
            _userAbstract.UserId.UId = Userid;
            var data = await _userAbstract.ExecutObject();
            return Ok(data);
        }

        [HttpGet("AbstractclassIDRole_WOFactory")]
        public async Task<IActionResult> Abstractclassidrole([FromQuery] string Userid, [FromQuery] string UserRoles)
        {
            if (!Enum.TryParse<UserRole>(UserRoles, true, out var role))
                return BadRequest("Invalid role");
            AbstractUserIDRole handler = role switch
            {
                UserRole.Admin => new HanndlerUserAdminAbstract(_context, _roleManager, _userManager),
                UserRole.Student => new HanndlerUserStudentAbstract(_context, _roleManager, _userManager),
                UserRole.Staff => new HanndlerUserStaffAbstract(_context, _roleManager, _userManager),
                _ => throw new Exception("Invalid role")
            };

            if (handler == null) return BadRequest("Invalid role");

            handler.UserId.UId = Userid;
            handler.UserRole.SetUserRole(role);

            var data = await handler.ExecuteData();
            return Ok(data);
        }

        [HttpGet("AbstractclassIDRole_WFactory")]
        public async Task<IActionResult> Abstractclassidrolefactory([FromQuery] string Userid, [FromQuery] string UserRoles)
        {
            if (!Enum.TryParse<UserRole>(UserRoles, true, out var role))
                return BadRequest("Invalid role");

            var handler = _fAbstractUserDetails.GetHandler(role);
            if (handler == null) return BadRequest("Invalid role");

            handler.UserId.UId = Userid;
            handler.UserRole.SetUserRole(role);
            var data = await handler.ExecuteData();

            return Ok(data);
        }
    }
}
