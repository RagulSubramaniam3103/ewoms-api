using Microsoft.AspNetCore.Identity;
using StartUpCompany.MainModel.Data_Admin_Staff;
using StartUpCompany.MainModel.Data_Student;

namespace StartUpCompany.MainModel
{
    public class MasterUsers : IdentityUser
    {
        public MasterAdmin? IsAdmin { get; set; }
        public MasterStaff? IsStaff { get; set; }
        public MasterStudent? IsStudent { get; set; }
    }
}
